export class PaginationModel {
  perPage: number;
  page: number;
  constructor(page?: number, perPage?: number) {
    this.page = page ?? 1;
    this.perPage = perPage ?? 10;
  }
}
