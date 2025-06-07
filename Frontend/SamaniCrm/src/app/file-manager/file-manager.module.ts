import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCropperDialogComponent } from './image-cropper-dialog/image-cropper-dialog.component';
import { FormsModule } from '@angular/forms';
import { ImageCropperComponent } from 'ngx-image-cropper';
import { TranslateModule } from '@ngx-translate/core';
import { FileManagerService } from './file-manager.service';
import { FileSelectorComponent } from './file-selector/file-selector.component';
import { CreateFolderDialogComponent } from './create-folder/create-folder.component';
import { FileManagerComponent } from './file-manager/file-manager.component';
import { SelectIconDialogComponent } from './select-icon/select-icon.component';
import { TreeFolderComponent } from './tree-folder/tree-folder.component';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { AddressBarComponent } from './file-manager/address-bar/address-bar.component';
import { SharedModule } from '@shared/shared.module';
import { TusUploadService } from './tus-upload.service';
import { FileManagerServiceProxy } from '@shared/service-proxies';

@NgModule({
  declarations: [
    ImageCropperDialogComponent,
    FileManagerComponent,
    TreeFolderComponent,
    SelectIconDialogComponent,
    CreateFolderDialogComponent,
    FileSelectorComponent,
    AddressBarComponent,
  ],
  imports: [CommonModule, FormsModule, MaterialCommonModule, ImageCropperComponent, TranslateModule, SharedModule],
  providers: [TusUploadService, FileManagerService, FileManagerServiceProxy],
})
export class FileManagerModule {}
