import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FileManagerDto } from '../models/file-manager-dto';

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
  openCloseFolder(item: FileManagerDto) {
    item.isOpen = !item.isOpen;
    this.openFolder.emit(item);
  }

  onChildOpenFolder(item: FileManagerDto) {
    this.openFolder.emit(item);
  }
}
