import { NgModule } from '@angular/core';
import { TabGroupComponent } from './tab-group.component';
import { TabItemComponent } from './tab-item/tab-item.component';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [TabGroupComponent, TabItemComponent],
  imports: [CommonModule],
  exports: [TabGroupComponent, TabItemComponent],
})
export class TabGroupModule {}
