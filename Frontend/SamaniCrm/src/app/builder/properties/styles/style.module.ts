import { NgModule } from '@angular/core';
import { StyleBorderComponent } from './style-border/style-border.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StyleSpacingComponent } from './style-spacing/style-spacing.component';

const comp = [StyleBorderComponent, StyleSpacingComponent];
@NgModule({
  declarations: [...comp],
  imports: [CommonModule, FormsModule],
  exports: [...comp],
})
export class StyleProperties {}
