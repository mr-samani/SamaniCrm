import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileManagetConsts } from '@app/file-manager/consts/file-manager-consts';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'tree-folder',
  templateUrl: './tree-folder.component.html',
  styleUrls: ['./tree-folder.component.scss'],
  standalone: false,
})
export class TreeFolderComponent {
  @Input() isOpen?: boolean = true;
  @Input('folders') folders: FileManagerDto[] = [];
  @Output('openFolder') openFolder = new EventEmitter<FileManagerDto>();

  baseUrl = AppConst.apiUrl;

  defaultOpenFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultOpenFolderIcon;
  defaultFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultFolderIcon;
  defaultFileIcon = AppConst.apiUrl + FileManagetConsts.DefaultFileIcon;
  openCloseFolder(item: FileManagerDto) {
    item.isOpen = !item.isOpen;
    this.openFolder.emit(item);
  }

  onChildOpenFolder(item: FileManagerDto) {
    this.openFolder.emit(item);
  }
}
