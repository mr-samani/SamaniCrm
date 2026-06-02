import { Component, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  standalone: false,
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss'],
})
export class SubscriptionComponent extends AppComponentBase implements OnInit {
  constructor() {
    super();
    this.breadcrumb.list = [{ name: this.l('Subscription'), url: '/panel/subscription' }];
  }

  ngOnInit() {}
}
