import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'tab-item',
  templateUrl: './tab-item.component.html',
  styleUrls: ['./tab-item.component.scss'],
  standalone: false,
})
export class TabItemComponent implements OnInit {
  @Input() title = '';
  @Input() active = false;
  constructor() {}

  ngOnInit(): void {}
}
