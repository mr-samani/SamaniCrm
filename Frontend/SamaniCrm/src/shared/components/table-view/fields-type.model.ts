export class FieldsType {
  title!: string;
  column!: string;
  type?: 'text' | 'image' | 'profilePicture' | 'date' | 'time' | 'dateTime' | 'yesNo' | 'number' | undefined;
  width?: number;
  wrap?: boolean;
}

export class SortEvent {
  field: string = '';
  direction: 'asc' | 'desc' = 'asc';
}
