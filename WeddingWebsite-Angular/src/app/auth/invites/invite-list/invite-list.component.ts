import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, RouterLink, RouterLinkActive } from '@angular/router';
import { Invite } from '../../../models/invite';
import { InviteService } from '../../../services/invite.service';
import { error } from 'console';

@Component({
  selector: 'app-invite-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './invite-list.component.html',
  styleUrl: './invite-list.component.scss'
})
export class InviteListComponent implements OnInit{

  invites: Invite[] = []
  filteredInvites: Invite[] = [];
  searchTerm: string = '';

  constructor(private inviteService: InviteService) {

  }

  ngOnInit(): void {
    this.loadInvites();
  }

  loadInvites(): void {
    this.inviteService.getAllInvites().subscribe({
      next: (data) => {
        this.invites = data;
        this.filteredInvites = data;
      },
      error: (error) => {
        console.error('Error loading invites', error);
      }
    });
  }

  filterInvites(): void {
    const term = this.searchTerm.toLowerCase();

    if(!term) {
      this.filteredInvites = this.invites;
      return; 
    }

    this.filteredInvites = this.invites.filter(invite => 
      invite.name.toLowerCase().includes(term)
    );
  }

  deleteInvite(id: number): void {
        if (confirm('Are you sure you want to delete this invite?')) {
      this.inviteService.deleteInvite(id).subscribe({
        next: () => {
          this.invites = this.invites.filter(guest => guest.id !== id);
          this.filteredInvites = this.filteredInvites.filter(invite => invite.id !== id);
        },
        error: (error) => {
          console.error('Error deleting invite', error);
        }
      });
    }
  }

}
