import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  Injector,
  OnDestroy,
  OnInit,
  viewChild,
} from '@angular/core';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder/core';
import { Subscription } from 'rxjs';
// import { Navigation, Pagination } from 'swiper/modules';
import { AppConst } from '@shared/app-const';
import { BaseComponent } from '@app/base-components';
import { SwiperSlideSetting } from './SwiperSlideSetting';
import { EffectCards } from 'swiper/modules';
import Swiper from 'swiper';
import 'swiper/css';
import 'swiper/css/effect-cards';
// import 'swiper/css/navigation';
// import 'swiper/css/pagination';
@Component({
  selector: 'swiper-slide',
  templateUrl: './swiper-slide.component.html',
  styleUrls: ['./swiper-slide.component.scss'],
})
export class SwiperSlideComponent extends BaseComponent implements AfterViewInit, OnDestroy {
  settings = new SwiperSlideSetting();

  container = viewChild<ElementRef<HTMLDivElement>>('container');

  swiper?: Swiper;
  baseUrl = AppConst.fileServerUrl + '/';

  constructor(
    private chdr: ChangeDetectorRef,
    injector: Injector,
    @Inject(COMPONENT_DATA) private context: ComponentDataContext<SwiperSlideSetting>
  ) {
    super(injector);
    this.settings = context.data ?? new SwiperSlideSetting();
  }

  ngAfterViewInit() {
    this.update();
  }

  update() {
    this.chdr.detectChanges();
    setTimeout(() => {
      const container = this.container()?.nativeElement;
      if (!container) return;

      this.swiper?.destroy(true, true);
      this.swiper = new Swiper(container, {
        effect: 'cards',
        // configure Swiper to use modules
        grabCursor: true,
        modules: [
          // Navigation, Pagination,
          EffectCards,
        ],
        // ...this.settings,

        // // If we need pagination
        // pagination: {
        //   el: '.swiper-pagination',
        // },

        // // Navigation arrows
        // navigation: {
        //   nextEl: '.swiper-button-next',
        //   prevEl: '.swiper-button-prev',
        // },

        // // And if we need scrollbar
        // scrollbar: {
        //   el: '.swiper-scrollbar',
        // },
      });
      console.log(this.swiper);
    }, 100);
  }

  ngOnDestroy() {
    this.swiper?.destroy();
  }

  next() {
    if (!this.swiper) return;
    this.swiper.slideNext();
    console.log(this.swiper.activeIndex);
  }
}
