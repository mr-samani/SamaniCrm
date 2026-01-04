import { MenuDTO } from '@shared/service-proxies';


export interface TreeNode extends MenuDTO {
  children: Array<TreeNode>;
  isExpanded?: boolean;
  loading?: boolean;
}
