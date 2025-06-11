import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MainSpinnerService {
  constructor(@Inject(DOCUMENT) private _document: Document) {}
  public set showLoading(val: boolean) {
    const spinner: HTMLDivElement | null = this._document.querySelector('.main-spinner-container');
    if (!spinner) return;
    if (val) {
      spinner.style.display = 'flex';
    } else {
      spinner.style.display = 'none';
    }
  }
}
