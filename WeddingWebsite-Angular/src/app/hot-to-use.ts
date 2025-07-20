// In your component
constructor(private guestService: GuestService) {}

ngOnInit() {
  // Get all guests
  this.guestService.list().subscribe(guests => {
    this.guests = guests;
  });
}

// Create a new guest
createGuest(guest: Guest): void {
  this.guestService.create(guest).subscribe(createdGuest => {
    console.log('Guest created:', createdGuest);
    // Refresh list or navigate
  });
}

// Update a guest
updateGuest(guest: Guest): void {
  this.guestService.update(guest).subscribe(() => {
    console.log('Guest updated successfully');
    // Refresh or show success message
  });
}

// Delete a guest
deleteGuest(id: number): void {
  this.guestService.delete(id).subscribe(() => {
    console.log('Guest deleted successfully');
    // Remove from local list or refresh
  });
}

// In your RSVP component
constructor(private rsvpService: RsvpService) {}

// Step 1: Get the invite details using the unique code
getInvite(inviteCode: string): void {
  const rsvpPassword = 'your-rsvp-password'; // This should be stored securely
  
  this.rsvpService.getInviteByCode(inviteCode, rsvpPassword).subscribe({
    next: (invite) => {
      this.invite = invite;
      this.guests = invite.guests;
    },
    error: (error) => {
      if (error.status === 401) {
        this.errorMessage = 'Invalid password';
      } else if (error.status === 404) {
        this.errorMessage = 'Invite not found';
      } else {
        this.errorMessage = 'An error occurred';
      }
    }
  });
}

// Step 2: Submit the RSVP responses
submitRsvp(): void {
  const rsvpPassword = 'your-rsvp-password';
  
  // Create an array of guest updates
  const guestUpdates: GuestRsvpUpdate[] = this.guests.map(guest => ({
    id: guest.id,
  attending: guest.attending,
    dietaryRequirements: guest.dietaryRequirements
  }));
  
  this.rsvpService.updateInviteRsvp(
    this.invite,
    guestUpdates
  ).subscribe({
    next: () => {
      this.successMessage = 'RSVP submitted successfully!';
    },
    error: (error) => {
      if (error.status === 401) {
        this.errorMessage = 'Invalid invite code or password';
      } else {
        this.errorMessage = 'An error occurred';
      }
    }
  });
}