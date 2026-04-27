import { Component,  input, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';

@Component({
  selector: 'sub-header',
  templateUrl: './sub-header.component.html',
  styleUrls: ['./sub-header.component.scss'],
  standalone: true,
})
export class SubHeaderComponent extends AppComponentBase implements OnInit {
  title = input<string>();
  constructor() {
    super();
  }

  ngOnInit() {}
}
