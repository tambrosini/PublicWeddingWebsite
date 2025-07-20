import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule, RouterLink, RouterLinkActive, ActivatedRoute, Router } from '@angular/router';
import { Invite } from '../../../models/invite';
import { InviteService } from '../../../services/invite.service';
import { Guest } from '../../../models/guest';
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-invite-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink, RouterLinkActive],
  templateUrl: './invite-detail.component.html',
  styleUrl: './invite-detail.component.scss'
})

export class InviteDetailComponent implements OnInit {
  invite: Invite | null = null;
  guests: Guest[] = [];
  loading = true;
  error: string | null = null;
  showDeleteConfirm = false;
  copySuccess = false;

  constructor(
    private inviteService: InviteService,
    private route: ActivatedRoute,
    private router: Router,
    private clipboard: Clipboard
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadInvite(+id);
      } else {
        this.error = 'No invite ID provided';
        this.loading = false;
      }
    });
  }

  loadInvite(id: number): void {
    this.loading = true;
    this.error = null;

    this.inviteService.getInviteById(id).subscribe({
      next: (invite) => {
        this.invite = invite;
        this.guests = invite.guests.$values;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading invite', error);
        this.error = 'Error loading invite details. Please try again.';
        this.loading = false;
      }
    });
  }

  copyToClipboard(text: string): void {
    this.clipboard.copy(text);

    // Show quick feedback that copy succeeded
    this.copySuccess = true;
    setTimeout(() => {
      this.copySuccess = false;
    }, 2000);
  }

  confirmDelete(): void {
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void {
    this.showDeleteConfirm = false;
  }

  deleteInvite(): void {
    if (!this.invite) return;

    this.inviteService.deleteInvite(this.invite.id).subscribe({
      next: () => {
        this.router.navigate(['/admin/invites']);
      },
      error: (error) => {
        console.error('Error deleting invite', error);
        this.error = 'Error deleting invite. Please try again.';
        this.showDeleteConfirm = false;
      }
    });
  }

}