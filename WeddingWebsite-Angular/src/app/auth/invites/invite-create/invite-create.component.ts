import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { InviteService } from '../../../services/invite.service';
import { Invite } from '../../../models/invite';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';

@Component({
  selector: 'app-invite-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, RouterLink, RouterLinkActive, ReactiveFormsModule],
  templateUrl: './invite-create.component.html',
  styleUrl: './invite-create.component.scss'
})
export class InviteCreateComponent {
  inviteForm: FormGroup;
  isSubmitting = false;
  error: string | null = null;
  loading = false;

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
    private router: Router
  ) {
    this.inviteForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.inviteForm.valueChanges.subscribe(() => {
      this.updateFormErrors();
    });

    this.loadAvailableGuests();
  }

  loadAvailableGuests(): void {
    this.loading = true;
    this.guestService.getAvailableGuests().subscribe({
      next: (guests) => {
        this.availableGuests = guests;
        this.filterAvailableGuests();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading available guests', error);
        this.error = 'Error loading guests. Please try again.';
        this.loading = false;
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
    const inviteData: Invite = {
      ...this.inviteForm.value,
      id: 0,
      rowVersion: "",
      publicCode: "",
      guestIds: this.selectedGuests.map(guest => guest.id)
    };

    this.inviteService.createInvite(inviteData).subscribe({
      next: (createdInvite) => {
        this.router.navigate(['/admin/invites', createdInvite.id]);
      },
      error: (error) => {
        console.error('Error creating invite', error);
        this.error = 'Error creating invite. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

}
