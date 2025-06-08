import { Component, Inject, Injector, OnInit } from '@angular/core';
import { finalize } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { FileManagerServiceProxy } from '@shared/service-proxies/api/file-manager.service';
import { CreateFolderCommand } from '@shared/service-proxies/model/create-folder-command';

@Component({
  selector: 'app-create-folder',
  templateUrl: './create-folder.component.html',
  styleUrls: ['./create-folder.component.scss'],
  standalone: false,
})
export class CreateFolderDialogComponent extends AppComponentBase implements OnInit {
  loading = false;
  folderName = '';
  parentId = '';
  basePath = AppConst.apiUrl;
  constructor(
    injector: Injector,
    private matDialogRef: MatDialogRef<CreateFolderDialogComponent>,
    @Inject(MAT_DIALOG_DATA) data: any,
    private fileManagerService: FileManagerServiceProxy,
  ) {
    super(injector);
    this.parentId = data.parentId;
  }

  ngOnInit(): void {}

  createFolder() {
    this.loading = true;
    this.fileManagerService
      .createFolder(
        new CreateFolderCommand({
          name: this.folderName,
          parentId: this.parentId,
        }),
      )
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((response) => {
        this.notify.success(this.l('Message.SaveSuccessfully'));
        this.matDialogRef.close(response.data);
      });
  }
}
