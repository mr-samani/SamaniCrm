import { Injectable } from '@angular/core';
import { PageDto } from '@shared/service-proxies';

@Injectable({
  providedIn: 'root',
})
export class SharedPageDataService {
  pageInfo?: PageDto;
}
