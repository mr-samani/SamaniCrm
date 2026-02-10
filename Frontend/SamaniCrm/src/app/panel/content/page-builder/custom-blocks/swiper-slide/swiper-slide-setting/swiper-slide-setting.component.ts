import { Component, Inject, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from 'grapesjs-tui-image-editor';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder';
import { SwiperSlideContext, SwiperSlideSetting, SwiperPresetType } from './SwiperSlideSetting';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { SwitchModule } from '@shared/components/switch/switch.module';

interface PresetOption {
  value: SwiperPresetType;
  label: string;
  description: string;
  icon: string;
}

@Component({
  selector: 'app-swiper-slide-setting',
  templateUrl: './swiper-slide-setting.component.html',
  styleUrls: ['./swiper-slide-setting.component.scss'],
  imports: [CommonModule, FormsModule, FileManagerModule, SwitchModule, TranslateModule],
})
export class SwiperSlideSettingComponent extends AppComponentBase implements OnInit {
  settings: SwiperSlideSetting;
  imagePickerOptions: IOptions = {
    type: 'Image',
    showPreview: true,
  };

  activeTab: 'slides' | 'preset' | 'basic' | 'advanced' | 'responsive' = 'preset';

  presetOptions: PresetOption[] = [
    {
      value: 'default',
      label: 'Default Slider',
      description: 'Classic slider with navigation and pagination',
      icon: '🎞️',
    },
    {
      value: 'cards',
      label: 'Cards Stack',
      description: 'Tinder-like card swiper',
      icon: '🃏',
    },
    {
      value: 'stories',
      label: 'Stories',
      description: 'Instagram stories style',
      icon: '📸',
    },
    {
      value: 'products',
      label: 'Product Showcase',
      description: 'Grid layout for products',
      icon: '🛍️',
    },
    {
      value: 'coverflow',
      label: 'Coverflow',
      description: 'Apple iTunes style',
      icon: '💿',
    },
    {
      value: 'cube',
      label: 'Cube Effect',
      description: '3D cube transition',
      icon: '🎲',
    },
    {
      value: 'creative',
      label: 'Creative',
      description: 'Custom creative effects',
      icon: '✨',
    },
  ];

  effectOptions = [
    { value: 'slide', label: 'Slide' },
    { value: 'fade', label: 'Fade' },
    { value: 'cube', label: 'Cube' },
    { value: 'coverflow', label: 'Coverflow' },
    { value: 'flip', label: 'Flip' },
    { value: 'creative', label: 'Creative' },
    { value: 'cards', label: 'Cards' },
  ];

  paginationTypes = [
    { value: 'bullets', label: 'Bullets' },
    { value: 'fraction', label: 'Fraction' },
    { value: 'progressbar', label: 'Progress Bar' },
  ];

  breakPointSizes = [320, 480, 640, 768, 992, 1024, 1280, 1536];

  constructor(@Inject(COMPONENT_DATA) private context: ComponentDataContext<SwiperSlideSetting>) {
    super();
    this.settings = this.context.data || new SwiperSlideSetting();
  }

  ngOnInit() {
    // Ensure breakpoints are normalized
    if (!this.settings.breakpointsList || this.settings.breakpointsList.length === 0) {
      this.settings.normalizeBreakpoints();
    }
  }

  // === Preset Management ===
  onPresetChange(preset: SwiperPresetType) {
    this.settings.applyPreset(preset);
    this.update();
  }

  // === Slides Management ===
  onSelectFile(item: SwiperSlideContext, file: FileManagerDto) {
    if (file) {
      item.image = file.id ?? '';
    }
    this.update();
  }

  addSlide() {
    this.settings.sliders.push(new SwiperSlideContext());
    this.update();
  }

  removeSlide(index: number) {
    this.alert
      .show({
        title: this.l('AreYouSureForDelete'),
        showCancelButton: true,
        cancelButtonText: this.l('Cancel'),
        showConfirmButton: true,
        confirmButtonText: this.l('Ok'),
      })
      .then((r) => {
        if (r.isConfirmed) {
          this.settings.sliders.splice(index, 1);
          this.update();
        }
      });
  }

  // === Breakpoints Management ===
  addBreakPoint() {
    const existingWidths = this.settings.breakpointsList.map((b) => b.width);
    const availableWidth = this.breakPointSizes.find((size) => !existingWidths.includes(size));

    if (!availableWidth) {
      this.alert.show({
        title: 'No More Breakpoints',
        text: 'All standard breakpoint sizes are already in use.',
        icon: 'warning',
      });
      return;
    }

    this.settings.breakpointsList.push({
      width: availableWidth,
      slidesPerView: this.settings.slidesPerView || 1,
      spaceBetween: this.settings.spaceBetween || 10,
    });

    this.update();
  }

  removeBreakPoint(index: number) {
    this.settings.breakpointsList.splice(index, 1);
    this.update();
  }

  // === Tab Management ===
  setActiveTab(tab: 'slides' | 'preset' | 'basic' | 'advanced' | 'responsive') {
    this.activeTab = tab;
  }

  // === Update Context ===
  update() {
    this.context.onChange.next(this.settings);
  }

  // === Helper Methods ===
  trackByIndex(index: number): number {
    return index;
  }
}
