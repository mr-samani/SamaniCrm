import { Injectable } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { AppConst } from '@shared/app-const';
import { CreatePluginCommand, GetPluginQuery } from '@shared/service-proxies';
import { PageBuilderServiceProxy } from '@shared/service-proxies/api/page-builder.service';
import { IPaginationPlugin, IPlugin, IPluginStore } from 'ngx-page-builder';
import { finalize } from 'rxjs';

@Injectable()
export class PluginService extends AppComponentBase implements IPluginStore {
  loading = false;
  saving = false;
  constructor(private pageBuilderService: PageBuilderServiceProxy) {
    super();
  }

  getAllPlugins(take: number, skip: number, filter: string): Promise<IPaginationPlugin> {
    return new Promise<IPaginationPlugin>((resolve, reject) => {
      const input = new GetPluginQuery();
      input.filter = filter;
      input.pageNumber = skip + 1;
      input.pageSize = take;
      this.loading = true;
      this.pageBuilderService
        .getPlugins(input)
        .pipe(finalize(() => (this.loading = false)))
        .subscribe({
          next: (result) => {
            const items: IPlugin[] = (result.data?.items ?? []).map((m) => {
              return {
                name: m.name ?? '',
                image: m.image ? AppConst.fileServerUrl + '/' + m.image : '',
                plugin: m.data ?? '',
              };
            });
            const total = result.data?.pageSize ?? 0;
            resolve({
              items,
              total,
            });
          },
          error: (err) => {
            reject(err);
          },
        });
    });
  }
  save(plugin: IPlugin): void {
    this.saving = true;
    const input = new CreatePluginCommand();
    input.name = plugin.name;
    input.image = plugin.image;
    input.categoryName = 'Default';
    input.description = '';

    this.pageBuilderService
      .createPlugin(input)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: (result) => {
          this.notify.success(this.l('Message.SaveSuccessfully'));
        },
        error: (err) => {
          console.error(err);
        },
      });
  }
}
