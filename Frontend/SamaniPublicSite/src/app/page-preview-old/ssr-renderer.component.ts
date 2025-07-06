import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { HeroBannerComponent } from './blocks/hero-banner/hero-banner.component';
import { ProductCategoryComponent } from './blocks/product-category/product-category.component';
 
@Component({
  selector: 'app-ssr-renderer',
  standalone: true,
  imports: [CommonModule, ProductCategoryComponent, HeroBannerComponent],
  template: `
    <ng-container *ngFor="let block of blocks">
      <block-product-category *ngIf="block.type === 'product-category'" [data]="block.data" />
      <block-hero-banner *ngIf="block.type === 'hero-banner'" [data]="block.data" />
    </ng-container>
  `
})
export class SsrRendererComponent {
  @Input() blocks: any[] = [];
}
