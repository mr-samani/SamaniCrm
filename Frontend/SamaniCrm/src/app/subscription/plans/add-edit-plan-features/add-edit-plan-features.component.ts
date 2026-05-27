import { Component, inject, OnInit, viewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';

import { CreateOrEditPlanComponent } from '../create-or-edit/create-or-edit.component';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { IDropEvent, moveItemInArray } from 'ngx-drag-drop-kit';
import { finalize } from 'rxjs';
import { PlanFeatureDto } from '@shared/service-proxies/model/plan-feature-dto';
import { SubscriptionServiceProxy } from '@shared/service-proxies/api/subscription.service';
import { PlanFeatureType } from '@shared/service-proxies/model/plan-feature-type';
import { GetAllPlanFeatureForEditQuery } from '@shared/service-proxies/model/get-all-plan-feature-for-edit-query';
import { PlanDto } from '@shared/service-proxies/model/plan-dto';
import { AppConst } from '@shared/app-const';
import { PlanFeatureTranslationDto } from '@shared/service-proxies/model/plan-feature-translation-dto';
import { CreateOrEditPlanFeatureCommand } from '@shared/service-proxies/model/create-or-edit-plan-feature-command';
import { NgForm } from '@angular/forms';

@Component({
  standalone: false,
  selector: 'app-add-edit-plan-features',
  templateUrl: './add-edit-plan-features.component.html',
  styleUrls: ['./add-edit-plan-features.component.scss'],
})
export class AddEditPlanFeaturesComponent extends AppComponentBase implements OnInit {
  loading = false;
  saving = false;
  features: PlanFeatureDto[] = [];

  allLanguages = AppConst.languageList;
  // تب‌های زبان
  _data = inject<PlanDto | undefined>(MAT_DIALOG_DATA);

  form = viewChild<NgForm>('frm');
  constructor(
    private dialogRef: MatDialogRef<CreateOrEditPlanComponent>,
    private subscriptionService: SubscriptionServiceProxy,
  ) {
    super();
  }

  ngOnInit(): void {
    this.loadFeatures();
  }

  public get PlanFeatureType(): typeof PlanFeatureType {
    return PlanFeatureType;
  }
  loadFeatures() {
    this.loading = true;
    this.subscriptionService
      .getAllPlanFeatureForEdit(this._data?.id)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((result) => {
        this.features = result.data ?? [];
      });
  }

  onDrop(event: CdkDragDrop<PlanFeatureDto[]>) {
    console.log(event);
    moveItemInArray(this.features, event.previousIndex, event.currentIndex);
  }

  addFeature() {
    let newFeature: PlanFeatureDto = new PlanFeatureDto({
      featureKey: '',
      unit: '',
      value: '',
      planFeatureType: PlanFeatureType.Boolean,
      planId: this._data?.id,
      translations: [],
    });

    for (var lang of this.allLanguages) {
      newFeature.translations!.push(
        new PlanFeatureTranslationDto({
          planFeatureId: newFeature.id,
          culture: lang.culture,
          title: '',
        }),
      );
    }

    this.features.push(newFeature);
  }

  save() {
    debugger;
    if (this.form()?.invalid) {
      this.notify.warning(this.l('CompleteFormFields'));
      return;
    }

    this.features.forEach((feature, index) => {
      feature.sortOrder = index + 1;
    });

    this.saving = true;
    const input = new CreateOrEditPlanFeatureCommand();
    input.items = this.features;
    this.subscriptionService
      .createOrEditPlanFeature(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        if (result.data) {
          this.notify.success(this.l('SavedSuccessfully'));
          this.dialogRef.close(true);
        }
      });
  }
}
