import { CommonModule, NgComponentOutlet } from '@angular/common';
import { Component, ComponentRef, inject,  OnInit, Type } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AppComponentBase } from '@app/app-component-base';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';

import { SharedModule } from '@shared/shared.module';
import { finalize } from 'rxjs';
import { CreateDashboardComponent } from './create-dashboard/create-dashboard.component';
import { AddDashboardWidgetComponent } from './add-widget/add-widget.component';
import { Widget, WidgetHelper } from './widgets/widgets';
import { DashboardDto } from '@shared/service-proxies/model/dashboard-dto';
import { DasboardServiceProxy } from '@shared/service-proxies/api/dasboard.service';
import { UpdateDashboardItemsCommand } from '@shared/service-proxies/model/update-dashboard-items-command';
import { DeleteDashboardItemCommand } from '@shared/service-proxies/model/delete-dashboard-item-command';
import { DeleteDashboardCommand } from '@shared/service-proxies/model/delete-dashboard-command';
import { MatButtonModule } from '@angular/material/button';
import { cloneDeep } from 'lodash-es';
import { IGridLayoutOptions, NgxGridLayoutModule } from 'ngx-drag-drop-kit';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, NgxGridLayoutModule, TabGroupModule, MatProgressBarModule, MatButtonModule],
  providers: [DasboardServiceProxy],
})
export class DashboardComponent extends AppComponentBase implements OnInit {
  selectedIndex = -1;
  editMode = false;
  loading = false;
  loadingItems = false;
  dashboards: DashboardDto[] = [];
  dashboardItems: Widget[] = [];
  options: IGridLayoutOptions = {
    cols: 12,
    gap: 5,
    rowHeight: 35,
  };
  private dashboardService = inject(DasboardServiceProxy);
  private dialog = inject(MatDialog);
  constructor() {
    super();
  }

  ngOnInit() {
    this.getDashboards();
  }

  getDashboards() {
    this.loading = true;
    this.dashboardService
      .getAllDashboards()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe((result) => {
        this.dashboards = result.data ?? [];
        if (this.dashboards.length > 0 && this.selectedIndex < 0) {
          this.onChangeDashboard(0);
        }
      });
  }

  getItems(id: string) {
    this.loadingItems = true;
    this.dashboardItems = [];
    this.dashboardService
      .getAllDashboardItems(id)
      .pipe(finalize(() => (this.loadingItems = false)))
      .subscribe(async (result) => {
        this.dashboardItems = await WidgetHelper.loadWidgets(result.data ?? []);
        console.log(this.dashboardItems);
      });
  }

  onChangeDashboard(index: number) {
    if (this.selectedIndex == index) return;
    this.selectedIndex = index;
    this.getItems(this.dashboards[index].id!);
  }

  openDahboardDialog() {
    this.dialog
      .open(CreateDashboardComponent, {
        data: {},
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getDashboards();
        }
      });
  }

  openWidgetDialog(item?: Widget) {
    const activeDashboard = this.dashboards[this.selectedIndex];
    if (!activeDashboard || !activeDashboard.id) return;
    this.dialog
      .open(AddDashboardWidgetComponent, {
        data: {
          dashboardId: activeDashboard.id,
          item,
        },
        width: '768px',
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.getItems(activeDashboard.id!);
        }
      });
  }

  onChangeLayout(layouts: any[]) {
    console.log(layouts);
    if (layouts.length == 0) return;
    let changedList: Widget[] = [];
    for (let item of layouts) {
      let find = cloneDeep(this.dashboardItems.find((x) => x.id == item.id));
      if (!find) continue;
      let previous = find;
      find.position = JSON.stringify({
        x: item.x,
        y: item.y,
        w: item.w,
        h: item.h,
      });
      //if (!equals(find, previous)) {
      if (find.data && typeof find.data == 'object') {
        find.data = JSON.stringify(find.data);
      }
      changedList.push(find);
      // }
    }

    if (changedList.length == 0) return;

    // this.loadingItems = true;
    const input = new UpdateDashboardItemsCommand();
    input.list = changedList as any;
    this.dashboardService
      .updateDashboardItems(input)
      // .pipe(finalize(() => (this.loadingItems = false)))
      .subscribe((r) => {
        console.log(r);
        this.notify.success(this.l('SavedSuccessfully'));
      });
  }

  deleteWidget(item: Widget) {
    this.loadingItems = true;
    const input = new DeleteDashboardItemCommand();
    input.id = item.id;
    this.dashboardService
      .deleteDashboardItem(input)
      .pipe(finalize(() => (this.loadingItems = false)))
      .subscribe((r) => {
        console.log(r);
        const index = this.dashboardItems.findIndex((x) => x.id == item.id);
        this.dashboardItems.splice(index, 1);
        this.notify.success(this.l('DeletedSuccessfully'));
      });
  }
  editWidget(item: Widget, c: NgComponentOutlet) {
    let instance = c.componentInstance;
    instance?.edit();
  }
  deleteDashboard() {
    const dashboard = this.dashboards[this.selectedIndex];
    if (!dashboard) return;

    this.confirmMessage(dashboard.title, this.l('AreYouSureToDelete')).then((c) => {
      if (c.isConfirmed) {
        this.loadingItems = true;
        const input = new DeleteDashboardCommand();
        input.id = dashboard.id;
        this.dashboardService
          .deleteDashboard(input)
          .pipe(finalize(() => (this.loadingItems = false)))
          .subscribe((r) => {
            console.log(r);
            this.notify.success(this.l('DeletedSuccessfully'));
          });
      }
    });
  }
}
