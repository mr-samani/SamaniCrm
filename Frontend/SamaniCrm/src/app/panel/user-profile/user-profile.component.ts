import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule],
})
export class UserProfileComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);

    this.breadcrumb.list = [{ name: this.l('UserProfile'), url: '/panel/user-profile' }];
  }

  ngOnInit() {}
}
