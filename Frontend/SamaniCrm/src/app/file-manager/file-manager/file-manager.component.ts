import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, Inject, Injector, OnDestroy, OnInit } from '@angular/core';
import { Subject, catchError, debounceTime, distinctUntilChanged, finalize, map, of, switchMap } from 'rxjs';
import { SelectIconDialogComponent } from '../select-icon/select-icon.component';
import { CreateFolderDialogComponent } from '../create-folder/create-folder.component';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FileManagerDto } from '../models/file-manager-dto';
import { Apis } from '@shared/apis';
import {
  ImageCropperDialogComponent,
  ImageCropperDialogData,
} from '../image-cropper-dialog/image-cropper-dialog.component';
import { ApiResult } from '@shared/models/api-result';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { IOptions } from '../options.interface';

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

  imageExtensions = ['.jpg', '.png', '.jpeg', '.tif', '.gif', '.bmp'];
  cdnUrl = AppConst.apiUrl;

  getDetailsRequest$ = new Subject<{ id: string }>();
  constructor(
    injector: Injector,
    private http: HttpClient,
    private dialogRef: MatDialogRef<FileManagerComponent>,
    @Inject(MAT_DIALOG_DATA) _data: IOptions,
    private matDialog: MatDialog,
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
    // this.loadingFolders = true;
    // this.dataService
    //   .get<any, FileManagerDto[]>(Apis.getFolders, {})
    //   .pipe(finalize(() => (this.loadingFolders = false)))
    //   .subscribe((result) => {
    //     this.folders = result.result ?? [];
    //   });
  }

  private initGetFolderDetails() {
    // this.getDetailsRequest$
    //   .pipe(
    //     debounceTime(500),
    //     distinctUntilChanged(),
    //     switchMap((input) => {
    //       this.loading = true;
    //       this.fileList = [];
    //       return this.dataService.get<any, FileManagerDto[]>(Apis.getFileDetails, input).pipe(
    //         catchError((err, caught) => {
    //           return of(new ApiResult<FileManagerDto[]>());
    //         }),
    //         map((response) => response.data ?? []),
    //         finalize(() => (this.loading = false)),
    //       );
    //     }),
    //   )
    //   .subscribe((result) => {
    //     this.fileList = result ?? [];
    //   });
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
      error: (err: HttpErrorResponse) => console.log(err),
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
    this.openedFolder = item;
    this.selectedFileInfo = item;
    this.getDetailsRequest$.next({ id: item.id });
  }

  selectIcon() {
    if (!this.selectedFileInfo || !this.selectedFileInfo.isDirectory) {
      return;
    }
    this.matDialog
      .open(SelectIconDialogComponent, {
        data: this.selectedFileInfo,
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
    if (item.isDirectory) {
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
    if (!this.selectedFileInfo) {
      return;
    }
    this.alert
      .show({
        title: this.l('Delete'),
        text: this.l('AreYouSureDelete'),
        showConfirmButton: true,
        showCancelButton: true,
        confirmButtonText: this.l('Yes'),
        cancelButtonText: this.l('Cancel'),
      })
      .then((result) => {
        if (result.isConfirmed) {
          // this.showMainLoading();
          // this.fileManagerProxy.delete(this.selectedFileInfo?.id)
          //   .pipe(finalize(() => this.hideMainLoading()))
          //   .subscribe(response => {
          //     if (response.data == true) {
          //       this.notify.success(this.l('DeleteSuccessfully'));
          //       this.reload();
          //     }
          //   });
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
