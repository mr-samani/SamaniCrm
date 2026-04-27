import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared/shared.module';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  standalone: true,
  imports: [SharedModule],
})
export class UserProfileComponent extends AppComponentBase implements OnInit {
  constructor() {
    super();

    this.breadcrumb.list = [{ name: this.l('UserProfile'), url: '/panel/user-profile' }];
  }

  ngOnInit() {}
}
