import { SwiperOptions } from 'swiper/types';

export type SwiperBreakpointItem = {
  width: number;
  slidesPerView?: number | 'auto';
  spaceBetween?: number | string;
};

export class SwiperSlideSetting {
  rewind = true;
  sliders: SwiperSlideContext[];

  direction: 'horizontal' | 'vertical' = 'horizontal';

  /**
   * Duration of transition between slides (in ms)
   *
   * @default 300
   */
  speed?: number = 300;

  /**
   * Enabled this option and plugin will set width/height on swiper wrapper equal to total size of all slides.
   * Mostly should be used as compatibility fallback option for browser that don't support flexbox layout well
   *
   * @default false
   */
  setWrapperSize?: boolean;

  /**
   * Transition effect. Can be `'slide'`, `'fade'`, `'cube'`, `'coverflow'`, `'flip'`, `'creative'` or `'cards'`
   *
   * @default 'slide'
   */
  effect?: 'slide' | 'fade' | 'cube' | 'coverflow' | 'flip' | 'creative' | 'cards' | (string & {}) = 'slide';
  /**
   * Distance between slides in px.
   *
   * @default 0
   *
   * @note If you use "margin" css property to the elements which go into Swiper in which you pass "spaceBetween" into, navigation might not work properly.
   */
  spaceBetween?: number | string = 5;

  /**
   * Number of slides per view (slides visible at the same time on slider's container).
   * @note `slidesPerView: 'auto'` is currently not compatible with multirow mode, when `grid.rows` > 1
   *
   * @default 1
   */
  slidesPerView?: number | 'auto';

  /**
   * If `true`, then active slide will be centered, not always on the left side.
   *
   * @default false
   */
  centeredSlides?: boolean;
  /**
   * Add (in px) additional slide offset in the beginning of the container (before all slides)
   *
   * @default 0
   */
  slidesOffsetBefore?: number;

  /**
   * Add (in px) additional slide offset in the end of the container (after all slides)
   *
   * @default 0
   */
  slidesOffsetAfter?: number;
  /**
   * This option may a little improve desktop usability. If `true`, user will see the "grab" cursor when hover on Swiper
   *
   * @default false
   */
  grabCursor?: boolean;

  /**
   * Set to `true` and click on any slide will produce transition to this slide
   *
   * @default false
   */
  slideToClickedSlide?: boolean;

  // Progress
  /**
   * Enable this feature to calculate each slides progress and visibility (slides in viewport will have additional visible class)
   *
   * @default false
   */
  watchSlidesProgress?: boolean;

  /**
   * Set to `true` to enable continuous loop mode
   *
   * Because of nature of how the loop mode works (it will rearrange slides), total number of slides must be:
   *
   * - more than or equal to `slidesPerView` + `slidesPerGroup` (and `+ 1` in case of `centeredSlides`)
   * - even to `slidesPerGroup` (or use `loopAddBlankSlides` parameter)
   * - even to `grid.rows` (or use `loopAddBlankSlides` parameter)
   *
   * @default false
   *
   */
  loop?: boolean = true;
  breakpointsList: SwiperBreakpointItem[] = [
    { width: 320, slidesPerView: 2, spaceBetween: 20 },
    { width: 480, slidesPerView: 3, spaceBetween: 30 },
    { width: 640, slidesPerView: 4, spaceBetween: 40 },
  ];
  // legacy persisted shape
  breakpoints?: {
    [width: number]: SwiperOptions;
    [ratio: string]: SwiperOptions;
  };

  constructor() {
    this.sliders = Array.from({ length: 10 }).map(
      (e) =>
        (e = {
          image: '',
        }),
    );
    this.slidesPerView = 8;
    this.slidesOffsetBefore = 10;
    this.slidesOffsetAfter = 10;
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
    const { breakpointsList, breakpoints, ...rest } = this as unknown as Record<string, unknown>;
    return {
      ...(rest as SwiperOptions),
      breakpoints: this.buildBreakpoints(),
    };
  }
}

export class SwiperSlideContext {
  image: string = '';
  title?: string = '';
}
