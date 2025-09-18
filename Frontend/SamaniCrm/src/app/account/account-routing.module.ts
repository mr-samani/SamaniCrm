import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountComponent } from './account.component';
import { accountResolver } from './account.resolver';
import { ExternalLoginCalbackComponent } from './external-login-calback/external-login-calback.component';

const routes: Routes = [
  {
    path: '',
    component: AccountComponent,
    resolve: { data: accountResolver },
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'external/:provider', component: ExternalLoginCalbackComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
