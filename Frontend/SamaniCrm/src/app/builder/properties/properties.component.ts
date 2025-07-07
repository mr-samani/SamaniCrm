import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FormBuilderService } from '../form-builder.service';
import { BlockTypeEnum } from '../blocks/block-registry';

@Component({
  selector: 'properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.scss'],
  standalone: false,
  encapsulation: ViewEncapsulation.None,
})
export class PropertiesComponent extends AppComponentBase implements OnInit {
  borderCss = '';

  constructor(
    injector: Injector,
    public b: FormBuilderService,
  ) {
    super(injector);
  }

  ngOnInit() {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  updateCss() {
    if (!this.b.selectedBlock) return;

    this.b.selectedBlock.data.css = `
  ${this.borderCss}
  ${this.b.selectedBlock.data.style.padding ? 'padding:' + this.b.selectedBlock.data.style.padding + ';' : ''}
  ${this.b.selectedBlock.data.style.margin ? 'margin:' + this.b.selectedBlock.data.style.margin + ';' : ''}
  ${this.b.selectedBlock.data.style.boxShadow ? 'box-shadow:' + this.b.selectedBlock.data.style.boxShadow + ';' : ''}
    `;
  }
}
