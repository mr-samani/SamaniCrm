import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { FieldsType } from '@shared/components/table-view/fields-type.model';
import { SubscriptionServiceProxy } from '@shared/service-proxies/api/subscription.service';
import { DeletePlanCommand } from '@shared/service-proxies/model/delete-plan-command';
import { PlanDto } from '@shared/service-proxies/model/plan-dto';
import { finalize } from 'rxjs/operators';
import { CreateOrEditPlanComponent } from './create-or-edit/create-or-edit.component';
import { AddEditPlanFeaturesComponent } from './add-edit-plan-features/add-edit-plan-features.component';

@Component({
  standalone: false,
  selector: 'app-plans',
  templateUrl: './plans.component.html',
  styleUrls: ['./plans.component.scss'],
})
export class PlansComponent extends AppComponentBase implements OnInit {
  plans: PlanDto[] = [];
  fields: FieldsType[] = [
    { column: 'title', title: this.l('Title'), type: 'text' },
    { column: 'code', title: this.l('Code') },
    { column: 'description', title: this.l('Description') },
    { column: 'billingType', title: this.l('BillingType'), type: 'enum', localizeKey: 'BillingType_', width: 150 },
    { column: 'isActive', title: this.l('IsActive'), type: 'yesNo', width: 100 },
    { column: 'isPublic', title: this.l('IsPublic'), type: 'yesNo', width: 100 },
    { column: 'createdAt', title: this.l('CreatedAt'), type: 'dateTime', width: 150 },
  ];

  totalSize = 0;
  loading = true;

  subscriptionService = inject(SubscriptionServiceProxy);
  matDialog = inject(MatDialog);
  constructor() {
    super();
    this.breadcrumb.list = [
      { name: this.l('Subscription'), url: '/panel/subscription' },
      { name: this.l('Plans'), url: '/panel/subscription/plans' },
    ];
  }
  ngOnInit() {
    this.getData();
  }

  getData() {
    this.loading = true;
    this.subscriptionService
      .getAllPlans()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((response) => {
        this.plans = response.data ?? [];
        this.totalSize = 0;
      });
  }

  remove(item: PlanDto) {
    this.confirmMessage(`${this.l('Delete')}:${item.title}`, this.l('AreYouSureForDelete')).then((result) => {
      if (result.isConfirmed) {
        this.showMainLoading();
        this.subscriptionService
          .deletePlan(new DeletePlanCommand({ planId: item.id! }))
          .pipe(
            finalize(() => {
              this.hideMainLoading();
              this.chdr.detectChanges();
            }),
          )
          .subscribe((response) => {
            this.notify.success(this.l('DoneSuccessFully') + '(' + response.data + ')');
            this.getData();
          });
      }
    });
  }

  openCreateOrEditPlanDialog(item?: PlanDto) {
    this.matDialog
      .open(CreateOrEditPlanComponent, {
        data: item,
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getData();
        }
      });
  }

  openFeaturesDialog(item?: PlanDto) {
    this.matDialog
      .open(AddEditPlanFeaturesComponent, {
        data: item,
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getData();
        }
      });
  }
}
