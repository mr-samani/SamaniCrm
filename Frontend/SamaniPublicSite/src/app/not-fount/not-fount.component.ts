
import { CommonModule } from '@angular/common'
import { Component, Injector, OnInit } from '@angular/core';
import { BaseComponent } from '@app/base-components';

@Component({
  selector: 'app-not-fount',
  templateUrl: './not-fount.component.html',
  styleUrls: ['./not-fount.component.scss'],
  standalone: true,
  imports: [CommonModule],
}) 
export class NotFountComponent extends BaseComponent implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
