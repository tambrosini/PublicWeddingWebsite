import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule, RouterLink, RouterLinkActive, ActivatedRoute, Router } from '@angular/router';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';
import { InviteService } from '../../../services/invite.service';
import { Invite } from '../../../models/invite';
import { UpdateInviteDto } from '../../../models/dto/update-invite';

@Component({
  selector: 'app-invite-edit',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, RouterLink, RouterLinkActive, ReactiveFormsModule],
  templateUrl: './invite-edit.component.html',
  styleUrl: './invite-edit.component.scss'
})
export class InviteEditComponent implements OnInit {

  inviteId: number = 0;
  inviteForm: FormGroup;
  isSubmitting = false;
  loading = false;
  loadingGuests = false;
  error: string | null = null;

  // Guest selection
  availableGuests: Guest[] = [];
  filteredAvailableGuests: Guest[] = [];
  selectedGuests: Guest[] = [];
  guestSearchTerm = '';

  formErrors: any = {
    name: ''
  };

  constructor(
    private fb: FormBuilder,
    private inviteService: InviteService,
    private guestService: GuestService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.inviteForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      this.inviteId = +id!;
      this.loadInvite(+id!);
    });

    this.inviteForm.valueChanges.subscribe(() => {
      this.updateFormErrors();
    });
  }

  loadInvite(id: number): void {
    this.loading = true;
    this.inviteService.getInviteById(id).subscribe({
      next: (invite) => {
        this.inviteForm.patchValue({
          name: invite.name,
          rsvpCompleted: invite.rsvpCompleted || false
        });

        // Initialize the selected guests
        if (invite.guests && invite.guests.$values.length > 0) {
          this.selectedGuests = [...invite.guests.$values];
        }

        this.loading = false;
        this.loadAvailableGuests();
      },
      error: (error) => {
        console.error('Error loading invite', error);
        this.error = 'Error loading invite details. Please try again.';
        this.loading = false;
      }
    });
  }

  loadAvailableGuests(): void {
    this.loadingGuests = true;
    this.guestService.getAvailableGuests().subscribe({
      next: (guests) => {
        // In edit mode, we need to add any guests that are already associated with this invite
        if (this.selectedGuests.length > 0) {
          // Filter out guests that are already selected
          const selectedGuestIds = this.selectedGuests.map(g => g.id);
          this.availableGuests = guests.filter(g => !selectedGuestIds.includes(g.id));
        } else {
          this.availableGuests = guests;
        }

        this.filterAvailableGuests();
        this.loadingGuests = false;
      },
      error: (error) => {
        console.error('Error loading available guests', error);
        this.error = 'Error loading guests. Please try again.';
        this.loadingGuests = false;
      }
    });
  }

  filterAvailableGuests(): void {
    if (!this.guestSearchTerm.trim()) {
      this.filteredAvailableGuests = [...this.availableGuests];
      return;
    }

    const searchTerm = this.guestSearchTerm.toLowerCase().trim();
    this.filteredAvailableGuests = this.availableGuests.filter(guest =>
      guest.firstName.toLowerCase().includes(searchTerm) ||
      guest.lastName.toLowerCase().includes(searchTerm)
    );
  }

  addGuest(guest: Guest): void {
    // Add to selected guests
    this.selectedGuests.push(guest);

    // Remove from available guests
    const index = this.availableGuests.findIndex(g => g.id === guest.id);
    if (index !== -1) {
      this.availableGuests.splice(index, 1);
      this.filterAvailableGuests();
    }
  }

  removeGuest(guest: Guest): void {
    // Remove from selected guests
    const index = this.selectedGuests.findIndex(g => g.id === guest.id);
    if (index !== -1) {
      this.selectedGuests.splice(index, 1);
    }

    // Add back to available guests
    this.availableGuests.push(guest);
    this.filterAvailableGuests();
  }

  updateFormErrors(): void {
    const form = this.inviteForm;

    for (const field in this.formErrors) {
      this.formErrors[field] = '';
      const control = form.get(field);

      if (control && control.dirty && !control.valid) {
        const messages: any = {
          'required': `${field} is required.`,
          'maxlength': `${field} cannot be more than 100 characters long.`
        };

        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  onSubmit(): void {
    if (this.inviteForm.invalid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    const formValue = this.inviteForm.value;
    const guestIds = this.selectedGuests.map(guest => guest.id);

    const updatedInviteData: UpdateInviteDto = {
      id: this.inviteId!,
      name: formValue.name,
      guestIds: guestIds
    };

    this.inviteService.updateInvite(updatedInviteData).subscribe({
      next: () => {
        this.router.navigate(['/admin/invites', this.inviteId]);
      },
      error: (error) => {
        console.error('Error updating invite', error);
        this.error = 'Error updating invite. Please try again.';
        this.isSubmitting = false;
      }
    });

  }
}
