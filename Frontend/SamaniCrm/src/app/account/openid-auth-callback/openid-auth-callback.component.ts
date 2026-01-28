import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TokenService } from '@shared/services/token.service';

@Component({
  selector: 'app-openid-auth-callback',
  templateUrl: './openid-auth-callback.component.html',
  styleUrls: ['./openid-auth-callback.component.scss'],
  standalone: false,
})
export class OpenidAuthCallbackComponent extends AppComponentBase implements OnInit {
  constructor(tokenService: TokenService) {
    super();
    this.route.queryParams.subscribe((params) => {
      const token = params['token'];
      const refreshToken = params['refreshToken'];
      const error = params['error'];

      if (error) {
        console.error('External auth error:', error);
        this.router.navigate(['/login'], {
          queryParams: { error: 'احراز هویت خارجی ناموفق بود' },
        });
        return;
      }

      if (token && refreshToken) {
        // Save tokens
        tokenService.set({
          accessToken: token,
          refreshToken: refreshToken,
        });

        // Navigate to dashboard or home
        this.router.navigate(['/panel/dashboard']);
      } else {
        this.router.navigate(['/account/login'], {
          queryParams: { error: 'توکن دریافت نشد' },
        });
      }
    });
  }

  ngOnInit() {}
}
