import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Inject, OnDestroy, ViewChild } from '@angular/core';
import { SliderSetting } from './slider-setting/SliderSetting';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder';
import { Subscription } from 'rxjs';
import { AppConst } from '@shared/app-const';
@Component({
  selector: 'blc-slider',
  templateUrl: './slider.component.html',
  styles: `
    :host {
      display: block;
    }
    .carousel,
    .carousel-inner {
      height: 100%;
    }
    .carousel-item {
      height: 100%;
      img {
        display: block;
        max-width: 100% !important;
        object-fit: cover;
        max-height: 100%;
        margin: 0 auto;
        height: 100%;
        width: 100%;
      }
    }
    .carousel-caption {
      background-color: #ffffffd4;
      background: linear-gradient(transparent, #ffffffd4);
      color: #000;
      width: 100%;
      left: 0;
      right: 0;
      bottom: 0;
      min-height: 50%;
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: flex-end;
      &.carousel-dark {
        background-color: #100f14d4;
        background: linear-gradient(transparent, #100f14d4);
        color: #fff;
      }
    }
    .carousel-indicators {
      margin-bottom: 0.2rem;
    }
    .carousel-indicators [data-bs-target] {
      width: 18px;
      height: 18px;
      border-radius: 50%;
      flex-shrink: 0;
      margin: 0 4px;
      border: none;
    }
  `,
  imports: [CommonModule],
})
export class SliderComponent implements AfterViewInit, OnDestroy {
  @ViewChild('myCarousel') myCarousel!: ElementRef<HTMLElement>;
  carouselId = 'carousel-' + crypto.randomUUID();

  settingChangeSubscription?: Subscription;

  settings = new SliderSetting();

  baseUrl = AppConst.fileServerUrl + '/';
  constructor(
    private chdr: ChangeDetectorRef,

    @Inject(COMPONENT_DATA) private context: ComponentDataContext<SliderSetting>,
  ) {
    this.settings = context.data ?? new SliderSetting();
    this.settingChangeSubscription = this.context.onChange.subscribe((data) => {
      this.settings = data;
      this.chdr.detectChanges();
    });
  }

  ngAfterViewInit(): void {
    // setTimeout(() => {
    //   const el = this.myCarousel.nativeElement;
    //   bootstrap.Carousel.getInstance(el)?.dispose();
    //   this.slider = new bootstrap.Carousel(el, {
    //     interval: 4000,
    //     wrap: true,
    //   });
    //   this.chdr.detectChanges();
    // });
  }

  ngOnDestroy() {
    // if (this.slider) {
    //   this.slider.dispose();
    // }
    if (this.settingChangeSubscription) {
      this.settingChangeSubscription.unsubscribe();
    }
  }
}
