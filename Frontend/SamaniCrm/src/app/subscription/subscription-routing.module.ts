import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlansComponent } from './plans/plans.component';
import { SubscriptionComponent } from './subscription.component';

const routes: Routes = [
  { path: '', component: SubscriptionComponent },
  { path: 'plans', component: PlansComponent },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SubscriptionRoutingModule {}
