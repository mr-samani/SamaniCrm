import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BreadcrumbService {
  list: {
    name: string;
    url?: string;
    queryParams?: { [key: string]: string };
  }[] = [];
  constructor() {}
}
