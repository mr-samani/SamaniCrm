import { isPlatformBrowser } from '@angular/common';
import { inject, Injectable, PLATFORM_ID } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StoreService {
  platformId = inject(PLATFORM_ID);

  public getItem<T>(key: string): T | undefined {
    if (isPlatformBrowser(this.platformId) == false) return;
    return <T>localStorage.getItem(key);
  }

  public setItem<T = any>(key: string, data: T) {
    if (isPlatformBrowser(this.platformId) == false) return;
    try {
      let value = '';
      if (typeof data === 'object') value = JSON.stringify(data);
      else value = data as string;
      localStorage.setItem(key, value);
    } catch (error) {
      console.error('Error on set data to localstorage:', error);
    }
  }

  public removeItem(key: string) {
    if (isPlatformBrowser(this.platformId) == false) return;
    localStorage.removeItem(key);
  }
}
