import { StyleShadowComponent } from './style-shadow/style-shadow.component';
import { NgModule } from '@angular/core';
import { StyleBorderComponent } from './style-border/style-border.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StyleSpacingComponent } from './style-spacing/style-spacing.component';

const comp = [StyleBorderComponent, StyleSpacingComponent, StyleShadowComponent];
@NgModule({
  declarations: [...comp],
  imports: [CommonModule, FormsModule],
  exports: [...comp],
})
export class StyleProperties {}
