import { RolePermissionsDTO } from '@shared/service-proxies';

export class TreeViewModel extends RolePermissionsDTO {
  open?: boolean;
  isModified?: boolean;
  isOpen?: boolean;
  hasUnSelectedChildren?: boolean;
  hasChildren?: boolean;
  override children: TreeViewModel[] = [];
}
