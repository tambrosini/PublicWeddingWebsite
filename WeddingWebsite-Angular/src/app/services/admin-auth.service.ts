import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, tap, throwError, of } from 'rxjs';
import { ApiUrl } from '../constants';

export interface AdminUser {
  id: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface InfoResponse {
  email: string;
  isEmailConfirmed: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AdminAuthService {
  private currentAdminUserSubject = new BehaviorSubject<boolean | null>(null);
  public currentAdminLoggedIn$ = this.currentAdminUserSubject.asObservable();
  currentUser : AdminUser| null = null; 

  constructor(private http: HttpClient) {
    // Check auth status when service is initialized
    this.checkAuthStatus().subscribe();
  }

  public get currentAdminUserValue(): boolean | null {
    return this.currentAdminUserSubject.value;
  }

  login(email: string, password: string): Observable<AdminUser> {
    return this.http.post<AdminUser>(`${ApiUrl}/login?useCookies=true&useSessionCookies=true`, { email, password })
      .pipe(
        tap(user => {
          this.currentUser = user;
          this.currentAdminUserSubject.next(true);
          return user;
        }),
        catchError(error => {
          console.error('Login failed:', error);
          return throwError(() => error);
        })
      );
  }

  logout(): Observable<any> {
    // Make sure this endpoint path is correct
    return this.http.post(`${ApiUrl}/logout`, {})
      .pipe(
        tap(() => {
          this.currentUser = null;
          this.currentAdminUserSubject.next(false);
        }),
        catchError(error => {
          console.error('Logout failed:', error);
          return throwError(() => error);
        })
      );
  }

  checkAuthStatus(): Observable<InfoResponse | null> {
    return this.http.get<InfoResponse>(`${ApiUrl}/manage/info`)
      .pipe(
        tap(user => {
          this.currentAdminUserSubject.next(true);
          return user;
        }),
        catchError(error => {
          console.error('Auth check failed:', error);
          this.currentAdminUserSubject.next(false);
          // Return a null result instead of throwing an error
          return of(null);
        })
      );
  }

  isAuthenticated(): boolean {
    return !!this.currentAdminUserValue;
  }
}