import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { ApiUrl } from '../constants';
import { Invite } from '../models/invite';
import { JsonPreserveWrapper } from '../models/json-preserve-wrapper';
import { UpdateInviteDto } from '../models/dto/update-invite';

@Injectable({
  providedIn: 'root'
})
export class InviteService {
  private apiUrl = `${ApiUrl}/invite`;

  constructor(private http: HttpClient) {}

  /**
   * Fetch a list of all invites
   * @returns An Observable of Invite array
   */
  getAllInvites(): Observable<Invite[]> {
    return this.http.get<JsonPreserveWrapper<Invite>>(this.apiUrl).pipe(
      map(res => res.$values)
    );
  }

  /**
   * Get a single invite by ID
   * @param id The invite ID
   * @returns An Observable of the Invite object
   */
  getInviteById(id: number): Observable<Invite> {
    return this.http.get<Invite>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new invite
   * @param invite The invite object to create
   * @returns An Observable of the created Invite object
   */
  createInvite(invite: Invite): Observable<Invite> {
    return this.http.post<Invite>(this.apiUrl, invite);
  }

  /**
   * Delete an invite by ID
   * @param id The ID of the invite to delete
   * @returns An Observable of void
   */
  deleteInvite(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Update an existing invite
   * @param updateDto The invite update data
   * @returns An Observable of the updated Invite object
   */
  updateInvite(updateDto: UpdateInviteDto): Observable<Invite> {
    return this.http.put<Invite>(`${this.apiUrl}/${updateDto.id}`, updateDto);
  }

  /**
   * Admin function to manually RSVP to an invite
   * @param updateDto The invite update data with guest RSVP information
   * @returns An Observable of the updated Invite object
   */
  adminRsvpToInvite(updateDto: UpdateInviteDto): Observable<Invite> {
    return this.http.put<Invite>(`${this.apiUrl}/admin-rsvp/${updateDto.id}`, updateDto);
  }
}