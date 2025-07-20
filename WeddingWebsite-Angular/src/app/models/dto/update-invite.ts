import { GuestRsvp } from './guest-rsvp';

export interface UpdateInviteDto {
  id: number;
  name: string;

  /** List of guest IDs to link to the invite */
  guestIds?: number[];

  /** Only populated if manually RSVPing for guest */
  guestRsvps?: GuestRsvp[];
}
