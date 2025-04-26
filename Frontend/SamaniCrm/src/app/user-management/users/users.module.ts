import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UsersRoutingModule } from './users-routing.module';
import { UserListComponent } from './list/list.component';

import { MaterialCommonModule } from '@shared/material/material.common.module';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { FilterComponent } from '@shared/components/filter/filter.component';
import { FilteFormrDirective } from '@shared/directives/filter-form.directive';
import { TableViewComponent } from '@shared/components/table-view/table-view.compoenent';
import { FileManagerModule } from '@app/file-manager/file-manager.module';

@NgModule({
  declarations: [UserListComponent],
  imports: [
    CommonModule,
    UsersRoutingModule,
    TableViewComponent,
    FilterComponent,
    FilteFormrDirective,
    PaginationComponent,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    FileManagerModule,
    MaterialCommonModule,
  ],
  providers: [],
})
export class UsersModule {}
