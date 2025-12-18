import { CommonModule } from '@angular/common';
import { Component, inject, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AppComponentBase } from '@app/app-component-base';
import { TabGroupModule } from '@shared/components/tab-group/tab-group.module';
import { DasboardServiceProxy, DashboardDto, DashboardItemDto } from '@shared/service-proxies';
import { SharedModule } from '@shared/shared.module';
import { GridLayoutModule, IGridLayoutOptions } from 'ngx-drag-drop-kit';
import { finalize } from 'rxjs';
import { CreateDashboardComponent } from './create-dashboard/create-dashboard.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule, GridLayoutModule, TabGroupModule, MatProgressBarModule],
  providers: [DasboardServiceProxy],
})
export class DashboardComponent extends AppComponentBase implements OnInit {
  selectedIndex = -1;
  editMode = false;
  loading = false;
  loadingItems = false;
  dashboards: DashboardDto[] = [];
  dashboardItems: DashboardItemDto[] = [];
  options: IGridLayoutOptions = {
    cols: 20,
    gap: 5,
    rowHeight: 'fit',
  };
  private dashboardService = inject(DasboardServiceProxy);
  private dialog = inject(MatDialog);
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}

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
      .subscribe((result) => {
        this.dashboardItems = result.data ?? [];
      });
  }

  onChangeDashboard(index: number) {
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
}
