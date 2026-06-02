import { AfterViewInit, ChangeDetectorRef, Component, Input, TemplateRef, ViewChild } from '@angular/core';

@Component({
  selector: 'm-tab-item',
  templateUrl: './tab-item.component.html',
  styleUrls: ['./tab-item.component.scss'],
  standalone: false,
})
export class TabItemComponent implements AfterViewInit {
  @Input() title = '';

  @ViewChild('tabContentContainer', { static: true })
  template!: TemplateRef<any>;

  active = false;
  public setActive(val: boolean) {
    this.active = val;
    this.chdr.detectChanges();
  }
  constructor(private chdr: ChangeDetectorRef) {}

  ngAfterViewInit(): void {
  }
}
