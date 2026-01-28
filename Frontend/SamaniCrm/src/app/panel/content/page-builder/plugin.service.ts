import { Injectable } from '@angular/core';
import { IPaginationPlugin, IPlugin, IPluginStore } from 'ngx-page-builder';

@Injectable({
  providedIn: 'root',
})
export class PluginService implements IPluginStore {
  private _plugins: IPlugin[] = [];
 
  // constructor() {
  //   debugger;
  //   const plugins = JSON.parse(localStorage.getItem('plugins') || '[]') || [];
  //   if (plugins.length > 0) this._plugins = plugins;
  // }

  getAllPlugins(take: number, skip: number, filter: string): Promise<IPaginationPlugin> {
    return new Promise<IPaginationPlugin>((resolve, reject) => {
      const result = this._plugins.filter((x) => x.name.includes(filter));
      const list = result.slice(skip, skip + take);
      resolve({
        items: list,
        total: result.length,
      });
    });
  }
  save(plugin: IPlugin): void {
    console.log('i get plugin', plugin);
    this._plugins.push(plugin);
    localStorage.setItem('plugins', JSON.stringify(this._plugins));
  }
}
