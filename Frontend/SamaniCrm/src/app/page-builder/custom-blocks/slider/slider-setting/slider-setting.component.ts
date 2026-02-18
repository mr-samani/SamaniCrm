import { Component, Inject, OnInit } from '@angular/core';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder/core';
import { SlideContext, SliderSetting } from '../SliderSetting';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { IOptions } from '@app/file-manager/options.interface';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { AppComponentBase } from '@app/app-component-base';
import { TranslateModule } from '@ngx-translate/core';
import { SwitchModule } from '@shared/components/switch/switch.module';

@Component({
  selector: 'app-slider-setting',
  templateUrl: './slider-setting.component.html',
  styleUrls: ['./slider-setting.component.scss'],
  imports: [FormsModule, FileManagerModule, SwitchModule, TranslateModule],
})
export class SliderSettingComponent extends AppComponentBase implements OnInit {
  settings: SliderSetting;
  imagePickerOptions: IOptions = {
    type: 'Image',
    showPreview: true,
  };
  constructor(@Inject(COMPONENT_DATA) private context: ComponentDataContext<SliderSetting>) {
    super();
    this.settings = this.context.data || new SliderSetting();
  }

  ngOnInit() {}
  onSelectFile(item: SlideContext, file: FileManagerDto) {
    if (file) {
      item.image = file.id ?? '';
    }
    this.update();
  }
  addSlide() {
    this.settings.sliders.push(new SlideContext());
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
  update() {
    this.context.onChange.next(this.settings);
  }
}
