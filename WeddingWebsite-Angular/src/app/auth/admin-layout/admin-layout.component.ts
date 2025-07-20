import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminAuthService, AdminUser } from '../../services/admin-auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent implements OnInit{
  currentUser: AdminUser | null = null;

  constructor(
    private adminAuthService: AdminAuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.adminAuthService.currentUser;
  }

  logout(): void {
    this.adminAuthService.logout().subscribe({
      next: () => {
        this.router.navigate(['/admin/login']);
      },
      error: error => {
        console.error('Logout failed', error);
      }
    });
  }
}
