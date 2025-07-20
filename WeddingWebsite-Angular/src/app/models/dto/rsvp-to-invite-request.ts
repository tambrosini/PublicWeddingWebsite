import { GuestRsvp } from './guest-rsvp';

export interface RsvpToInviteRequest {
  inviteId: number;
  inviteUniqueCode?: string;
  rsvpPassword?: string;
  guestRsvps: GuestRsvp[];
}