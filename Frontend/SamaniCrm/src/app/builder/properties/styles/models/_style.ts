import { BorderStyle } from './BorderStyle';

export class BlockStyle {
  border?: BorderStyle;
  borderRadius?: string;
  padding?: string;
  margin?: string;

  boxShadow?: string;

  /** background */
  backgroundType?: 'solidColor' | 'gradient' | 'image' | 'none';
  backgroundColor?: string;
  backgroundGradient?: string;
  imageId?: string;
  backgroundImage?: string;
  backgroundPosition?: string;
  backgroundSize?: string;
  backgroundRepeat?: string;
}
