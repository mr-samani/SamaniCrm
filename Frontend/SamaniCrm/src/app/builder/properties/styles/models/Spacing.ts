import { SizeUnit } from './SizeUnit';

export class SpacingValue {
  size?: number | 'auto';
  unit?: SizeUnit;
}

export class Spacing {
  top?: SpacingValue;
  right?: SpacingValue;
  bottom?: SpacingValue;
  left?: SpacingValue;
}
