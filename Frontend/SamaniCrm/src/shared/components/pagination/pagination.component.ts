import { CommonModule } from '@angular/common';
import {
  AfterContentInit,
  AfterViewInit,
  Component,
  EventEmitter,
  Inject,
  Input,
  OnInit,
  Output,
  output,
} from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { InjectionToken } from '@angular/core';
import { IPaginationLabel, PaginationLabel } from './labels';
import { Observable } from 'rxjs/internal/Observable';
import { FormsModule } from '@angular/forms';

export const PAGINATION_LABELS = new InjectionToken<IPaginationLabel>('localize pagination label', {
  providedIn: 'root',
  factory: () => ({
    first: 'First',
    last: 'Last',
    next: 'Next',
    previous: 'Previous',
    showing: 'Showing',
    to: 'to',
    of: 'of',
    results: 'results',
    perPage: 'Per page',
  }),
});

export interface PageEvent {
  page: number;
  perPage: number;
}

@Component({
  selector: 'pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss'],
  standalone: true,
  imports: [CommonModule, TranslateModule, FormsModule],
})
export class PaginationComponent implements AfterViewInit {
  totalPages = 0;
  @Input() set total(val: number) {
    this.totalPages = val;
    this.init();
  }
  @Input() perPage = 10;
  @Output() perPageChange = new EventEmitter<number>();
  /** start with: 1 */
  @Input() page = 1;
  @Output() pageChange = new EventEmitter<number>();
  @Output() change = new EventEmitter<PageEvent>();
  paginationCount = 0;

  pages: any[] = [];
  firstItem = 0;
  lastItem = 0;

  perPageList = [5, 10, 20, 30, 50, 100, 500, 1000, 5000];
  labels = new PaginationLabel();
  constructor(@Inject(PAGINATION_LABELS) labels: PaginationLabel) {
    if (labels) {
      this.labels = labels;
    }
  }

  ngAfterViewInit(): void {
    this.init();
  }

  init() {
    this.setupPage();
  }

  setupPage() {
    this.paginationCount = Math.ceil(this.totalPages / this.perPage);

    if (this.page < 1 || this.page > this.paginationCount) {
      this.page = 1;
      this.pageChange.emit(this.page);
    }

    const pages: any[] = [];
    const delta = 2; // چند صفحه قبل و بعد از page نشان داده شود

    const left = Math.max(2, this.page - delta);
    const right = Math.min(this.paginationCount - 1, this.page + delta);

    // همیشه صفحه اول
    pages.push(1);

    // ... اگر فاصله زیاد بود
    if (left > 2) {
      pages.push('...');
    }

    // صفحات میانی
    for (let i = left; i <= right; i++) {
      pages.push(i);
    }

    // ... اگر فاصله زیاد بود
    if (right < this.paginationCount - 1) {
      pages.push('...');
    }

    // همیشه صفحه آخر
    if (this.paginationCount > 1) {
      pages.push(this.paginationCount);
    }

    this.pages = pages;

    this.firstItem = (this.page - 1) * this.perPage + 1;
    this.lastItem = Math.min(this.firstItem + this.perPage - 1, this.totalPages);
  }

  onPageChange(page: number) {
    this.page = page;
    this.setupPage();
    this.pageChange.emit(this.page);
    this.change.emit({
      page: this.page,
      perPage: this.perPage,
    });
  }

  onPerPageChange() {
    this.setupPage();
    if (this.paginationCount < this.page) {
      this.page = 1;
      this.pageChange.emit(this.page);
    }
    this.perPageChange.emit(this.perPage);
    this.change.emit({
      page: this.page,
      perPage: this.perPage,
    });
  }

  next() {
    if (this.page < this.paginationCount) {
      this.page++;
    }
    this.onPageChange(this.page);
  }

  previous() {
    if (this.page > 1) {
      this.page--;
    }
    this.onPageChange(this.page);
  }

  first() {
    this.page = 1;
    this.onPageChange(this.page);
  }
  last() {
    this.page = this.paginationCount;
    this.onPageChange(this.page);
  }
}
