// src/app/auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { GuestAuthService } from './services/guest-auth.service';

@Injectable({
  providedIn: 'root',
})
export class GuestAuthGuard implements CanActivate {
  constructor(private guestAuthService: GuestAuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.guestAuthService.isAuthenticated()) {
      return true;
    } else {
      this.router.navigate(['/guest-login']);
      return false;
    }
  }
}
