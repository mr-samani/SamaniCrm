import { Component, Inject, Injector, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { Apis } from '@shared/apis';
import { finalize } from 'rxjs';
import { GetMenuForEditDto, MenuTranslation } from '../models/get-menu-for-edit';
import { LanguageDto } from '@shared/models/language-dto';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'create-or-edit-menu',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
})
export class CreateOrEditMenuComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: MenuTranslation[];
  id: string;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) _data: any,
    private dialogRef: MatDialogRef<CreateOrEditMenuComponent>,
  ) {
    super(injector);
    this.form = this.fb.group({
      url: ['', [Validators.maxLength(500)]],
      icon: ['', [Validators.maxLength(200)]],
      translations: this.fb.array([]),
    });
    this.id = _data.id;

    if (_data.id) {
      this.isUpdate = true;
      this.getForEdit(_data.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}

  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push({
        id: undefined,
        lang: item.code,
        title: '',
      });
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.dataService
      .get<{ id: string }, GetMenuForEditDto>(Apis.getMenuForEdit, { id })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: (response) => {
          this.form.patchValue(response.data);
          this.translations = response.data.translations;
          this.setTranslations();
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
          lang: [translation.lang, Validators.required],
          // data: this.fb.group({
          id: [translation.id],
          title: [translation.title, Validators.required],
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
    const input = this.form.value;
    input.menuId = this.id;
    this.dataService
      .post(Apis.createOrEditMenu, input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.notify.success(this.l('SaveSuccessFully'));
            this.dialogRef.close(true);
          } else {
            let msg = response.message ?? this.l('Message.ErrorOccurred');
            this.notify.error(msg);
          }
        },
      });
  }
}
