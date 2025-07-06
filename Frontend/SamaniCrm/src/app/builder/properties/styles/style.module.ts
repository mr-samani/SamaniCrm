import { NgModule } from '@angular/core';
import { StyleBorderComponent } from './style-border/style-border.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

const comp = [StyleBorderComponent];
@NgModule({
  declarations: [...comp],
  imports: [CommonModule, FormsModule],
  exports: [...comp],
})
export class StyleProperties {}
