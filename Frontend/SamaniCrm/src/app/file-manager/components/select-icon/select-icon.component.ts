import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { FileManagerServiceProxy } from '@shared/service-proxies/api/file-manager.service';
import { SetFolderIconCommand } from '@shared/service-proxies/model/set-folder-icon-command';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-select-icon',
  templateUrl: './select-icon.component.html',
  styleUrls: ['./select-icon.component.scss'],
  standalone: false,
})
export class SelectIconDialogComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: string[] = [];
  folderId: string;
  saving = false;
  selected?: string;
  baseUrl = AppConst.apiUrl;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private matDialogRef: MatDialogRef<SelectIconDialogComponent>,
    private fileManagerService: FileManagerServiceProxy,
  ) {
    super(injector);
    this.folderId = _data.id;
  }

  ngOnInit(): void {
    this.getIcons();
  }

  getIcons() {
    this.loading = true;
    this.fileManagerService
      .getFileManagerIcons()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.list = result.data ?? [];
      });
  }

  changeIcon() {
    if (!this.selected) {
      return;
    }
    this.saving = true;
    this.fileManagerService
      .setFolderIcon(
        new SetFolderIconCommand({
          icon: this.selected,
          id: this.folderId,
        }),
      )
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.data) {
          this.matDialogRef.close(this.selected);
        }
      });
  }
}
