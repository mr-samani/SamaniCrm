export class FieldsType {
  title!: string;
  column!: string;
  type?:
    | 'text'
    | 'image'
    | 'profilePicture'
    | 'date'
    | 'time'
    | 'dateTime'
    | 'yesNo'
    | 'number'
    | 'localize'
    | 'template'
    | undefined;
  width?: number;
  wrap?: boolean;
  localizeKey?: string;
}

export class SortEvent {
  field: string = '';
  direction: 'asc' | 'desc' = 'asc';
}
