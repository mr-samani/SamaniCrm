import { Component, Inject, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { IOptions } from 'grapesjs-tui-image-editor';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder';
import { SwiperSlideContext, SwiperSlideSetting } from './SwiperSlideSetting';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { SwitchModule } from '@shared/components/switch/switch.module';

@Component({
  selector: 'app-swiper-slide-setting',
  templateUrl: './swiper-slide-setting.component.html',
  styleUrls: ['./swiper-slide-setting.component.scss'],
  imports: [CommonModule, FormsModule, FileManagerModule, SwitchModule, TranslateModule],
})
export class SwiperSlideSettingComponent extends AppComponentBase implements OnInit {
  breakPointSizes = [320, 480, 640, 768, 992, 1280];
  settings: SwiperSlideSetting;
  imagePickerOptions: IOptions = {
    type: 'Image',
    showPreview: true,
  };
  constructor(@Inject(COMPONENT_DATA) private context: ComponentDataContext<SwiperSlideSetting>) {
    super();
    this.settings = this.context.data || new SwiperSlideSetting();
  }

  ngOnInit() {}
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

  addBreakPoint() {
    if (!this.settings.breakpoints) return;
    const c = this.breakPointSizes.filter(
      (x) =>
        Object.entries(this.settings.breakpoints!)
          .map((m) => +m[0])
          .indexOf(x) == -1,
    );
    if (c.length == 0) {
      return;
    }
    this.settings.breakpoints[c[0]] = {
      slidesPerView: 2,
      spaceBetween: 1,
    };
    this.update();
  }
  removeBreakPoint(key: number | string) {
    if (!this.settings.breakpoints) return;
    delete this.settings.breakpoints[key];
    this.update();
  }

  updateBreakPointSize(oldKey: string, ev: Event, value: any) {
    if (!this.settings.breakpoints) return;
    const newKey = (ev.target as HTMLSelectElement).value;

    if (oldKey === newKey) return;

    // جلوگیری از overwrite شدن
    if (this.settings.breakpoints[newKey]) {
      console.warn('Breakpoint already exists:', newKey);
      return;
    }

    // انتقال مقدار
    this.settings.breakpoints = {
      ...this.settings.breakpoints,
      [newKey]: value,
    };

    // حذف کلید قبلی
    delete this.settings.breakpoints[oldKey];

    this.update(); // re-init swiper
  }
  update() {
    this.context.onChange.next(this.settings);
  }
}
