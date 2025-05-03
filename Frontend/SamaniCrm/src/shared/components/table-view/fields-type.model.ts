export class FieldsType {
  title!: string;
  column!: string;
  type?: 'text' | 'image' | 'profilePicture' | 'date' | 'time' | 'dateTime' | 'yesNo' | undefined;
  width?: number;
  wrap?: boolean;
}
