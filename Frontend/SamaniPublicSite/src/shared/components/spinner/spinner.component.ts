import { Component, Input, OnInit } from '@angular/core';

@Component({
  standalone: false,
  selector: 'spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss'],
})
export class SpinnerComponent implements OnInit {
  @Input() borderColor = '#056edb';
  @Input() borderBgColor = '#82b7ef';
  @Input() diameter = 26;
  @Input() borderSize = 2;

  constructor() {}

  ngOnInit(): void {}
}
