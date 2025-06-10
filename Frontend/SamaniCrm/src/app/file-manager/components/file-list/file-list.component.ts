import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import {
  DeleteFileOrFolderCommand,
  FileManagerServiceProxy,
  FileNodeDtoListApiResponse,
} from '@shared/service-proxies';
import { Subject, debounceTime, distinctUntilChanged, switchMap, catchError, of, map, finalize } from 'rxjs';
import { SelectIconDialogComponent } from '../select-icon/select-icon.component';
import { MatDialog } from '@angular/material/dialog';
import { FileManagetConsts } from '@app/file-manager/consts/file-manager-consts';
import { AppConst } from '@shared/app-const';
import { isImage } from '@app/file-manager/consts/is-image';
import { isVideo } from '@app/file-manager/consts/is-video';

@Component({
  selector: 'file-list',
  templateUrl: './file-list.component.html',
  styleUrls: ['./file-list.component.scss'],
  standalone: false,
})
export class FileListComponent extends AppComponentBase implements OnInit, OnDestroy {
  @Input('openFolder') set openFolder(item: FileManagerDto | undefined) {
    this.fileList = [];
    if (item) {
      this.selectedFileInfo = item;
      this.getDetailsRequest$.next({ id: item.id! });
    }
  }
  @Output() openFolderChange = new EventEmitter<FileManagerDto>();

  @Output() onSelectFile = new EventEmitter<FileManagerDto>();
  /** پوشه انتخاب شده برای نمایش جزئیات در سمت راست */
  selectedFileInfo?: FileManagerDto;
  fileList: FileManagerDto[] = [];
  loading = false;
  getDetailsRequest$ = new Subject<{ id: string }>();
  baseUrl = AppConst.apiUrl;
  fileServerUrl = AppConst.fileServerUrl;
  defaultOpenFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultOpenFolderIcon;
  defaultFolderIcon = AppConst.apiUrl + FileManagetConsts.DefaultFolderIcon;
  defaultFileIcon = AppConst.apiUrl + FileManagetConsts.DefaultFileIcon;
  constructor(
    injector: Injector,
    private fileManagerService: FileManagerServiceProxy,
    private matDialog: MatDialog,
  ) {
    super(injector);
  }

  ngOnInit() {
    this.initGetFolderDetails();
  }
  ngOnDestroy(): void {
    if (this.getDetailsRequest$) {
      this.getDetailsRequest$.unsubscribe();
    }
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
            map((response) => {
              var list: FileManagerDto[] = response.data ?? [];
              for (let item of list) {
                item.isImage = isImage(item.extension);
                item.isVideo = isVideo(item.extension);
              }
              return list;
            }),
            finalize(() => (this.loading = false)),
          );
        }),
      )
      .subscribe((result) => {
        this.fileList = result;
      });
  }

  public reload() {
    if (this.selectedFileInfo) {
      this.fileList = [];
      this.getDetailsRequest$.next({ id: this.selectedFileInfo.id! });
    }
  }

  selectFile(item: FileManagerDto) {
    this.selectedFileInfo = item;
  }

  dblClickFile(item: FileManagerDto) {
    if (item.isFolder) {
      this.fileList = [];
      this.selectedFileInfo = item;
      this.getDetailsRequest$.next({ id: item.id! });
      this.openFolderChange.emit(item);
    }
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
      .subscribe((icon: string) => {
        if (icon && this.selectedFileInfo) {
          this.selectedFileInfo.icon = icon;
        }
      });
  }

  deleteFileOrFolder() {
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
              let findedIndex = this.fileList.findIndex((x) => x.id == this.selectedFileInfo?.id);
              if (findedIndex > -1) {
                this.fileList.splice(findedIndex, 1);
              }
              this.selectedFileInfo = undefined;
            }
          });
      }
    });
  }

  chooseThisFile() {
    if (this.selectedFileInfo && this.selectedFileInfo.isFolder == false) {
      this.onSelectFile.emit(this.selectedFileInfo);
    }
  }
}
