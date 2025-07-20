import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { RsvpService } from './services/rsvp.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'WeddingWebsite';

  constructor(private rsvpService: RsvpService) {}
  ngOnInit(): void {
    // Wake up the database when the app loads - fire and forget
    this.rsvpService.wakeUpDatabase().subscribe();
  }
}
