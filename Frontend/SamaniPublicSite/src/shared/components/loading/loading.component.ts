import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../../shared.module';

@Component({
  selector: 'loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss'],
  standalone: false,
})
export class LoadingComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
