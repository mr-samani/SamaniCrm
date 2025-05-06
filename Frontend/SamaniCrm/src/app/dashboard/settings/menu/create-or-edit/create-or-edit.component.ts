import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { finalize } from 'rxjs';
import { AppConst } from '@shared/app-const';
import { MenuServiceProxy } from '@shared/service-proxies/api/menu.service';
import { CreateOrEditMenuCommand, MenuTargetEnum, MenuTranslationsDTO } from '@shared/service-proxies';

@Component({
  selector: 'create-or-edit-menu',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditMenuComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: MenuTranslationsDTO[];
  id: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: { id: string },
    private dialogRef: MatDialogRef<CreateOrEditMenuComponent>,
    private menuService: MenuServiceProxy,
  ) {
    super(injector);
    this.form = this.fb.group({
      url: ['', [Validators.maxLength(500)]],
      icon: ['', [Validators.maxLength(200)]],
      translations: this.fb.array([]),
      isActive: [true],
      isSystem: [{ value: false, disabled: true }],
      target: [MenuTargetEnum.Self, [Validators.required]],
    });
    this.id = _data.id;

    if (this.id) {
      this.isUpdate = true;
      this.getForEdit(this.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  public get MenuTargetEnum(): typeof MenuTargetEnum {
    return MenuTargetEnum;
  }
  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push(
        new MenuTranslationsDTO({
          culture: item.culture!,
          title: '',
          menuId: this.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.menuService
      .getForEdit(id)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.form.patchValue(response.data);
            this.translations = response.data.translations;
            this.setTranslations();
          } else {
            this.dialogRef.close();
          }
        },
        error: (err) => {
          this.dialogRef.close();
        },
      });
  }

  get translationsArray(): FormArray {
    return this.form.get('translations') as FormArray;
  }

  setTranslations() {
    if (!this.translations) {
      return;
    }
    this.translations.forEach((translation) => {
      this.translationsArray.push(
        this.fb.group({
          culture: [translation.culture],
          // data: this.fb.group({
          title: [translation.title, Validators.required],
          menuId: [translation.menuId],
          //})
        }),
      );
    });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormField'));
      return;
    }
    this.saving = true;
    const input = new CreateOrEditMenuCommand();
    input.init(this.form.value);
    input.id = this.id;
    this.menuService
      .createOrUpdate(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          }
        },
      });
  }
}
