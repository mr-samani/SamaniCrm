/* tslint:disable */
/* eslint-disable */

/**
 * api result
 */
export class ApiResult<T> {
  result!: T;

  /**
   * description of api result
   */
  message!: string;

  /**
   * success or failed: boolean
   */
  success!: boolean;

  links?: Links;
  meta?: Meta;
}

export interface Links {
  first: string;
  last: string;
  prev: null;
  next: string;
}

export interface Meta {
  current_page: number;
  from: number;
  last_page: number;
  links: Link[];
  path: string;
  per_page: number;
  to: number;
  total: number;
}

export interface Link {
  url: null | string;
  label: string;
  active: boolean;
}
