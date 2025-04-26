import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, Router, RouterStateSnapshot } from '@angular/router';
import { AppConst } from 'src/shared/app-const';
import { AuthService } from 'src/shared/services/auth.service';

export const dashboardResolver: ResolveFn<boolean> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
  authService: AuthService = inject(AuthService),
  router: Router = inject(Router),
) => {
  return authService
    .getCurrentUserValue()
    .then((r) => {
      return true;
    })
    .catch((er) => {
      router.navigate(['/account/login']);
      return false;
    });
};
