import { SourceItem } from 'ngx-page-builder/core';

export const SWIPER_SLIDE_BLOCK: SourceItem = {
  tag: 'swiper-slide',
  title: 'Swiper Slider',
  icon: ``,
  options: {},
  customComponent: {
    componentKey: 'swiperslide',
    component: () => import('./swiper-slide.component').then((c) => c.SwiperSlideComponent),
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
