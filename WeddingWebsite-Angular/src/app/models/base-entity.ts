export interface BaseEntity {
  id: number;
  rowVersion: string; // Equivalent of byte[] in C#
}