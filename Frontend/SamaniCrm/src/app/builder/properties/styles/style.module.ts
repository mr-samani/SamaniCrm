import { StyleShadowComponent } from './style-shadow/style-shadow.component';
import { NgModule } from '@angular/core';
import { StyleBorderComponent } from './style-border/style-border.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StyleSpacingComponent } from './style-spacing/style-spacing.component';
import { StyleBackgroundComponent } from './style-background/style-background.component';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { NgxInputColorModule, NgxInputGradientModule } from 'ngx-input-color';
import { StyleDimensionsComponent } from './style-dimensions/style-dimensions.component';
import { InputStyleComponent } from './input-style/input-style.component';

const comp = [
  StyleBorderComponent,
  StyleSpacingComponent,
  StyleShadowComponent,
  StyleBackgroundComponent,
  StyleDimensionsComponent,
  InputStyleComponent,
];
@NgModule({
  declarations: [...comp],
  imports: [CommonModule, FormsModule, FileManagerModule, NgxInputColorModule, NgxInputGradientModule],
  exports: [...comp],
})
export class StyleProperties {}
