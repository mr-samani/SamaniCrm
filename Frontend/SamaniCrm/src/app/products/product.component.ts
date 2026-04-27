import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
  standalone: false,
})
export class ProductComponent extends AppComponentBase implements OnInit {
  constructor() {
    super();
    this.breadcrumb.list = [{ name: this.l('Products'), url: '/panel/products' }];
  }

  ngOnInit() {}
}
