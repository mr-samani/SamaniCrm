import { SwiperOptions } from 'swiper/types';

export type SwiperPresetType =
  | 'default'
  | 'cards'
  | 'stories'
  | 'products'
  | 'coverflow'
  | 'cube'
  | 'creative'
  | 'custom';

export type SwiperBreakpointItem = {
  width: number;
  slidesPerView?: number | 'auto';
  spaceBetween?: number | string;
};

export class SwiperSlideSetting {
  // Preset Selection
  preset: SwiperPresetType = 'default';

  // Slides Data
  sliders: SwiperSlideContext[] = [];

  // Basic Settings
  direction: 'horizontal' | 'vertical' = 'horizontal';
  speed?: number = 300;
  effect?: 'slide' | 'fade' | 'cube' | 'coverflow' | 'flip' | 'creative' | 'cards' | (string & {}) = 'slide';
  spaceBetween?: number | string = 10;
  slidesPerView?: number | 'auto' = 1;

  // Advanced Settings
  centeredSlides?: boolean = false;
  loop?: boolean = false;
  rewind?: boolean = true;
  grabCursor?: boolean = true;
  slideToClickedSlide?: boolean = false;
  watchSlidesProgress?: boolean = false;
  setWrapperSize?: boolean = false;
  slidesOffsetBefore?: number = 0;
  slidesOffsetAfter?: number = 0;

  // Navigation & Controls
  navigation?: boolean = false;
  pagination?: boolean = false;
  paginationType?: 'bullets' | 'fraction' | 'progressbar' = 'bullets';
  scrollbar?: boolean = false;

  // Autoplay
  autoplay?: boolean = false;
  autoplayDelay?: number = 3000;
  autoplayDisableOnInteraction?: boolean = true;
  autoplayPauseOnMouseEnter?: boolean = true;

  // Advanced Features
  lazyLoading?: boolean = false;
  keyboard?: boolean = false;
  mousewheel?: boolean = false;
  zoom?: boolean = false;
  freeMode?: boolean = false;
  thumbs?: boolean = false;

  // Grid (for products showcase)
  gridRows?: number = 1;
  gridFill?: 'row' | 'column' = 'row';

  // Creative Effect Options (only used when effect is 'creative')
  creativeEffectPrev?: {
    translate?: [number, number, number];
    rotate?: [number, number, number];
    opacity?: number;
    scale?: number;
  };
  creativeEffectNext?: {
    translate?: [number | string, number | string, number | string];
    rotate?: [number, number, number];
    opacity?: number;
    scale?: number;
  };

  // Breakpoints
  breakpointsList: SwiperBreakpointItem[] = [];
  breakpoints?: {
    [width: number]: SwiperOptions;
    [ratio: string]: SwiperOptions;
  };

  constructor() {
    this.sliders = Array.from({ length: 5 }).map(() => new SwiperSlideContext());
    this.applyPreset('default');
  }

  /**
   * Apply a preset configuration
   */
  applyPreset(preset: SwiperPresetType) {
    this.preset = preset;

    switch (preset) {
      case 'cards':
        this.applyCardsPreset();
        break;
      case 'stories':
        this.applyStoriesPreset();
        break;
      case 'products':
        this.applyProductsPreset();
        break;
      case 'coverflow':
        this.applyCoverflowPreset();
        break;
      case 'cube':
        this.applyCubePreset();
        break;
      case 'creative':
        this.applyCreativePreset();
        break;
      case 'default':
      default:
        this.applyDefaultPreset();
        break;
    }
  }

  private applyDefaultPreset() {
    this.effect = 'slide';
    this.slidesPerView = 1;
    this.spaceBetween = 10;
    this.navigation = true;
    this.pagination = true;
    this.paginationType = 'bullets';
    this.loop = true;
    this.grabCursor = true;
    this.autoplay = false;
    this.centeredSlides = false;
    this.breakpointsList = [
      { width: 640, slidesPerView: 2, spaceBetween: 20 },
      { width: 1024, slidesPerView: 3, spaceBetween: 30 },
    ];
  }

  private applyCardsPreset() {
    this.effect = 'cards';
    this.slidesPerView = 1;
    this.spaceBetween = 0;
    this.navigation = false;
    this.pagination = false;
    this.loop = false;
    this.grabCursor = true;
    this.centeredSlides = true;
    this.breakpointsList = [];
  }

  private applyStoriesPreset() {
    this.effect = 'slide';
    this.slidesPerView = 'auto';
    this.spaceBetween = 10;
    this.navigation = false;
    this.pagination = false;
    this.centeredSlides = false;
    this.freeMode = true;
    this.grabCursor = true;
    this.loop = false;
    this.breakpointsList = [];
  }

  private applyProductsPreset() {
    this.effect = 'slide';
    this.slidesPerView = 2;
    this.spaceBetween = 15;
    this.navigation = true;
    this.pagination = false;
    this.loop = true;
    this.grabCursor = true;
    this.gridRows = 2;
    this.gridFill = 'row';
    this.breakpointsList = [
      { width: 640, slidesPerView: 3, spaceBetween: 20 },
      { width: 1024, slidesPerView: 4, spaceBetween: 25 },
      { width: 1280, slidesPerView: 5, spaceBetween: 30 },
    ];
  }

  private applyCoverflowPreset() {
    this.effect = 'coverflow';
    this.slidesPerView = 3;
    this.spaceBetween = 30;
    this.navigation = true;
    this.pagination = true;
    this.paginationType = 'bullets';
    this.loop = true;
    this.grabCursor = true;
    this.centeredSlides = true;
    this.breakpointsList = [
      { width: 640, slidesPerView: 2 },
      { width: 1024, slidesPerView: 3 },
    ];
  }

  private applyCubePreset() {
    this.effect = 'cube';
    this.slidesPerView = 1;
    this.spaceBetween = 0;
    this.navigation = true;
    this.pagination = true;
    this.paginationType = 'fraction';
    this.loop = false;
    this.grabCursor = true;
    this.breakpointsList = [];
  }

  private applyCreativePreset() {
    this.effect = 'creative';
    this.slidesPerView = 1;
    this.spaceBetween = 0;
    this.navigation = true;
    this.pagination = true;
    this.loop = true;
    this.grabCursor = true;
    this.centeredSlides = true;
    this.creativeEffectPrev = {
      translate: [0, 0, -400],
      rotate: [0, 0, 0],
      opacity: 0.5,
      scale: 0.8,
    };
    this.creativeEffectNext = {
      translate: ['100%', 0, 0],
    };
    this.breakpointsList = [];
  }

  normalizeBreakpoints() {
    if (this.breakpointsList?.length) return;

    if (!this.breakpoints) {
      this.breakpointsList = [];
      return;
    }

    this.breakpointsList = Object.entries(this.breakpoints)
      .map(([key, value]) => ({
        width: Number(key),
        slidesPerView: value.slidesPerView,
        spaceBetween: value.spaceBetween,
      }))
      .filter((item) => Number.isFinite(item.width))
      .sort((a, b) => a.width - b.width);
  }

  buildBreakpoints(): SwiperOptions['breakpoints'] {
    const result: { [width: number]: SwiperOptions } = {};
    for (const item of this.breakpointsList || []) {
      if (!Number.isFinite(item.width)) continue;
      result[item.width] = {
        slidesPerView: item.slidesPerView,
        spaceBetween: item.spaceBetween,
      };
    }
    return result;
  }

  toSwiperOptions(): SwiperOptions {
    const options: SwiperOptions = {
      direction: this.direction,
      speed: this.speed,
      effect: this.effect,
      spaceBetween: this.spaceBetween,
      slidesPerView: this.slidesPerView,
      centeredSlides: this.centeredSlides,
      loop: this.loop,
      rewind: this.rewind,
      grabCursor: this.grabCursor,
      slideToClickedSlide: this.slideToClickedSlide,
      watchSlidesProgress: this.watchSlidesProgress,
      setWrapperSize: this.setWrapperSize,
      slidesOffsetBefore: this.slidesOffsetBefore,
      slidesOffsetAfter: this.slidesOffsetAfter,
      breakpoints: this.buildBreakpoints(),
    };

    // Navigation
    if (this.navigation) {
      options.navigation = {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
      };
    }

    // Pagination
    if (this.pagination) {
      options.pagination = {
        el: '.swiper-pagination',
        type: this.paginationType,
        clickable: true,
      };
    }

    // Scrollbar
    if (this.scrollbar) {
      options.scrollbar = {
        el: '.swiper-scrollbar',
        draggable: true,
      };
    }

    // Autoplay
    if (this.autoplay) {
      options.autoplay = {
        delay: this.autoplayDelay,
        disableOnInteraction: this.autoplayDisableOnInteraction,
        pauseOnMouseEnter: this.autoplayPauseOnMouseEnter,
      };
    }

    // Lazy Loading
    if (this.lazyLoading) {
      // TODO: Property 'lazy' does not exist on type 'SwiperOptions'.ts(2339)
      // options.lazy = {
      //   loadPrevNext: true,
      // };
    }

    // Keyboard
    if (this.keyboard) {
      options.keyboard = {
        enabled: true,
      };
    }

    // Mousewheel
    if (this.mousewheel) {
      options.mousewheel = {
        forceToAxis: true,
      };
    }

    // Zoom
    if (this.zoom) {
      options.zoom = {
        maxRatio: 3,
      };
    }

    // Free Mode
    if (this.freeMode) {
      options.freeMode = {
        enabled: true,
        sticky: true,
      };
    }

    // Grid
    if (this.gridRows && this.gridRows > 1) {
      options.grid = {
        rows: this.gridRows,
        fill: this.gridFill,
      };
    }

    // Creative Effect
    if (this.effect === 'creative' && (this.creativeEffectPrev || this.creativeEffectNext)) {
      options.creativeEffect = {
        prev: this.creativeEffectPrev,
        next: this.creativeEffectNext,
      };
    }

    // Coverflow Effect
    if (this.effect === 'coverflow') {
      options.coverflowEffect = {
        rotate: 50,
        stretch: 0,
        depth: 100,
        modifier: 1,
        slideShadows: true,
      };
    }

    // Cube Effect
    if (this.effect === 'cube') {
      options.cubeEffect = {
        shadow: true,
        slideShadows: true,
        shadowOffset: 20,
        shadowScale: 0.94,
      };
    }

    return options;
  }

  /**
   * Get required modules based on settings
   */
  getRequiredModules(): string[] {
    const modules: string[] = [];

    if (this.navigation) modules.push('Navigation');
    if (this.pagination) modules.push('Pagination');
    if (this.scrollbar) modules.push('Scrollbar');
    if (this.autoplay) modules.push('Autoplay');
    if (this.keyboard) modules.push('Keyboard');
    if (this.mousewheel) modules.push('Mousewheel');
    if (this.zoom) modules.push('Zoom');
    if (this.thumbs) modules.push('Thumbs');
    if (this.freeMode) modules.push('FreeMode');
    if (this.gridRows && this.gridRows > 1) modules.push('Grid');

    // Effect modules
    switch (this.effect) {
      case 'fade':
        modules.push('EffectFade');
        break;
      case 'cube':
        modules.push('EffectCube');
        break;
      case 'coverflow':
        modules.push('EffectCoverflow');
        break;
      case 'flip':
        modules.push('EffectFlip');
        break;
      case 'creative':
        modules.push('EffectCreative');
        break;
      case 'cards':
        modules.push('EffectCards');
        break;
    }

    return modules;
  }
}

export class SwiperSlideContext {
  image: string = '';
  title?: string = '';
  description?: string = '';
  link?: string = '';
}
