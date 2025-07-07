import { SizeUnit } from './SizeUnit';

export interface BorderStyle {
  forAll: boolean;
  border?: Border;
  top?: Border;
  right?: Border;
  bottom?: Border;
  left?: Border;
}

export interface Border {
  unit?: SizeUnit;
  size?: number;
  type?: BorderType;
  color?: string;

  /** 'inset' | 'outset' */
  inset?: boolean;
}

export declare type BorderType = 'none' | 'hidden' | 'dotted' | 'dashed' | 'solid' | 'double' | 'groove' | 'ridge';
export const BorderTypeList: readonly BorderType[] = [
  'none',
  'hidden',
  'dotted',
  'dashed',
  'solid',
  'double',
  'groove',
  'ridge',
] as const;
