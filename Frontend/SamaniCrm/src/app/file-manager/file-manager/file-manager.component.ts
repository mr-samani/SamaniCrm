import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, Inject, Injector, OnDestroy, OnInit } from '@angular/core';
import { Subject, catchError, debounceTime, distinctUntilChanged, finalize, map, of, switchMap } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FileManagerDto } from '../models/file-manager-dto';
import { Apis } from '@shared/apis';
import {
  ImageCropperDialogComponent,
  ImageCropperDialogData,
} from '../image-cropper-dialog/image-cropper-dialog.component';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { IOptions } from '../options.interface';
import { FileManagerService } from '../file-manager.service';
import {
  DeleteFileOrFolderCommand,
  FileManagerServiceProxy,
  FileNodeDtoListApiResponse,
} from '@shared/service-proxies';
import { FileManagetConsts } from '../consts/file-manager-consts';
import { CreateFolderDialogComponent } from '../components/create-folder/create-folder.component';
import { SelectIconDialogComponent } from '../components/select-icon/select-icon.component';

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
  fileList: FileManagerDto[] = [];
  loading = false;

  /** پوشه فعال */
  openedFolder?: FileManagerDto;
  /** پوشه انتخاب شده برای نمایش جزئیات در سمت راست */
  selectedFileInfo?: FileManagerDto;

  baseUrl = AppConst.apiUrl;

  getDetailsRequest$ = new Subject<{ id: string }>();

  defaultOpenFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultOpenFolderIcon;
  defaultFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultFolderIcon;
  defaultFileIcon = AppConst.apiUrl + FileManagetConsts.DefaultFileIcon;

  constructor(
    injector: Injector,
    private http: HttpClient,
    private dialogRef: MatDialogRef<FileManagerComponent>,
    @Inject(MAT_DIALOG_DATA) _data: IOptions,
    private matDialog: MatDialog,
    private fileManagerService: FileManagerServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getTreeFolders();
    this.initGetFolderDetails();
  }

  ngOnDestroy(): void {
    if (this.getDetailsRequest$) {
      this.getDetailsRequest$.unsubscribe();
    }
  }

  getTreeFolders() {
    this.loadingFolders = true;
    this.fileManagerService
      .getTreeFolders()
      .pipe(finalize(() => (this.loadingFolders = false)))
      .subscribe((result) => {
        this.folders = result.data ?? ([] as any);
      });
  }

  private initGetFolderDetails() {
    this.getDetailsRequest$
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((input) => {
          this.loading = true;
          this.fileList = [];
          return this.fileManagerService.getFolderDetails(input.id).pipe(
            catchError((err, caught) => {
              return of(new FileNodeDtoListApiResponse());
            }),
            map((response) => response.data ?? []),
            finalize(() => (this.loading = false)),
          );
        }),
      )
      .subscribe((result) => {
        this.fileList = result;
      });
  }

  reload() {
    this.getTreeFolders();
    if (this.openedFolder) {
      this.openFolder(this.openedFolder);
    }
  }

  uploadFile = (fileInput: HTMLInputElement) => {
    let files = fileInput.files;
    if (!files || files.length === 0) {
      return;
    }
    let filesToUpload: FileList = files;
    const formData = new FormData();
    var parentId = '';

    if (this.openedFolder && this.openedFolder.id) {
      parentId = this.openedFolder.id;
      formData.append('parentId', parentId);
    }
    Array.from(filesToUpload).map((file, index) => {
      return formData.append('file', file, file.name);
    });

    this.http.post(AppConst.apiUrl + Apis.uploadFile, formData, { reportProgress: true, observe: 'events' }).subscribe({
      next: (event) => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round((100 * event.loaded) / (event.total ?? 0));
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          if (this.openedFolder) {
            this.openFolder(this.openedFolder);
          }
          setTimeout(() => {
            this.progress = 0;
            this.message = '';
          }, 1000);
          // this.onUploadFinished.emit(event.body);
        }
      },
      error: (err: HttpErrorResponse) => {
        console.log(err);
        this.progress = 0;
      },
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
        this.reload();
      });
  }

  openFolder(item: FileManagerDto) {
    if (this.selectedFileInfo?.id == item.id) {
      return;
    }
    this.openedFolder = item;
    this.selectedFileInfo = item;
    this.getDetailsRequest$.next({ id: item.id! });
  }

  selectIcon() {
    if (!this.selectedFileInfo || !this.selectedFileInfo.isFolder) {
      return;
    }
    this.matDialog
      .open(SelectIconDialogComponent, {
        data: this.selectedFileInfo,
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.selectedFileInfo = result;
          this.reload();
        }
      });
  }

  selectFile(item: FileManagerDto) {
    this.selectedFileInfo = item;
  }

  dblClickFile(item: FileManagerDto) {
    if (item.isFolder) {
      this.openFolder(item);
    }
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
          this.reload();
        }
      });
  }

  delete() {
    if (!this.selectedFileInfo || !this.selectedFileInfo.id) {
      return;
    }
    this.confirmMessage(this.l('Delete'), this.l('AreYouSureDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.fileManagerService
          .deleteFileOrFolder(new DeleteFileOrFolderCommand({ id: this.selectedFileInfo?.id }))
          .pipe(finalize(() => this.hideMainLoading()))
          .subscribe((response) => {
            if (response.data == true) {
              this.notify.success(this.l('DeleteSuccessfully'));
              this.reload();
            }
          });
      }
    });
  }

  chooseThisFile() {
    if (this.selectedFileInfo) {
      // todo: validate choosed file
      this.dialogRef.close(this.selectedFileInfo.id);
    }
  }
}
