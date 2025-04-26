export class FieldsType {
  title!: string;
  column!: string;
  type?: 'text' | 'image' | 'avatar' | 'date' | 'time' | 'dateTime' | undefined;
  width?: number;
  wrap?: boolean;
}
