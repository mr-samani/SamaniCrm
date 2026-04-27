export interface ContextMenuItem {
  title: string;
  icon: string; // e.g. 'fa fa-trash'
  callback: () => void;
  disabled?: boolean;
  danger?: boolean;
  dividerAfter?: boolean;
}
