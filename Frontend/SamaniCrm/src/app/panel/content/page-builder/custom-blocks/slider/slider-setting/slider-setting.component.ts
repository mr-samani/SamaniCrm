import { Component, Inject, OnInit } from '@angular/core';
import { COMPONENT_DATA, ComponentDataContext } from 'ngx-page-builder';
import { SlideContext, SliderSetting } from './SliderSetting';
import { FormsModule } from '@angular/forms';
import { FileManagerModule } from '@app/file-manager/file-manager.module';
import { IOptions } from '@app/file-manager/options.interface';
import { FileManagerDto } from '@app/file-manager/models/file-manager-dto';
import { AppConst } from '@shared/app-const';
import { AppComponentBase } from '@app/app-component-base';
import { SharedModule } from "@shared/shared.module";

@Component({
  selector: 'app-slider-setting',
  templateUrl: './slider-setting.component.html',
  styleUrls: ['./slider-setting.component.scss'],
  imports: [FormsModule, FileManagerModule, SharedModule],
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
      item.image = AppConst.fileServerUrl + '/' + file.id;
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
