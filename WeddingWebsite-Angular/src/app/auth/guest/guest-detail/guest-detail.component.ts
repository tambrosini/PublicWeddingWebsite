import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterModule } from '@angular/router';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';

@Component({
  selector: 'app-guest-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink, RouterLinkActive],
  templateUrl: './guest-detail.component.html',
  styleUrl: './guest-detail.component.scss'
})

export class GuestDetailComponent implements OnInit {
  guest: Guest | null = null;
  error: string | null = null;
  
  constructor(
    private guestService: GuestService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadGuest(id);
    } else {
      this.error = 'Invalid guest ID';
    }
  }

  loadGuest(id: number): void {
    this.guestService.getById(id).subscribe({
      next: (data) => {
        this.guest = data;
      },
      error: (error) => {
        console.error('Error loading guest', error);
        this.error = 'Error loading guest details. Please try again.';
      }
    });
  }

  deleteGuest(): void {
    if (!this.guest) return;
    
    if (confirm('Are you sure you want to delete this guest?')) {
      this.guestService.delete(this.guest.id).subscribe({
        next: () => {
          this.router.navigate(['/admin/guests']);
        },
        error: (error) => {
          console.error('Error deleting guest', error);
          this.error = 'Error deleting guest. Please try again.';
        }
      });
    }
  }
}
