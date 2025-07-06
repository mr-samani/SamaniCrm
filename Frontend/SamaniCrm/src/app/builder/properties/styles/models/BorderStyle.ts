export interface BorderStyle {
  forAll: boolean;
  border?: Border;
  borderTop?: Border;
  borderRight?: Border;
  borderBottom?: Border;
  borderLeft?: Border;
}

export interface Border {
  prefix?: SizePrefix;
  width?: number;
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

export declare type SizePrefix = 'px' | '%' | 'rem' | 'em';
export const SizePrefixList: readonly SizePrefix[] = ['px', '%', 'rem', 'em'] as const;
