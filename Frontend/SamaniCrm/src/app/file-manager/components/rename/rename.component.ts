import { Component, Inject, OnInit } from '@angular/core';
import { finalize } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { FileManagerServiceProxy } from '@shared/service-proxies/api/file-manager.service';
import { RenameCommand } from '@shared/service-proxies/model/rename-command';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';

@Component({
  selector: 'app-rename',
  templateUrl: './rename.component.html',
  styleUrls: ['./rename.component.scss'],
  standalone: false,
})
export class RenameDialogComponent extends AppComponentBase implements OnInit {
  loading = false;
  name = '';
  id = '';
  ext = '';

  constructor(
    private matDialogRef: MatDialogRef<RenameDialogComponent>,
    @Inject(MAT_DIALOG_DATA) data: FileManagerDto,
    private fileManagerService: FileManagerServiceProxy,
  ) {
    super();
    this.id = data.id!;
    let fileName = (data.name ?? '').split('.');
    this.ext = fileName?.pop() ?? '';
    this.name = fileName.join('.') ?? '';
  }

  ngOnInit(): void {}

  createFolder() {
    this.loading = true;
    this.fileManagerService
      .rename(
        new RenameCommand({
          name: this.name + '.' + this.ext,
          id: this.id,
        }),
      )
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((response) => {
        this.notify.success(this.l('Message.SaveSuccessfully'));
        this.matDialogRef.close(this.name);
      });
  }
}
