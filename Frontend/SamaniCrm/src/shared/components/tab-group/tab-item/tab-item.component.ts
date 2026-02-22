import { ChangeDetectorRef, Component, input, Input, OnInit } from '@angular/core';

@Component({
  selector: 'm-tab-item',
  templateUrl: './tab-item.component.html',
  styleUrls: ['./tab-item.component.scss'],
  standalone: false,
})
export class TabItemComponent implements OnInit {
  @Input() title = '';

  active = false;
  public  setActive(val: boolean) {
    this.active = val;
    this.chdr.detectChanges();
  }
  constructor(private chdr: ChangeDetectorRef) {}

  ngOnInit(): void {}
}
