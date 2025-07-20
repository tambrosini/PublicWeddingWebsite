import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink, RouterModule } from '@angular/router';
import { InviteService } from '../../../services/invite.service';
import { Invite } from '../../../models/invite';
import { Guest } from '../../../models/guest';
import { GuestRsvp } from '../../../models/dto/guest-rsvp';
import { UpdateInviteDto } from '../../../models/dto/update-invite';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-rsvp',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink, FormsModule],
  templateUrl: './admin-rsvp.component.html',
  styleUrl: './admin-rsvp.component.scss'
})
export class AdminRsvpComponent implements OnInit {
  invite: Invite | null = null;
  guestRsvps: GuestRsvp[] = [];
  guests: Guest[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private inviteService: InviteService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadInvite(+id);
      } else {
        this.error = 'No invite ID provided.';
        this.loading = false;
      }
    });
  }

  loadInvite(id: number): void {
    this.loading = true;
    this.error = null;

    this.inviteService.getInviteById(id).subscribe({
      next: (invite) => {        this.invite = invite;
        this.guests = invite.guests.$values;
        this.guestRsvps = this.guests.map(g => ({
          guestId: g.id,
          attending: typeof g.attending === 'boolean' ? g.attending : false,
          dietaryRequirements: g.dietaryRequirements || ''
        }));

        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load invite', err);
        this.error = 'Failed to load invite data.';
        this.loading = false;
      }
    });
  }

  submitRsvp(): void {
    if (!this.invite) return;

    const dto: UpdateInviteDto = {
      id: this.invite.id,
      name: this.invite.name,
      guestRsvps: this.guestRsvps
    };

    this.inviteService.adminRsvpToInvite(dto).subscribe({
      next: () => {
        this.router.navigate(['/admin/invites', this.invite!.id]);
      },
      error: (err) => {
        console.error('RSVP submission failed', err);
        this.error = 'Failed to submit RSVP. Please try again.';
      }
    });
  }
}
