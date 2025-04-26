import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateChild } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate, CanActivateChild {
  constructor(
    private router: Router,
    private authService: AuthService,
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const rootUrl = route.url[0].path;
    return this.chechGuard(rootUrl, state);
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const rootUrl = route.parent?.url[0].path;
    return this.chechGuard(rootUrl, state);
  }

  chechGuard(rootUrl: string | undefined, state: RouterStateSnapshot) {
    const user = this.authService.currentUserValue;
    return true;
    if (user) {
      return true;
    }

    // this.toastrService.error('شما اجازه دسترسی به این صفحه را ندارید!');
    //// not logged in so redirect to login page with the return url
    // this.router.navigate(['/503'], { queryParams: { returnUrl: state.url } });
    this.router.navigate(['/account/login']);

    return false;
  }
}
