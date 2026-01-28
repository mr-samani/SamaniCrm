import { Component, Inject,  OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { LanguageDTOExtended } from '../language-list/language-list.component';
import { FormGroup, Validators } from '@angular/forms';
import { CreateOrEditLanguageCommand, LanguageServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-create-or-edit-language',
  templateUrl: './create-or-edit-language.component.html',
  styleUrls: ['./create-or-edit-language.component.scss'],
  standalone: false,
})
export class CreateOrEditLanguageComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  isUpdate: boolean;
  saving = false;
  constructor(
    @Inject(MAT_DIALOG_DATA) _data: LanguageDTOExtended,
    private dialogRef: MatDialogRef<CreateOrEditLanguageComponent>,
    private languageService: LanguageServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      culture: ['', [Validators.required, Validators.maxLength(5)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      isRtl: [false, []],
      flag: ['', []],
      isDefault: [false, []],
      isActive: [true, []],
    });
    if (_data) {
      this.isUpdate = true;
      this.form.patchValue(_data);
    } else {
      this.isUpdate = false;
    }
  }

  ngOnInit() {}

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.saving = true;
    const input = new CreateOrEditLanguageCommand();
    input.init(this.form.value);
    this.languageService
      .createOrUpdate(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');
          this.dialogRef.close(true);
        }
      });
  }
}
