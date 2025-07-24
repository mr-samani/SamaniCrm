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

  // dimensions
  width?: number;
  height?: number;
  minWidth?: number;
  minHeight?: number;
  maxWidth?: number;
  maxHeight?: number;

  // flex
  flex?: number;
  flexGrow?: number;
  flexShrink?: number;
  flexBasis?: string;
  flexDirection?: string;
  flexWrap?: string;
  justifyContent?: string;
  alignItems?: string;
  alignContent?: string;
  gap?: string;

  // position
  position?: 'static' | 'relative' | 'absolute' | 'fixed' | 'sticky';
  top?: number;
  right?: number;
  bottom?: number;
  left?: number;
  zIndex?: number;

  // overflow
  overflow?: 'visible' | 'hidden' | 'scroll' | 'auto';
  overflowX?: 'visible' | 'hidden' | 'scroll' | 'auto';
  overflowY?: 'visible' | 'hidden' | 'scroll' | 'auto';

  // opacity
  opacity?: number;

  // visibility
  visibility?: 'visible' | 'hidden';

  // display
  display?: 'block' | 'inline' | 'inline-block' | 'flex' | 'grid' | 'none';

  // cursor
  cursor?: 'auto' | 'default' | 'pointer' | 'wait' | 'move' | 'not-allowed' | 'none';

  // pointer events
  pointerEvents?: 'auto' | 'none';

  // user select
  userSelect?: 'auto' | 'text' | 'none';

  // transition
  transition?: string;

  // animation
  animation?: string;
}
