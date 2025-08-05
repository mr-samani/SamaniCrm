import { Injectable } from '@angular/core';
import { StoreService } from './localstore.service';
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
  constructor(private storeService: StoreService) {}

  /**
   * store token on client
   * @param accessToken  access token
   * @param refreshToken refresh token
   * @param expire expire day
   */
  set(token: TokenModel) {
    this.storeService.setItem(this.accessTokenName, token.accessToken ?? '');
    this.storeService.setItem(this.refreshTokenName, token.refreshToken ?? '');
  }

  get(): TokenModel {
    const token = {
      accessToken: this.storeService.getItem<string>(this.accessTokenName) || '',
      refreshToken: this.storeService.getItem<string>(this.refreshTokenName) || '',
    };
    return token;
  }

  remove(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.storeService.removeItem(this.accessTokenName);
      this.storeService.removeItem(this.refreshTokenName);
      this.storeService.removeItem(this.basicTokenName);
      sessionStorage.removeItem(this.accessTokenName);
      sessionStorage.removeItem(this.refreshTokenName);
      sessionStorage.removeItem(this.basicTokenName);
      this.storeService.removeItem('rememberMe');

      setTimeout(() => {
        resolve();
      }, 500);
    });
  }
}
