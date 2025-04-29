export class TreeViewModel {
  isModified?: boolean;
  isOpen?: boolean;
  hasUnSelectedChildren?: boolean;
  hasChildren?: boolean;
  selected?: boolean;
  name?: string;
  declare children: TreeViewModel[];
}
