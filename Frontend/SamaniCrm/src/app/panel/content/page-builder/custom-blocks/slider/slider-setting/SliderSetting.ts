export class SliderSetting {
  sliders: SlideContext[] = [
    {
      image: '/images/default-image.png',
    },
  ];
  showCaptions: boolean = true;
  showIndicators: boolean = true;
  showNextPrevious: boolean = true;
  effect: 'carousel-fade' | 'default' = 'default';
  darkMode: boolean = false;
  /** time (milli seconds) for change slide */
  interval: number = 5000;
  keyboard: boolean = true;
  touch: boolean = true;
  pause: 'hover' | false = 'hover';
  ride: 'carousel' | boolean = 'carousel';
}
export class SlideContext {
  image: string = '';
  title?: string = '';
  description?: string = '';
}
