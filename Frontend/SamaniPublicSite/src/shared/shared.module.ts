import { NgModule } from '@angular/core';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { LoadingComponent } from './components/loading/loading.component';
const components = [SpinnerComponent, LoadingComponent];
@NgModule({
  declarations: [...components],
  exports: [...components],
})
export class SharedModule {}
