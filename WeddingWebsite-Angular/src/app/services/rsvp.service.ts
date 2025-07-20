import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrl } from '../constants';
import { Invite } from '../models/invite';

// Request DTOs matching the C# models
export interface GetInviteRequest {
  inviteUniqueCode: string;
}

export interface GuestRsvpUpdate {
  id: number;
  attending: boolean | null;
  dietaryRequirements?: string;
}

export interface RsvpToInviteRequest {
  inviteId: number;
  inviteUniqueCode: string;
  guestRsvps: GuestRsvp[];
}

// This matches the backend GuestRsvp DTO exactly
export interface GuestRsvp {
  guestId: number;
  attending: boolean;
  dietaryRequirements?: string;
}

// Error response interface for handling RSVP errors
export interface RsvpErrorResponse {
  errorCode: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class RsvpService {
  private apiUrl = `${ApiUrl}/rsvp`;

  constructor(private http: HttpClient) { }

  /**
   * Checks if an HTTP error response indicates that the RSVP has already been completed
   * @param error The HTTP error response
   * @returns True if this is an "RSVP already completed" error
   */
  isRsvpAlreadyCompletedError(error: any): boolean {
    return error?.status === 409 && 
           error?.error?.errorCode === 'RSVP_ALREADY_COMPLETED';
  }

  /**
   * Gets the error message from an RSVP error response
   * @param error The HTTP error response
   * @returns The error message if available, otherwise a default message
   */
  getErrorMessage(error: any): string {
    if (this.isRsvpAlreadyCompletedError(error)) {
      return error.error.message;
    }
    return 'An unexpected error occurred. Please try again or contact us for assistance.';
  }

  /**
   * Gets an invite with its guests based on the unique invite code
   * @param inviteUniqueCode The unique code for the invite
   * @param rsvpPassword The password required to access RSVP functionality
   * @returns An Observable of the Invite object with guests included
   */
  getInviteByCode(inviteUniqueCode: string): Observable<Invite> {
    const request: GetInviteRequest = {
      inviteUniqueCode
    };    
    
    return this.http.post<Invite>(`${this.apiUrl}/get-invite`, request);
  }

  /**
   * Updates the attendance and dietary requirements for guests in an invite
   * @param invite The full invite object (needed for the ID)
   * @param guests Array of guest updates with attendance and dietary information
   * @returns An Observable of void (indicating successful completion)
   */
  updateInviteRsvp(invite: Invite, guests: GuestRsvpUpdate[]): Observable<void> {
    // Convert GuestRsvpUpdate to GuestRsvp format expected by backend
    const guestRsvps: GuestRsvp[] = guests.map(guest => ({
      guestId: guest.id,
      attending: guest.attending ?? false, // Backend requires boolean, not nullable
      dietaryRequirements: guest.dietaryRequirements
    }));

    const request: RsvpToInviteRequest = {
      inviteId: invite.id,
      inviteUniqueCode: invite.publicCode,
      guestRsvps
    };    
    
    return this.http.post<void>(`${this.apiUrl}/update-invite`, request);
  }

  /**
   * Wake up the Azure database connection
   * This method is called without waiting for the response to improve user experience
   * @returns An Observable of void
   */
  wakeUpDatabase(): Observable<void> {
    return this.http.get<void>(`${this.apiUrl}/wake-up`);
  }

  /**
   * Downloads the RSVP report as an Excel spreadsheet
   * Note: This endpoint requires authorization
   * @returns An Observable of Blob containing the Excel file
   */
  downloadRsvpReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/download`, {
      responseType: 'blob',
      observe: 'body'
    });
  }
}