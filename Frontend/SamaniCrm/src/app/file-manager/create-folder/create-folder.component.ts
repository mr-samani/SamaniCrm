import { Component, EventEmitter, Inject, Injector, Input, OnInit, Output } from '@angular/core';
import { finalize } from 'rxjs';
import { Apis } from '@shared/apis';
import { FileManagerDto } from '../models/file-manager-dto';
import { CreateFolderRequest } from '../models/create-folder-request';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';

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
  ) {
    super(injector);
    this.parentId = data.parentId ?? '';
  }

  ngOnInit(): void {}

  createFolder() {
    this.loading = true;
    // this.dataService
    //   .post<CreateFolderRequest, FileManagerDto[]>(Apis.createDirectory, {
    //     name: this.folderName,
    //     parentId: this.parentId,
    //   })
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((response) => {
    //     this.notify.success(this.l('Message.SaveSuccessfully'));
    //     this.matDialogRef.close(response.data);
    //   });
  }
}
