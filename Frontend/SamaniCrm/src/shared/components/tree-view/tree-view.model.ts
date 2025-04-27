import { RolePermissionsDto } from '@app/user-management/roles/models/role-permissions';

export class TreeViewModel extends RolePermissionsDto {
  isModified?: boolean;
  isOpen?: boolean;
  hasUnSelectedChildren?: boolean;
  hasChildren?: boolean;

  declare  children: TreeViewModel[];
}
