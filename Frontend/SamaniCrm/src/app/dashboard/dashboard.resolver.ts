import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, ResolveFn, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@shared/services/auth.service';
 
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
