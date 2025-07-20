import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class GuestAuthService {
  private readonly PASSWORD = 'WEDDING123';
  private readonly STORAGE_KEY = 'guest-auth';
  private isBrowser: boolean;

  constructor(
    private router: Router,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  isAuthenticated(): boolean {
    if (this.isBrowser) {
      return localStorage.getItem(this.STORAGE_KEY) === 'true';
    }
    return false;
  }

  login(password: string): boolean {
    if (password === this.PASSWORD) {
      if (this.isBrowser) {
        localStorage.setItem(this.STORAGE_KEY, 'true');
      }
      return true;
    }
    return false;
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.STORAGE_KEY);
    }
    this.router.navigate(['/guest-login']);
  }
}