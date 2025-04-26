/* tslint:disable */

import { CaptchaDto } from './captcha-dto';

/* eslint-disable */
export interface LoginRequest {
  captcha?: CaptchaDto;
  email: string;
  password: string;

  rememberMe?: boolean;
}
