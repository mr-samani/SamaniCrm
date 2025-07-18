import { ChangeDetectorRef, Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FormBuilderService } from '../form-builder.service';
import { BlockTypeEnum } from '../blocks/block-registry';
import { BlockStyle } from './styles/models/_style';

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
    private ch: ChangeDetectorRef,
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
  ${this.b.selectedBlock.data.style.borderRadius ? 'border-radius:' + this.b.selectedBlock.data.style.borderRadius + ';' : ''}
  ${this.b.selectedBlock.data.style.boxShadow ? 'box-shadow:' + this.b.selectedBlock.data.style.boxShadow + ';' : ''}
  ${this.b.selectedBlock.data.style.backgroundColor ? 'background-color:' + this.b.selectedBlock.data.style.backgroundColor + ';' : ''}
   ${this.b.selectedBlock.data.style.backgroundImage ? 'background-image:' + this.b.selectedBlock.data.style.backgroundImage + ';' : ''}
  ${this.b.selectedBlock.data.style.backgroundSize ? 'background-size:' + this.b.selectedBlock.data.style.backgroundSize + ';' : ''}
  ${this.b.selectedBlock.data.style.backgroundRepeat ? 'background-repeat:' + this.b.selectedBlock.data.style.backgroundRepeat + ';' : ''}
  `;
    this.ch.detectChanges();
  }

  clearStyle(styleKey: keyof BlockStyle) {
    if (!this.b.selectedBlock || !this.b.selectedBlock.data || !this.b.selectedBlock.data.style) return;
    this.b.selectedBlock.data.style[styleKey] = undefined;
  }
}
