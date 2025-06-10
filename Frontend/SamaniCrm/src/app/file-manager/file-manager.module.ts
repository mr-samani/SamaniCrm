import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCropperDialogComponent } from './image-cropper-dialog/image-cropper-dialog.component';
import { FormsModule } from '@angular/forms';
import { ImageCropperComponent } from 'ngx-image-cropper';
import { TranslateModule } from '@ngx-translate/core';
import { FileManagerService } from './file-manager.service';
import { FileManagerComponent } from './file-manager/file-manager.component';
import { SelectIconDialogComponent } from './components/select-icon/select-icon.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { AddressBarComponent } from './components/address-bar/address-bar.component';
import { SharedModule } from '@shared/shared.module';
import { TusUploadService } from './tus-upload.service';
import { FileManagerServiceProxy } from '@shared/service-proxies';
import { CreateFolderDialogComponent } from './components/create-folder/create-folder.component';
import { FileSelectorComponent } from './components/file-selector/file-selector.component';
import { TreeFolderComponent } from './components/tree-folder/tree-folder.component';
import { FileListComponent } from './components/file-list/file-list.component';

@NgModule({
  declarations: [
    ImageCropperDialogComponent,
    FileManagerComponent,
    TreeFolderComponent,
    SelectIconDialogComponent,
    CreateFolderDialogComponent,
    FileSelectorComponent,
    AddressBarComponent,
    FileListComponent,
  ],
  imports: [CommonModule, FormsModule, MaterialCommonModule, ImageCropperComponent, TranslateModule, SharedModule],
  providers: [TusUploadService, FileManagerService, FileManagerServiceProxy],
  exports: [FileSelectorComponent],
})
export class FileManagerModule {}
