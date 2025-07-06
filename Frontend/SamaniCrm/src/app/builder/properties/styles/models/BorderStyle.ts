export interface BorderStyle {
  forAll: boolean;
  border?: Border;
  borderTop?: Border;
  borderRight?: Border;
  borderBottom?: Border;
  borderLeft?: Border;
}

export interface Border {
  prefix: 'px' | '%' | 'rem' | 'em';
  width?: number;
  type: BorderType;
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
