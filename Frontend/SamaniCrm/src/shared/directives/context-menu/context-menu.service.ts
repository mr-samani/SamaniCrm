import { ComponentRef, Injectable } from '@angular/core';
import { ContextMenuComponent } from './context-menu.component';

@Injectable({
  providedIn: 'root',
})
export class ContextMenuService {
  public menuRef: ComponentRef<ContextMenuComponent> | null = null;

  constructor() {}
}
