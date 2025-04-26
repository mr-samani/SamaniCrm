export class RolePermissionsDto {
  id!: string;
  name!: string;
  // slug!:      string;
  parent_id?: null | string;
  children!: RolePermissionsDto[];

  selected?: boolean;
}
