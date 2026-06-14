import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormArray, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { SubscriptionServiceProxy } from '@shared/service-proxies/api/subscription.service';
import { BillingType } from '@shared/service-proxies/model/billing-type';
import { CreateOrEditPlanCommand } from '@shared/service-proxies/model/create-or-edit-plan-command';
import { PlanDto } from '@shared/service-proxies/model/plan-dto';
import { PlanTranslationDto } from '@shared/service-proxies/model/plan-translation-dto';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'create-or-edit-category',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss'],
  standalone: false,
})
export class CreateOrEditPlanComponent extends AppComponentBase implements OnInit {
  form: FormGroup;
  loading = false;
  saving = false;
  isUpdate: boolean;
  translations?: PlanTranslationDto[];

  _data = inject<PlanDto | undefined>(MAT_DIALOG_DATA);
  constructor(
    private dialogRef: MatDialogRef<CreateOrEditPlanComponent>,
    private subscriptionService: SubscriptionServiceProxy,
  ) {
    super();
    this.form = this.fb.group({
      code: ['', [Validators.maxLength(100)]],
      billingType: [null, [Validators.required]],
      translations: this.fb.array([]),
      isActive: [true],
      isPublic: [true],
    });

    if (this._data?.id) {
      this.isUpdate = true;
      this.getForEdit(this._data.id);
    } else {
      this.isUpdate = false;
      this.getForCreate();
    }
  }

  ngOnInit(): void {}
  public get BillingType(): typeof BillingType {
    return BillingType;
  }
  getForCreate() {
    this.translations = [];
    for (let item of AppConst.languageList ?? []) {
      this.translations.push(
        new PlanTranslationDto({
          culture: item.culture!,
          title: '',
          description: '',
          planId: this._data?.id,
        }),
      );
    }
    this.setTranslations();
  }

  getForEdit(id: string) {
    this.loading = true;
    this.subscriptionService
      .getPlanForEdit(id)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
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
          description: [translation.description],
          PlanId: [translation.planId],
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
    const formValue = this.form.value;

    this.saving = true;
    const input = new CreateOrEditPlanCommand();

    input.init(formValue);
    input.id = this._data?.id;

    this.subscriptionService
      .createOrEditPlan(input)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.chdr.detectChanges();
        }),
      )
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
