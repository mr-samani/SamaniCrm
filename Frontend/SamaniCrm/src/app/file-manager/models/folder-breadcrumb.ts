import { FileManagerDto } from "./file-manager-dto";

export class FolderBreadcrumb extends FileManagerDto {
  hierarchy?: string;
  declare children: FolderBreadcrumb[];
  constructor(data?: any) {
    super(data);
    if (data) {
      for (let property in data) {
        if (data.hasOwnProperty(property)) (this as any)[property] = (data as any)[property];
      }
    }
  }
}
