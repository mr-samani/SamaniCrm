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
import Swiper from 'swiper';
import { Navigation, Pagination } from 'swiper/modules';
import { AppConst } from '@shared/app-const';
import { BaseComponent } from '@app/base-components';
import { SwiperSlideSetting } from './SwiperSlideSetting';

@Component({
  selector: 'swiper-slide',
  templateUrl: './swiper-slide.component.html',
  styleUrls: ['./swiper-slide.component.scss'],
})
export class SwiperSlideComponent extends BaseComponent implements AfterViewInit, OnDestroy {
  settings = new SwiperSlideSetting();
  settingChangeSubscription?: Subscription;

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
    this.settingChangeSubscription = this.context.onChange.subscribe((data) => {
      this.settings = data;
      console.log(data);
      this.update();
    });
  }

  ngAfterViewInit() {
    this.update();
  }

  update() {
    const container = this.container()?.nativeElement;
    if (!container) return;

    this.swiper?.destroy(true, true);

    this.swiper = new Swiper(container, {
      // configure Swiper to use modules
      modules: [Navigation, Pagination],
      ...this.settings,

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
  }

  ngOnDestroy() {
    if (this.settingChangeSubscription) {
      this.settingChangeSubscription.unsubscribe();
    }
    this.swiper?.destroy();
  }

  next() {
    if (!this.swiper) return;
    this.swiper.slideNext();
    console.log(this.swiper.activeIndex);
  }
}
