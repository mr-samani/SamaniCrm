import { HttpClient } from '@angular/common/http';
import { Component, Inject, Injector, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FileManagerDto } from '../models/file-manager-dto';
import {
  FileUsageEnum,
  ImageCropperDialogComponent,
  ImageCropperDialogData,
} from '../image-cropper-dialog/image-cropper-dialog.component';
import { AppComponentBase } from '@app/app-component-base';
import { IOptions } from '../options.interface';
import { FileManagerServiceProxy } from '@shared/service-proxies';
import { CreateFolderDialogComponent } from '../components/create-folder/create-folder.component';
import { TusUploadService } from '../tus-upload.service';
import { FileListComponent } from '../components/file-list/file-list.component';

@Component({
  selector: 'app-file-manager',
  templateUrl: './file-manager.component.html',
  styleUrls: ['./file-manager.component.scss'],
  standalone: false,
})
export class FileManagerComponent extends AppComponentBase implements OnInit, OnDestroy {
  progress = 0;
  message = '';
  loadingFolders = true;
  folders: FileManagerDto[] = [];
  loading = false;

  /** پوشه فعال */
  openedFolder?: FileManagerDto;

  @ViewChild('fileList', { static: false }) _fileList?: FileListComponent;

  constructor(
    injector: Injector,
    private http: HttpClient,
    private dialogRef: MatDialogRef<FileManagerComponent>,
    @Inject(MAT_DIALOG_DATA) _data: IOptions,
    private matDialog: MatDialog,
    private fileManagerService: FileManagerServiceProxy,
    public tusUpoadService: TusUploadService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getTreeFolders();
  }

  ngOnDestroy(): void {}

  getTreeFolders() {
    this.loadingFolders = true;
    this.fileManagerService
      .getTreeFolders()
      .pipe(finalize(() => (this.loadingFolders = false)))
      .subscribe((result) => {
        this.folders = result.data ?? ([] as any);
        if (this.openedFolder && this.openedFolder.id) {
          this.tryOpenFolderInTree(this.folders, this.openedFolder.id);
        }
      });
  }

  tryOpenFolderInTree(tree: FileManagerDto[], id: string): boolean {
    for (const f of tree) {
      if (f.id === id) {
        f.isOpen = true;
        return true;
      }

      if (f.children && this.tryOpenFolderInTree(f.children, id)) {
        f.isOpen = true;
        return true;
      }
    }
    return false;
  }

  reload(reloadFolderView = false) {
    this.getTreeFolders();
    if (reloadFolderView && this._fileList) {
      this._fileList.reload();
    }
  }

  uploadFile = (fileInput: HTMLInputElement) => {
    let files = fileInput.files;
    if (!files || files.length === 0 || !this.openedFolder || !this.openedFolder.id) {
      return;
    }
    let filesToUpload: FileList = files;
    let parentId = this.openedFolder.isFolder ? this.openedFolder.id : this.openedFolder.parentId;
    Array.from(filesToUpload).map((file, index) => {
      this.tusUpoadService
        .uploadFile(file, '', FileUsageEnum.FILE_MANAGER, '', parentId)
        .then((result) => {
          // this.progress = Math.round((100 * event.loaded) / (event.total ?? 0));
          this.notify.success(this.l('Message.UploadedSuccessFully'));
          this.reload();
        })
        .catch((error) => {
          this.notify.error(this.l('Message.AnErrorOccurred'));
        });
    });
  };

  uploadImage() {
    this.matDialog
      .open(ImageCropperDialogComponent, {
        data: <ImageCropperDialogData>{
          parentId: this.openedFolder?.id,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        this.reload(true);
      });
  }

  createFolder() {
    this.matDialog
      .open(CreateFolderDialogComponent, {
        data: {
          parentId: this.openedFolder?.id,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.reload(true);
        }
      });
  }

  onSelectFile(ev: FileManagerDto) {
    // todo: validate choosed file
    this.dialogRef.close(ev.id);
  }
}
