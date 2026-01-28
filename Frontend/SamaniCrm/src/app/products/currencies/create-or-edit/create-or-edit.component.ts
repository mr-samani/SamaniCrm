import { Component, Inject,  OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { FormGroup, Validators } from '@angular/forms';
import { CreateOrUpdateCurrencyCommand, CurrencyDto, ProductServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-create-or-editcurrency',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditCurrencyComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  isUpdate: boolean;
  saving = false;
  constructor(
    @Inject(MAT_DIALOG_DATA) _data: CurrencyDto,
    private dialogRef: MatDialogRef<CreateOrEditCurrencyComponent>,
    private productService: ProductServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      id: [''],
      currencyCode: ['', [Validators.required, Validators.maxLength(5)]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      symbol: ['', [Validators.required]],
      exchangeRateToBase: [1, [Validators.required]],
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
    const input = new CreateOrUpdateCurrencyCommand();
    input.init(this.form.value);
    if (!this.isUpdate) {
      input.id = undefined;
    }
    this.productService
      .createOrEditCurrency(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((response) => {
        if (response.success) {
          this.notify.success('SavedSuccessfully');
          this.dialogRef.close(true);
        }
      });
  }
}
