import { Injectable } from '@angular/core';
export class TokenModel {
  accessToken?: string;
  refreshToken?: string;
}
@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private accessTokenName = 'access_token';
  private refreshTokenName = 'refresh_token';
  private basicTokenName = 'auth';
  constructor() {}

  /**
   * store token on client
   * @param accessToken  access token
   * @param refreshToken refresh token
   * @param expire expire day
   */
  set(token: TokenModel) {
    localStorage.setItem(this.accessTokenName, token.accessToken ?? '');
    localStorage.setItem(this.refreshTokenName, token.refreshToken ?? '');
  }

  get(): TokenModel {
    const token = {
      accessToken: localStorage.getItem(this.accessTokenName) || '',
      refreshToken: localStorage.getItem(this.refreshTokenName) || '',
    };
    return token;
  }

  remove(): Promise<void> {
    return new Promise((resolve, reject) => {
      localStorage.removeItem(this.accessTokenName);
      localStorage.removeItem(this.refreshTokenName);
      localStorage.removeItem(this.basicTokenName);
      sessionStorage.removeItem(this.accessTokenName);
      sessionStorage.removeItem(this.refreshTokenName);
      sessionStorage.removeItem(this.basicTokenName);
      localStorage.removeItem('rememberMe');

      setTimeout(() => {
        resolve();
      }, 500);
    });
  }
}
