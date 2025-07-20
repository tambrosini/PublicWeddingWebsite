import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AdminAuthService } from '../services/admin-auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminAuthGuard implements CanActivate {
  constructor(
    private adminAuthService: AdminAuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (this.adminAuthService.isAuthenticated()) {
      return true;
    }

    // If not logged in, try to check auth status first
    return this.adminAuthService.checkAuthStatus().pipe(
      map(user => {
        if (user) {
          return true;
        }
        this.router.navigate(['/admin/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }),
      catchError(() => {
        this.router.navigate(['/admin/login'], { queryParams: { returnUrl: state.url } });
        return of(false);
      })
    );
  }
}