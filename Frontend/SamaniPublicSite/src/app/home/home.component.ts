import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AppConst } from '@shared/app-const';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule],
})
export class HomeComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
