/* tslint:disable */

import { CaptchaDto } from './captcha-dto';

/* eslint-disable */
export interface RegisterRequest {
  captcha?: CaptchaDto;
  email: string;
  name: string;
  password: string;
}
