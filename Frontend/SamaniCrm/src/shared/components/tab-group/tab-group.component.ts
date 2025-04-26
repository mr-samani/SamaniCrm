import { AfterContentInit, Component, ContentChildren, EventEmitter, Input, Output, QueryList } from '@angular/core';
import { TabItemComponent } from './tab-item/tab-item.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'tab-group',
  templateUrl: './tab-group.component.html',
  styleUrls: ['./tab-group.component.scss'],
})
export class TabGroupComponent implements AfterContentInit {
  @Input() showPlusButton = false;
  @Output() onAddTab = new EventEmitter();
  @Output() selectedIndex = new EventEmitter<number>();
  @ContentChildren(TabItemComponent) tabs?: QueryList<TabItemComponent>;
  rndId = Math.round(Math.random() * 5000);
  constructor() {}

  ngAfterContentInit() {
    setTimeout(() => {
      // get all active tabs
      let activeTabs = this.tabs?.filter((tab) => tab.active);
      // if there is no active tab set, activate the first
      if (activeTabs?.length === 0 && this.tabs) {
        this.selectTab(this.tabs.first, 0);
      }
    }, 10);
  }

  selectTab(tab: TabItemComponent, index: number) {
    if (!tab) {
      return;
    }
    // deactivate all tabs
    this.tabs?.toArray().forEach((tab) => (tab.active = false));
    // activate the tab the user has clicked on.
    tab.active = true;
    this.selectedIndex.emit(index);
  }

  onClickPlus() {
    this.onAddTab.emit();
  }
}
