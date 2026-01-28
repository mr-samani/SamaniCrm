import { Component, Inject,  OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import {
  CreateOrEditLocalizeKeyCommand,
  LanguageServiceProxy,
  LocalizationCategoryEnum,
  LocalizationKeyDTO,
} from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-new-localize-key',
  templateUrl: './add-new-localize-key.component.html',
  styleUrls: ['./add-new-localize-key.component.scss'],
  standalone: false,
})
export class AddNewLocalizeKeyComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  localizations?: { value: string; culture: string }[];
  culture: string;
  constructor(
    @Inject(MAT_DIALOG_DATA) _data: { culture: string },
    private dialogRef: MatDialogRef<AddNewLocalizeKeyComponent>,
    private languageService: LanguageServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      key: ['', [Validators.required, Validators.maxLength(500)]],
      category: [LocalizationCategoryEnum.Other, [Validators.maxLength(200)]],
      localizations: this.fb.array([]),
    });
    this.culture = _data.culture;
  }

  ngOnInit(): void {
    this.localizations = [];
    for (let item of AppConst.languageList ?? []) {
      this.localizations.push({
        value: '',
        culture: item.culture!,
      });
    }
    this.setlocalizations();
  }

  public get LocalizationCategoryEnum(): typeof LocalizationCategoryEnum {
    return LocalizationCategoryEnum;
  }

  get localizationsArray(): FormArray {
    return this.form.get('localizations') as FormArray;
  }

  setlocalizations() {
    if (!this.localizations) {
      return;
    }
    this.localizations.forEach((translation) => {
      this.localizationsArray.push(
        this.fb.group({
          value: [translation.value],
          culture: [translation.culture],
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
    const input = new CreateOrEditLocalizeKeyCommand();
    input.key = this.form.get('key')!.value;
    input.items = [];
    for (let item of this.localizationsArray.controls) {
      input.items.push(
        new LocalizationKeyDTO({
          category: this.form.get('category')!.value,
          culture: item.get('culture')!.value,
          key: input.key!,
          value: item.get('value')!.value,
        }),
      );
    }
    this.languageService
      .createOrEditLocalizeKey(input)
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
