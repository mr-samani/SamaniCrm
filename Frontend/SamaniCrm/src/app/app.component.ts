import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SyncHistoryManagerServiceProxy } from '@shared/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SamaniCrm';


  
  constructor(
    private syncHistoryManagerService: SyncHistoryManagerServiceProxy,
) {
}

getAll() {
    


    this.syncHistoryManagerService
        .getAll(
            this.syncChainFilter,
            this.user ? +this.user.id : undefined,
            this.tenant ? this.tenant.id : undefined,
            this.deviceManufacturerFilter,
            this.deviceModelFilter,
            this.versionOSFilter,
            this.sdkNumberFilter,
            undefined,
            undefined,
            this.syncGuidFilter,
            undefined,
            this.hasException,
            this._dateTimeService.getStartOfDayForDate(this.dateRange[0]),
            this._dateTimeService.getEndOfDayForDate(this.dateRange[1]),
            this.primengTableHelper.getSorting(this.dataTable),
            this.primengTableHelper.getMaxResultCount(this.paginator, event),
            this.primengTableHelper.getSkipCount(this.paginator, event),
        )
        .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
        .subscribe((result) => {
            this.primengTableHelper.totalRecordsCount = result.totalCount;
            this.primengTableHelper.records = result.items;
            this.primengTableHelper.hideLoadingIndicator();
        });
}


}
