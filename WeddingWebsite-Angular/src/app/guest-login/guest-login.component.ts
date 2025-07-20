import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { GuestAuthService } from '../services/guest-auth.service';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-guest-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './guest-login.component.html',
  styleUrl: './guest-login.component.scss'
})
export class GuestLoginComponent {

  password = '';
  errorMessage = '';

  constructor(
    private guestAuthService: GuestAuthService,
    private router: Router
  ){}

  login() {
    if (this.guestAuthService.login(this.password))
    {
      this.router.navigate(['']);
    } 
    else {
      this.errorMessage = 'Incorrect Password';
    }
  }
}
