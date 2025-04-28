export class FieldsType {
  title!: string;
  column!: string;
  type?: 'text' | 'image' | 'profilePicture' | 'date' | 'time' | 'dateTime' | undefined;
  width?: number;
  wrap?: boolean;
}
