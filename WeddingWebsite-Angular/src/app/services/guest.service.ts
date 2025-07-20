import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiUrl } from '../constants';
import { Guest } from '../models/guest';
import { JsonPreserveWrapper } from '../models/json-preserve-wrapper';
import { DashboardModel } from '../models/dashboard-model';


@Injectable({
  providedIn: 'root'
})
export class GuestService {

  private apiUrl = `${ApiUrl}/guest`;

  constructor(private http: HttpClient) {

  }

  /**
   * Fetch a list of all guests from the API
   * @returns An Observable of Guest array
   */
  list(): Observable<Guest[]> {
    // This matches the ListGuests() endpoint in your controller
    return this.http.get<JsonPreserveWrapper<Guest>>(this.apiUrl).pipe(
      map(res => res.$values)
    );
  }

  /**
   * Get a single guest by ID
   * @param id The guest ID
   * @returns An Observable of the Guest object
   */
  getById(id: number): Observable<Guest> {
    // This matches the GetGuest(int id) endpoint in your controller
    return this.http.get<Guest>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create a new guest
   * @param guest The guest object to create
   * @returns An Observable of the created Guest object
   */
  create(guest: Guest): Observable<Guest> {
    // This matches the CreateGuest(Guest guest) endpoint in your controller
    return this.http.post<Guest>(this.apiUrl, guest);
  }

  /**
   * Update an existing guest
   * @param guest The guest object to update
   * @returns An Observable of void
   */
  update(guest: Guest): Observable<void> {
    // This matches the UpdateGuest(int id, Guest updatedGuest) endpoint in your controller
    return this.http.put<void>(`${this.apiUrl}/${guest.id}`, guest);
  }

  /**
   * Delete a guest by ID
   * @param id The ID of the guest to delete
   * @returns An Observable of void
   */
  delete(id: number): Observable<void> {
    // This matches the DeleteGuest(int id) endpoint in your controller
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAvailableGuests(): Observable<Guest[]> {
    return this.http.get<JsonPreserveWrapper<Guest>>(`${this.apiUrl}/available`).pipe(
      map(res => res.$values)
    );
  }

  getDashboard(): Observable<DashboardModel> {
    return this.http.get<DashboardModel>(`${this.apiUrl}/dashboard`);
  }
}