import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';

@Component({
  selector: 'address-bar',
  templateUrl: './address-bar.component.html',
  styleUrl: './address-bar.component.scss',
  standalone: false,
})
export class AddressBarComponent {
  private _folders: FolderBreadcrumb[] = [];
  private _folderId: string = '';
  @Input() set folders(val: FileManagerDto[]) {
    this._folders = this.createHierarchy(val);
  }
  @Input() set folderId(val: string) {
    this._folderId = val;
    this.initBreadcrumb();
  }
  @Output() selectionChange = new EventEmitter<FileManagerDto>();

  breadcrumb: FolderBreadcrumb[] = [];

  onSelect(item: FolderBreadcrumb) {
    this.selectionChange.emit(item);
  }

  private initBreadcrumb() {
    let finded = this._folders.find((x) => x.id == this._folderId);
    let parentList = finded?.hierarchy?.split('.') ?? [];
    this.breadcrumb = [];
    for (let item of parentList) {
      let p = this._folders.find((x) => x.id == item);
      this.breadcrumb.push(p!);
    }
  }

  private createHierarchy(nodes: FolderBreadcrumb[], parentIds = ''): FolderBreadcrumb[] {
    let flatList: FolderBreadcrumb[] = [];
    for (let node of nodes) {
      let currentId = parentIds ? `${parentIds}.${node.id}` : `${node.id}`;
      node.hierarchy = currentId;
      flatList.push(node);
      if (node.children && node.children.length > 0) {
        flatList = flatList.concat(this.createHierarchy(node.children, currentId));
      }
    }
    return flatList;
  }
}

export class FolderBreadcrumb extends FileManagerDto {
  hierarchy?: string;
  declare  children: FolderBreadcrumb[];
}
