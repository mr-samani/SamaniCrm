import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { TableViewModule } from '@shared/components/table-view/table-view.module';
import { MaterialCommonModule } from '@shared/material/material.common.module';
import { SharedModule } from '@shared/shared.module';
import { SubscriptionServiceProxy } from '@shared/service-proxies/api/subscription.service';
import { PlansComponent } from './plans/plans.component';
import { SubscriptionComponent } from './subscription.component';
import { SubscriptionRoutingModule } from './subscription-routing.module';
import { CreateOrEditPlanComponent } from './plans/create-or-edit/create-or-edit.component';
import { AddEditPlanFeaturesComponent } from './plans/add-edit-plan-features/add-edit-plan-features.component';
import{MatTableModule} from '@angular/material/table';
import{MatTabsModule} from '@angular/material/tabs';


@NgModule({
  declarations: [SubscriptionComponent, PlansComponent, CreateOrEditPlanComponent, AddEditPlanFeaturesComponent],
  imports: [
    CommonModule,
    SubscriptionRoutingModule,
    TableViewModule,
    FilterComponent,
    PaginationComponent,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    FileManagerModule,
    MaterialCommonModule,
    TabGroupModule,
    SharedModule,
    MatTableModule,
    MatTabsModule
  ],
  providers: [SubscriptionServiceProxy],
})
export class SubscriptionModule {}
