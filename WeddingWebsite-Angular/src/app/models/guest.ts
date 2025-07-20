import { BaseEntity } from './base-entity';
import { Invite } from './invite';

export interface Guest extends BaseEntity {
  firstName: string;
  lastName: string;
  dietaryRequirements?: string;
  attending?: boolean | null;
  inviteId?: number;
  invite?: Invite | null;
}