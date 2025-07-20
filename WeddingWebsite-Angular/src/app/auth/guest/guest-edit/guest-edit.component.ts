import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';

@Component({
  selector: 'app-guest-edit',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule, RouterLink, RouterLinkActive],
  templateUrl: './guest-edit.component.html',
  styleUrl: './guest-edit.component.scss'
})

export class GuestEditComponent implements OnInit {
  guestId: number;
  guest: Guest | null = null;
  guestForm: FormGroup | null = null;
  isSubmitting = false;
  error: string | null = null;
  
  formErrors: any = {
    firstName: '',
    lastName: ''
  };
  
  constructor(
    private fb: FormBuilder,
    private guestService: GuestService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.guestId = Number(this.route.snapshot.paramMap.get('id'));
  }

  ngOnInit(): void {
    this.loadGuest();
  }

  loadGuest(): void {
    if (!this.guestId) {
      this.error = 'Invalid guest ID';
      return;
    }

    this.guestService.getById(this.guestId).subscribe({
      next: (guest) => {
        this.guest = guest;
        this.initForm();
      },
      error: (error) => {
        console.error('Error loading guest', error);
        this.error = 'Error loading guest details. Please try again.';
      }
    });
  }

  initForm(): void {
    if (!this.guest) return;

    this.guestForm = this.fb.group({
      firstName: [this.guest.firstName, [Validators.required, Validators.maxLength(100)]],
      lastName: [this.guest.lastName, [Validators.required, Validators.maxLength(100)]],
      dietaryRequirements: [this.guest.dietaryRequirements || ''],
      attending: [this.guest.attending],
      inviteId: [this.guest.inviteId]
    });

    this.guestForm.valueChanges.subscribe(() => {
      this.updateFormErrors();
    });
  }

  updateFormErrors(): void {
    if (!this.guestForm) return;
    
    const form = this.guestForm;
    
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
    if (!this.guestForm || !this.guest || this.guestForm.invalid || this.isSubmitting) {
      return;
    }
    
    this.isSubmitting = true;
    const updatedGuest: Guest = {
      ...this.guest,
      ...this.guestForm.value
    };
    
    this.guestService.update(updatedGuest).subscribe({
      next: () => {
        this.router.navigate(['/admin/guests', this.guestId]);
      },
      error: (error) => {
        console.error('Error updating guest', error);
        this.error = 'Error updating guest. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}
