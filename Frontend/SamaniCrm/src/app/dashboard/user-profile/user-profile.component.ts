import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  standalone: true,
  imports: [CommonModule, TranslateModule],
})
export class UserProfileComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);

    this.breadcrumb.list = [{ name: this.l('UserProfile'), url: '/dashboard/user-profile' }];
  }

  ngOnInit() {}
}
