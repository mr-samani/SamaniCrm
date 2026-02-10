import { SourceItem } from 'ngx-page-builder';

export const SWIPER_SLIDE_BLOCK: SourceItem = {
  tag: 'swiper-slide',
  title: 'Swiper Slider',
  icon: `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <rect x="2" y="6" width="20" height="12" rx="2"/>
    <path d="M7 11h10"/>
    <path d="M7 15h6"/>
    <circle cx="17" cy="13" r="1"/>
  </svg>`,
  options: {},
  customComponent: {
    componentKey: 'swiperslide',
    component: () => import('./swiper-slide.component').then((c) => c.SwiperSlideComponent),
    componentSettings: () =>
      import('./swiper-slide-setting/swiper-slide-setting.component').then((s) => s.SwiperSlideSettingComponent),
  },
  classList: ['swiper-block'],
  css: `
  .swiper-block {
    min-height: 300px;
    width: 100%;
  }
  
  /* Default preview styles for the builder */
  .swiper-block .swiper-container-wrapper {
    width: 100%;
    height: 100%;
  }
  `,
};
