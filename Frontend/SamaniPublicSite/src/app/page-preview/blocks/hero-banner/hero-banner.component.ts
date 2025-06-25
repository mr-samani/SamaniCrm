
import { CommonModule } from '@angular/common'
import { Component, Injector, Input, OnInit } from '@angular/core';
import { BaseComponent } from '@app/base-components';

@Component({
  selector: 'block-hero-banner',
  templateUrl: './hero-banner.component.html',
  styleUrls: ['./hero-banner.component.scss'],
  standalone: true,
  imports: [CommonModule],
}) 
export class HeroBannerComponent extends BaseComponent implements OnInit {
  @Input() data!: { title: string; categories: string[] };

  
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
