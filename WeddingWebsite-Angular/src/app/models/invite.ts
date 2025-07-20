import { BaseEntity } from './base-entity';
import { Guest } from './guest';
import { JsonPreserveWrapper } from './json-preserve-wrapper';

export interface Invite extends BaseEntity {
  name: string;
  guests: JsonPreserveWrapper<Guest>;
  publicCode: string;
  rsvpCompleted: boolean;
}
