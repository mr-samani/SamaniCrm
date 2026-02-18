import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, Inject, OnDestroy, viewChild } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { SwiperSlideSetting } from './SwiperSlideSetting';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder/core';
import { Subscription } from 'rxjs';
import Swiper from 'swiper';
import type { SwiperModule, SwiperOptions } from 'swiper/types';
import { AppConst } from '@shared/app-const';

// Import basic CSS
// import 'swiper/css';
// import 'swiper/css/navigation';
// import 'swiper/css/pagination';
// import 'swiper/css/scrollbar';
// import 'swiper/css/effect-fade';
// import 'swiper/css/effect-cube';
// import 'swiper/css/effect-coverflow';
// import 'swiper/css/effect-flip';
// import 'swiper/css/effect-creative';
// import 'swiper/css/effect-cards';
// import 'swiper/css/zoom';
// import 'swiper/css/grid';

@Component({
  selector: 'swiper-slide',
  templateUrl: './swiper-slide.component.html',
  styleUrls: ['./swiper-slide.component.scss'],
})
export class SwiperSlideComponent extends AppComponentBase implements AfterViewInit, OnDestroy {
  settings = new SwiperSlideSetting();
  settingChangeSubscription?: Subscription;

  container = viewChild<ElementRef<HTMLDivElement>>('container');

  swiper?: Swiper;
  baseUrl = AppConst.fileServerUrl + '/';

  // Track loaded CSS to avoid duplicate imports
  private loadedCSS = new Set<string>();

  constructor(
    private chdr: ChangeDetectorRef,
    @Inject(COMPONENT_DATA) private context: ComponentDataContext<SwiperSlideSetting>,
  ) {
    super();
    this.settings = Object.assign(new SwiperSlideSetting(), this.context.data);
    this.settingChangeSubscription = this.context.onChange.subscribe((data) => {
      this.settings = data;
      this.update();
    });
  }

  ngAfterViewInit() {
    this.chdr.detectChanges();
    this.update();
  }

  async update() {
    const container = this.container()?.nativeElement;
    if (!container) return;
    // Check if slides exist
    const slides = container.querySelectorAll('.swiper-slide');
    if (slides.length === 0) {
      console.warn('No slides found');
      return;
    }

    this.swiper?.destroy(true, true);

    try {
      // Load required modules dynamically
      const modules = await this.loadRequiredModules();
      const opt: SwiperOptions = {
        modules: modules,
        ...this.settings.toSwiperOptions(),
      };
      console.log(opt);
      // Initialize Swiper
      this.swiper = new Swiper(container, opt);

      console.log(
        'Swiper initialized with modules:',
        modules.map((m) => m.name),
      );
    } catch (error) {
      console.error('Failed to initialize Swiper:', error);
    }
  }

  /**
   * Dynamically load only required Swiper modules
   */
  private async loadRequiredModules(): Promise<SwiperModule[]> {
    const requiredModules = this.settings.getRequiredModules();
    const modules: SwiperModule[] = [];

    const modulePromises = requiredModules.map(async (moduleName) => {
      try {
        switch (moduleName) {
          case 'Navigation':
            const { Navigation } = await import('swiper/modules');
            return Navigation;
          case 'Pagination':
            const { Pagination } = await import('swiper/modules');
            return Pagination;
          case 'Scrollbar':
            const { Scrollbar } = await import('swiper/modules');
            return Scrollbar;
          case 'Autoplay':
            const { Autoplay } = await import('swiper/modules');
            return Autoplay;
          case 'Keyboard':
            const { Keyboard } = await import('swiper/modules');
            return Keyboard;
          case 'Mousewheel':
            const { Mousewheel } = await import('swiper/modules');
            return Mousewheel;
          case 'Zoom':
            const { Zoom } = await import('swiper/modules');
            return Zoom;
          case 'Thumbs':
            const { Thumbs } = await import('swiper/modules');
            return Thumbs;
          case 'FreeMode':
            const { FreeMode } = await import('swiper/modules');
            return FreeMode;
          case 'Grid':
            const { Grid } = await import('swiper/modules');
            return Grid;
          case 'EffectFade':
            const { EffectFade } = await import('swiper/modules');
            return EffectFade;
          case 'EffectCube':
            const { EffectCube } = await import('swiper/modules');
            return EffectCube;
          case 'EffectCoverflow':
            const { EffectCoverflow } = await import('swiper/modules');
            return EffectCoverflow;
          case 'EffectFlip':
            const { EffectFlip } = await import('swiper/modules');
            return EffectFlip;
          case 'EffectCreative':
            const { EffectCreative } = await import('swiper/modules');
            return EffectCreative;
          case 'EffectCards':
            const { EffectCards } = await import('swiper/modules');
            return EffectCards;
          default:
            console.warn(`Unknown module: ${moduleName}`);
            return null;
        }
      } catch (error) {
        console.error(`Failed to load module ${moduleName}:`, error);
        return null;
      }
    });

    const loadedModules = await Promise.all(modulePromises);
    return loadedModules.filter((m): m is SwiperModule => m !== null);
  }

  ngOnDestroy() {
    if (this.settingChangeSubscription) {
      this.settingChangeSubscription.unsubscribe();
    }
    this.swiper?.destroy(true, true);
  }

  // Navigation methods (can be used externally)
  next() {
    if (!this.swiper) return;
    this.swiper.slideNext();
  }

  prev() {
    if (!this.swiper) return;
    this.swiper.slidePrev();
  }

  slideTo(index: number) {
    if (!this.swiper) return;
    this.swiper.slideTo(index);
  }
}
