import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RsvpService, GuestRsvpUpdate } from '../services/rsvp.service';
import { Invite } from '../models/invite';
import { Guest } from '../models/guest';

@Component({
  selector: 'app-rsvp',
  imports: [CommonModule, FormsModule],
  templateUrl: './rsvp.component.html',
  styleUrl: './rsvp.component.scss'
})
export class RsvpComponent {
  // Step tracking
  currentStep: 'welcome' | 'rsvp' | 'thankyou' = 'welcome';
  
  // Welcome step data
  inviteCode: string = '';
  loading = false;
  error: string | null = null;

  // RSVP step data
  invite: Invite | null = null;
  guests: Guest[] = [];
  guestRsvps: GuestRsvpUpdate[] = [];
  submitting = false;

  constructor(private rsvpService: RsvpService, private router: Router) { }
  /**
   * Get invite details using the invite code
   */
  getInvite(): void {
    if (!this.inviteCode.trim()) {
      this.error = 'Please enter an invite code';
      return;
    }

    this.loading = true;
    this.error = null;

    this.rsvpService.getInviteByCode(this.inviteCode.trim()).subscribe({
      next: (invite) => {
        this.invite = invite;
        this.guests = invite.guests.$values;
        this.initializeGuestRsvps();
        this.currentStep = 'rsvp';
        this.loading = false;
      }, error: (err) => {
        console.error('Failed to load invite', err);

        if (this.rsvpService.isRsvpAlreadyCompletedError(err)) {
          this.error = this.rsvpService.getErrorMessage(err);
        } else if (err.status === 404) {
          this.error = 'Please check your invite code and try again.';
        } else {
          this.error = 'Unable to load invite. Please try again later.';
        }

        this.loading = false;
      }
    });
  }
  /**
   * Initialize guest RSVP data structure
   */
  private initializeGuestRsvps(): void {
    this.guestRsvps = this.guests.map(guest => ({
      id: guest.id,
      attending: guest.attending ?? null,
      dietaryRequirements: guest.dietaryRequirements || ''
    }));
  }  /**
   * Submit the RSVP responses
   */
  submitRsvp(): void {
    if (!this.invite) return;

    // Validate that all guests have made an attendance selection
    const guestsWithoutSelection = this.guestRsvps.filter(rsvp => rsvp.attending === null);
    if (guestsWithoutSelection.length > 0) {
      this.error = 'Please indicate attendance for all guests before submitting.';
      return;
    }

    this.submitting = true;
    this.error = null;

    this.rsvpService.updateInviteRsvp(this.invite, this.guestRsvps).subscribe({
      next: () => {
        this.currentStep = 'thankyou';
        this.submitting = false;
      },
      error: (err) => {
        console.error('RSVP submission failed', err);

        if (this.rsvpService.isRsvpAlreadyCompletedError(err)) {
          this.error = this.rsvpService.getErrorMessage(err);
        } else if (err.status === 401) {
          this.error = 'Invalid invite code. Please check and try again.';
        } else {
          this.error = 'Failed to submit RSVP. Please try again.';
        }

        this.submitting = false;
      }
    });
  }

  goToInfo(): void {
    // Go to the FAQ page
    this.router.navigate(['/info']);
  }

  goToFaq(): void {
    // Go to the FAQ page
    this.router.navigate(['/faq']);
  }

  /**
   * Go back to the invite code step
   */
  goBack(): void {
    this.currentStep = 'welcome';
    this.error = null;
  }
}
