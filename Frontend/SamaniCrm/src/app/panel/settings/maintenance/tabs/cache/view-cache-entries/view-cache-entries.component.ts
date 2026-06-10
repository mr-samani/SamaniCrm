import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@app/app-component-base';
import { CacheEntryDtoExtended } from '../cahce.component';
import { MaintenanceServiceProxy } from '@shared/service-proxies/api/maintenance.service';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-view-cache-entries',
  templateUrl: './view-cache-entries.component.html',
  styleUrl: './view-cache-entries.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ViewCacheEntriesComponent extends AppComponentBase {
  loading = true;
  data: string = '';
  editorOptions = {
    theme: 'vs-dark',
    language: 'json',
    readOnly: true,
  };
  item = inject<CacheEntryDtoExtended>(MAT_DIALOG_DATA);
  private readonly maintenanceService = inject(MaintenanceServiceProxy);

  constructor() {
    super();
    this.getData();
  }

  getData() {
    this.loading = true;
    this.maintenanceService
      .getCacheInfo(this.item.key)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.chdr.detectChanges();
        }),
      )
      .subscribe((response) => {
        this.data = JSON.stringify(response.data ?? {}, null, 2);
        // console.log(this.data);
      });
  }

  onEditorInit(editor: any) {
    setTimeout(() => {
      // console.log('getSupportedActions', editor.getSupportedActions());
      const formatAction = editor.getAction('editor.action.formatDocument');
      if (formatAction.isSupported()) {
        formatAction.run();
      }
    }, 100);
  }
}
