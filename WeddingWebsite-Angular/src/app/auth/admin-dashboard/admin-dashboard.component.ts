import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AdminAuthService, AdminUser } from '../../services/admin-auth.service';
import { GuestService } from '../../services/guest.service';
import { RsvpService } from '../../services/rsvp.service';
import { Guest } from '../../models/guest';
import { DashboardModel } from '../../models/dashboard-model';
import { EventLogModel } from '../../models/event-log-model';
import { EventLogService } from '../../services/event-log.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  currentUser: AdminUser | null = null;

  dashboard: DashboardModel | null = null;
  eventLogs: EventLogModel[] = [];

  allGuests: Guest[] = [];

  totalGuestCount: number = 0;
  attendingCount: number = 0;
  declinedCount: number = 0;
  isDownloading: boolean = false;

  constructor(
    private adminAuthService: AdminAuthService,
    private router: Router,
    private guestService: GuestService,
    private eventLogService: EventLogService,
    private rsvpService: RsvpService
  ) {

  }

  ngOnInit(): void {
    this.guestService.getDashboard().subscribe(dashboard => {
      this.dashboard = dashboard;
    });

    this.eventLogService.getLogs().subscribe(logs => {
      this.eventLogs = logs;
    });
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

  /**
   * Downloads the RSVP report as an Excel spreadsheet
   */
  downloadRsvpReport(): void {
    this.isDownloading = true;
    
    this.rsvpService.downloadRsvpReport().subscribe({
      next: (blob: Blob) => {
        // Create a URL for the blob
        const url = window.URL.createObjectURL(blob);
        
        // Create a temporary link element and trigger download
        const link = document.createElement('a');
        link.href = url;
        link.download = `GuestList_${new Date().toLocaleDateString('en-GB').replace(/\//g, '-')}.xlsx`;
        document.body.appendChild(link);
        link.click();
        
        // Clean up
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        
        this.isDownloading = false;
      },
      error: (error) => {
        console.error('Failed to download RSVP report', error);
        this.isDownloading = false;
        // You could add a toast notification or error message here
        alert('Failed to download the RSVP report. Please try again.');
      }
    });
  }
}