import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
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
import { setPreviousFolderId } from '@app/file-manager/consts/PreviousFolderId';
import { ContextMenuItem } from '@shared/directives/context-menu/context-menu.model';
import { RenameDialogComponent } from '../rename/rename.component';
import { AppPermissions } from '@shared/permissions/app-permissions';

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
      if (item.isFolder) {
        setPreviousFolderId(item.id!);
      }
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

  fileContextMenu: ContextMenuItem[] = [];

  constructor(
    private fileManagerService: FileManagerServiceProxy,
    private matDialog: MatDialog,
  ) {
    super();
  }

  ngOnInit() {
    if (this.isGranted(AppPermissions.FileManager_CreateFile)) {
      this.fileContextMenu.push({
        title: this.l('Rename'),
        icon: 'fa fa-i-cursor',
        callback: () => this.renameFileOrFolder(),
      });
    }
    if (this.isGranted(AppPermissions.FileManager_Delete)) {
      this.fileContextMenu.push({
        title: this.l('Delete'),
        icon: 'fa fa-trash',
        callback: () => this.deleteFileOrFolder(),
        danger: true,
      });
    }
    this.fileContextMenu.push({
      title: this.l('ChooseThisFile'),
      icon: 'fa fa-octagon-check',
      callback: () => this.chooseThisFile(),
    });

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
            finalize(() => {
              this.loading = false;
              this.chdr.detectChanges();
            }),
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
      let id = '';
      if (this.selectedFileInfo.isFolder) {
        id = this.selectedFileInfo.id!;
      } else {
        id = this.selectedFileInfo.parentId!;
      }
      this.getDetailsRequest$.next({ id });
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
      setPreviousFolderId(item.id!);
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
          this.chdr.detectChanges();
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
          .pipe(
            finalize(() => {
              this.hideMainLoading();
              this.chdr.detectChanges();
            }),
          )
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

  renameFileOrFolder() {
    if (!this.selectedFileInfo) {
      return;
    }
    this.matDialog
      .open(RenameDialogComponent, {
        data: this.selectedFileInfo,
        width: '768px',
      })
      .afterClosed()
      .subscribe((name: string) => {
        if (name && this.selectedFileInfo) {
          this.selectedFileInfo.name = name;
          this.chdr.detectChanges();
        }
      });
  }

  download() {
    if (!this.selectedFileInfo || this.selectedFileInfo.isFolder) {
      return;
    }
    const a = this.doc.createElement('a');
    a.download = this.selectedFileInfo.name ?? 'Download';
    a.href = this.fileServerUrl + '/' + this.selectedFileInfo.id;
    a.click();
    a.remove();
  }

 

  chooseThisFile() {
    if (this.selectedFileInfo && this.selectedFileInfo.isFolder == false) {
      this.onSelectFile.emit(this.selectedFileInfo);
    }
  }
}
