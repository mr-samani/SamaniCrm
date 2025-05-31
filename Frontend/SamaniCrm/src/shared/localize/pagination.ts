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
    // this.labels = {
    //   previous: this.translate.instant('Pagination.Previous'),
    //   next: this.translate.instant('Pagination.Next'),
    //   showing: this.translate.instant('Pagination.Showing'),
    //   to: this.translate.instant('Pagination.To'),
    //   of: this.translate.instant('Pagination.Of'),
    //   results: this.translate.instant('Pagination.Results'),
    //   first: this.translate.instant('Pagination.First'),
    //   last: this.translate.instant('Pagination.Last'),
    //   perPage: this.translate.instant('Pagination.PerPage'),
    // };
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
