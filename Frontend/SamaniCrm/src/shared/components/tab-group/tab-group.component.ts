import {
  AfterContentInit,
  AfterViewInit,
  Component,
  ContentChildren,
  DOCUMENT,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  Output,
  QueryList,
  viewChild,
  ViewEncapsulation,
} from '@angular/core';
import { TabItemComponent } from './tab-item/tab-item.component';

@Component({
  selector: 'm-tab-group',
  templateUrl: './tab-group.component.html',
  styleUrls: ['./tab-group.component.scss'],
  standalone: false,
  encapsulation: ViewEncapsulation.None,
})
export class TabGroupComponent implements OnDestroy, AfterViewInit {
  @Input() showPlusButton = false;
  @Input() color = '#fff';
  @Output() onAddTab = new EventEmitter();

  @Input('selectedIndex') set setIndex(val: number) {
    if (this.tabs && this.tabs.get(val) != undefined) {
      this.selectTab(this.tabs.get(val)!, val);
    }
  }
  @Output() selectedIndexChange = new EventEmitter<number>();
  @ContentChildren(TabItemComponent) tabs = new QueryList<TabItemComponent>();
  tabGroupContainer = viewChild<ElementRef<HTMLElement>>('tabGroupContainer');
  rndId = Math.round(Math.random() * 5000);
  private index = -1;

  protected doc = inject(DOCUMENT);
  constructor() {}

  ngAfterViewInit(): void {
    setTimeout(() => {
      let activeTabs = this.tabs?.filter((tab) => tab.active);
      if (this.index < 0 && activeTabs?.length === 0 && this.tabs) {
        this.selectTab(this.tabs.first, 0);
      }
    }, 100);

    this.tabGroupContainer()?.nativeElement.style.setProperty('--oc-tab-bg', this.color);
  }
  ngOnDestroy(): void {}

  selectTab(tab: TabItemComponent, index: number) {
    if (!tab) {
      return;
    }
    // deactivate all tabs
    this.tabs?.toArray().forEach((tab) => (tab.active = false));
    // activate the tab the user has clicked on.
    tab.active = true;
    this.selectedIndexChange.emit(index);
  }

  onClickPlus() {
    this.onAddTab.emit();
  }
}
