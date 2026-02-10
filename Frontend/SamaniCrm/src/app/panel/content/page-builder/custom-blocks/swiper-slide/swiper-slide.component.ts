import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  Inject,
  OnDestroy,
  viewChild,
} from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { SwiperSlideSetting } from './swiper-slide-setting/SwiperSlideSetting';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder';
import { Subscription } from 'rxjs';
import Swiper from 'swiper';
import type { SwiperModule } from 'swiper/types';
import { AppConst } from '@shared/app-const';

// Import basic CSS
import 'swiper/css';

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
    this.settings = context.data ?? new SwiperSlideSetting();
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

      // Load required CSS
      await this.loadRequiredCSS();

      // Initialize Swiper
      this.swiper = new Swiper(container, {
        modules: modules,
        ...this.settings.toSwiperOptions(),
      });

      console.log('Swiper initialized with modules:', modules.map((m) => m.name));
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

  /**
   * Dynamically load only required CSS files
   */
  private async loadRequiredCSS(): Promise<void> {
    const cssFiles: string[] = [];

    // Add CSS based on enabled features
    if (this.settings.navigation && !this.loadedCSS.has('navigation')) {
      cssFiles.push('swiper/css/navigation');
      this.loadedCSS.add('navigation');
    }

    if (this.settings.pagination && !this.loadedCSS.has('pagination')) {
      cssFiles.push('swiper/css/pagination');
      this.loadedCSS.add('pagination');
    }

    if (this.settings.scrollbar && !this.loadedCSS.has('scrollbar')) {
      cssFiles.push('swiper/css/scrollbar');
      this.loadedCSS.add('scrollbar');
    }

    if (this.settings.zoom && !this.loadedCSS.has('zoom')) {
      cssFiles.push('swiper/css/zoom');
      this.loadedCSS.add('zoom');
    }

    if (this.settings.gridRows && this.settings.gridRows > 1 && !this.loadedCSS.has('grid')) {
      cssFiles.push('swiper/css/grid');
      this.loadedCSS.add('grid');
    }

    // Add effect CSS
    const effectKey = `effect-${this.settings.effect}`;
    if (this.settings.effect && this.settings.effect !== 'slide' && !this.loadedCSS.has(effectKey)) {
      cssFiles.push(`swiper/css/effect-${this.settings.effect}`);
      this.loadedCSS.add(effectKey);
    }

    // Import all required CSS
    try {
      await Promise.all(
        cssFiles.map((cssPath) =>
          import(
            /* webpackChunkName: "swiper-css" */
            `${cssPath}`
          ).catch((err) => console.error(`Failed to load CSS: ${cssPath}`, err)),
        ),
      );
    } catch (error) {
      console.error('Failed to load CSS files:', error);
    }
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
