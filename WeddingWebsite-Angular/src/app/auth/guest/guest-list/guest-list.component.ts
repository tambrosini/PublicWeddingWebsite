import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterModule } from '@angular/router';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-guest-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './guest-list.component.html',
  styleUrl: './guest-list.component.scss'
})

export class GuestListComponent implements OnInit {
  guests: Guest[] = [];
  filteredGuests: Guest[] = [];
  searchTerm: string = '';
  
  constructor(private guestService: GuestService) {}

  ngOnInit(): void {
    this.loadGuests();
  }

  loadGuests(): void {
    this.guestService.list().subscribe({
      next: (data) => {
        this.guests = data;
        this.filteredGuests = data;
      },
      error: (error) => {
        console.error('Error loading guests', error);
      }
    });
  }

  filterGuests(): void {
    const term = this.searchTerm.toLowerCase();
    
    if (!term) {
      this.filteredGuests = this.guests;
      return;
    }
    
    this.filteredGuests = this.guests.filter(guest => 
      guest.firstName.toLowerCase().includes(term) ||
      guest.lastName.toLowerCase().includes(term) ||
      (guest.dietaryRequirements && guest.dietaryRequirements.toLowerCase().includes(term))
    );
  }

  deleteGuest(id: number): void {
    if (confirm('Are you sure you want to delete this guest?')) {
      this.guestService.delete(id).subscribe({
        next: () => {
          this.guests = this.guests.filter(guest => guest.id !== id);
          this.filteredGuests = this.filteredGuests.filter(guest => guest.id !== id);
        },
        error: (error) => {
          console.error('Error deleting guest', error);
        }
      });
    }
  }
}
