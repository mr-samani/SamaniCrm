import { Component, OnInit } from '@angular/core';
import {
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router,
  RouterOutlet,
} from '@angular/router';
import { AccountServiceProxy } from '../shared/service-proxies';
import { MainSpinnerService } from '../shared/services/main-spinner.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers: [AccountServiceProxy],
})
export class AppComponent implements OnInit {
  constructor(
    router: Router,
    public mainSpinnerService: MainSpinnerService,
  ) {
    router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        const url = new URL(event.url, window.location.origin);

        const currentPath = location.pathname;
        const nextPath = url.pathname;

        if (currentPath !== nextPath) {
          mainSpinnerService.showLoading = true;
        }
      }
      if (event instanceof NavigationCancel || event instanceof NavigationError) {
        mainSpinnerService.showLoading = false;
      }
      if (event instanceof NavigationEnd) {
        mainSpinnerService.showLoading = false;
      }
    });
  }

  ngOnInit(): void {}
}
