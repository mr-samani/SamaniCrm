export class PageModel {
  id!: string;
  title!: null;
  description!: null;
  cover!: null;
  status!: number;
  active!: number;
  creator?: string;
  createdAt?: Date;
  updatedAt?: Date;

  loading?: boolean;
}
