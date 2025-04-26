export interface IPaginationLabel {
  first: string;
  last: string;
  next: string;
  previous: string;
  showing: string;
  to: string;
  of: string;
  results: string;
  perPage: string;
}

export class PaginationLabel {
  first = 'First';
  last = 'Last';
  next = 'Next';
  previous = 'Previous';
  showing = 'Showing';
  to = 'to';
  of = 'of';
  results = 'results';
  perPage = 'Per page';
  constructor(data?: PaginationLabel) {}
}
