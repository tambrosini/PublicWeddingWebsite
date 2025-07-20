import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Guest } from '../../../models/guest';
import { GuestService } from '../../../services/guest.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterModule } from '@angular/router';

@Component({
  selector: 'app-guest-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule, RouterLink, RouterLinkActive],
  templateUrl: './guest-create.component.html',
  styleUrl: './guest-create.component.scss'
})
export class GuestCreateComponent implements OnInit {
  guestForm: FormGroup;
  isSubmitting = false;
  error: string | null = null;
  
  formErrors: any = {
    firstName: '',
    lastName: ''
  };
  
  constructor(
    private fb: FormBuilder,
    private guestService: GuestService,
    private router: Router
  ) {
    this.guestForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      dietaryRequirements: [''],
      attending: [false]
    });
  }

  ngOnInit(): void {   
    this.guestForm.valueChanges.subscribe(() => {
      this.updateFormErrors();
    });
  }

  updateFormErrors(): void {
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
    if (this.guestForm.invalid || this.isSubmitting) {
      return;
    }
    
    this.isSubmitting = true;
    const guestData: Guest = {
      ...this.guestForm.value,
      id: 0,
      rowVersion: ""
    };
    
    this.guestService.create(guestData).subscribe({
      next: (createdGuest) => {
        this.router.navigate(['/admin/guests', createdGuest.id]);
      },
      error: (error) => {
        console.error('Error creating guest', error);
        this.error = 'Error creating guest. Please try again.';
        this.isSubmitting = false;
      }
    });
  }
}
