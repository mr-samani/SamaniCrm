import {
  ApplicationRef,
  ComponentRef,
  createComponent,
  Directive,
  ElementRef,
  EmbeddedViewRef,
  EnvironmentInjector,
  inject,
  Input,
  OnDestroy,
  Renderer2,
} from '@angular/core';
import { ContextMenuComponent } from './context-menu.component';
import { ContextMenuItem } from './context-menu.model';
import { ContextMenuService } from './context-menu.service';

@Directive({
  selector: '[contextMenu]',
  standalone: true,
  host: {
    '(contextmenu)': 'onContextMenu($event)',
  },
})
export class ContextMenuDirective implements OnDestroy {
  @Input('contextMenu') contextMenuList: ContextMenuItem[] = [];
  @Input('contextMenuTitle') contextMenuTitle?: string;

  private destroyListeners: (() => void)[] = [];

  private readonly appRef = inject(ApplicationRef);
  private readonly injector = inject(EnvironmentInjector);
  private readonly renderer = inject(Renderer2);
  private readonly host = inject(ElementRef);

  constructor(private ctxMenuService: ContextMenuService) {}

  onContextMenu(event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();

    this.destroyMenu();
    this.createMenu(event.clientX, event.clientY);
  }

  private createMenu(x: number, y: number): void {
    // Dynamically create the component
    this.ctxMenuService.menuRef = createComponent(ContextMenuComponent, {
      environmentInjector: this.injector,
    });
    this.ctxMenuService.menuRef.setInput('setItems', this.contextMenuList);
    this.ctxMenuService.menuRef.setInput('contextMenuTitle', this.contextMenuTitle);
    this.ctxMenuService.menuRef.setInput('x', x);
    this.ctxMenuService.menuRef.setInput('y', y);

    // Close on emit
    this.ctxMenuService.menuRef.instance.closed.subscribe(() => this.destroyMenu());

    // Attach to Angular's change detection
    this.appRef.attachView(this.ctxMenuService.menuRef.hostView);
    let el: HTMLElement = this.ctxMenuService.menuRef.location.nativeElement;
    // Append DOM node to body
    document.body.appendChild(el);

    el.popover = 'manual';
    el.showPopover();

    // Close on outside click
    const clickUnsub = this.renderer.listen('document', 'click', (e: MouseEvent) => {
      if (!this.ctxMenuService.menuRef?.location.nativeElement.contains(e.target as Node)) {
        this.destroyMenu();
      }
    });
    const ctxUnsub = this.renderer.listen('document', 'contextmenu', (e) => {
      if (!this.ctxMenuService.menuRef?.location.nativeElement.contains(e.target)) {
        this.destroyMenu();
      }
    });
    // and triggering the host directive to open a new menu
    const menuCtxUnsub = this.renderer.listen(
      this.ctxMenuService.menuRef.location.nativeElement,
      'contextmenu',
      (e: MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
      },
    );

    // Close on scroll
    const scrollUnsub = this.renderer.listen('window', 'scroll', () => this.destroyMenu());

    this.destroyListeners = [clickUnsub, ctxUnsub, menuCtxUnsub, scrollUnsub];
  }

  private destroyMenu(): void {
    if (!this.ctxMenuService.menuRef) return;
    const domElem = (this.ctxMenuService.menuRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement;

    this.destroyListeners.forEach((fn) => fn());
    this.destroyListeners = [];

    domElem.hidePopover();

    this.appRef.detachView(this.ctxMenuService.menuRef.hostView);
    this.ctxMenuService.menuRef.destroy();
    this.ctxMenuService.menuRef = null;
  }

  ngOnDestroy(): void {
    this.destroyMenu();
  }
}
