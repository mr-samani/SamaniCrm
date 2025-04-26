import { Injectable } from '@angular/core';
import { CookieService } from './cookie.service';
import { DateTime } from 'luxon';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  tokenName = 'access_token';
  _token = '';
  constructor(private cookieService: CookieService) {}
  saveToken(token: string, rememberMe?: boolean) {
    this._token = token;
    let exp = rememberMe ? 365 : undefined;
    this.cookieService.set(this.tokenName, token, exp);
    this.cookieService.set('rememberMe', rememberMe + '', exp);
  }

  /***
   * called from refresh token
   */
  updateToken(token: string) {
    this._token = token;
    let rememberMe = this.cookieService.get('rememberMe') === 'true';
    let exp = rememberMe ? 365 : undefined;
    this.cookieService.set(this.tokenName, token, exp);
  }
  getToken(): string {
    if (this._token) {
      return this._token;
    }
    return this.cookieService.get(this.tokenName);
  }

  removeTokens() {
    this.cookieService.delete(this.tokenName);
    sessionStorage.removeItem(this.tokenName);
    sessionStorage.removeItem('rememberMe');
  }
}
