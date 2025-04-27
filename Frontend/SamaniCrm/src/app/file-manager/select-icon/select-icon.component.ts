import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-select-icon',
  templateUrl: './select-icon.component.html',
  styleUrls: ['./select-icon.component.scss'],
  standalone: false,
})
export class SelectIconDialogComponent extends AppComponentBase implements OnInit {
  loading = true;
  list: string[] = [];
  folderId: string;
  saving = false;
  selected?: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private matDialogRef: MatDialogRef<SelectIconDialogComponent>,
  ) {
    super(injector);
    this.folderId = _data.id;
  }

  ngOnInit(): void {
    this.getIcons();
  }

  getIcons() {
    this.loading = true;
    // this.dataService
    //   .get(Apis.getFilemanagerIcons, {})
    //   .pipe(finalize(() => (this.loading = false)))
    //   .subscribe((result) => {
    //     this.list = result.result ?? [];
    //   });
  }

  changeIcon() {
    if (!this.selected) {
      return;
    }
    this.saving = true;
    // this.dataService
    //   .post(Apis.setFolderIcon, {
    //     icon: this.selected,
    //     id: this.folderId,
    //   })
    //   .pipe(finalize(() => (this.saving = false)))
    //   .subscribe((response) => {
    //     if (response.data) {
    //       this.matDialogRef.close(response.data);
    //     }
    //   });
  }
}
