import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IPaginationLabel } from '@shared/components/pagination/labels';

@Injectable({
  providedIn: 'root',
})
export class PaginationLocalize {
  labels?: IPaginationLabel;
  constructor(private translate: TranslateService) {
    this.getLabels();
  }

  getLabels() {
    this.translate
      .get([
        'Pagination.Previous',
        'Pagination.Next',
        'Pagination.Showing',
        'Pagination.To',
        'Pagination.Of',
        'Pagination.Results',
        'Pagination.First',
        'Pagination.Last',
        'Pagination.PerPage',
      ])
      .subscribe((translations) => {
        this.labels = {
          previous: translations['Pagination.Previous'],
          next: translations['Pagination.Next'],
          showing: translations['Pagination.Showing'],
          to: translations['Pagination.To'],
          of: translations['Pagination.Of'],
          results: translations['Pagination.Results'],
          first: translations['Pagination.First'],
          last: translations['Pagination.Last'],
          perPage: translations['Pagination.PerPage'],
        };
      });
  }
}
