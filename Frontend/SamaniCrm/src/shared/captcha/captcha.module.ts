import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CaptchaComponent } from './captcha.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [CaptchaComponent],
  imports: [
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    TranslateModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
  exports: [CaptchaComponent],
  providers: [],
})
export class CaptchaModule {}
