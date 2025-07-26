import { ChangeDetectorRef, Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { FormBuilderService } from '../form-builder.service';
import { BlockTypeEnum } from '../blocks/block-registry';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.scss'],
  standalone: false,
  encapsulation: ViewEncapsulation.None,
})
export class PropertiesComponent extends AppComponentBase implements OnInit {
  borderCss = '';
  tab: 'General' | 'Advanced' = 'General';

  constructor(
    injector: Injector,
    private ch: ChangeDetectorRef,
    public b: FormBuilderService,
    private matDialog: MatDialog,
  ) {
    super(injector);
  }

  ngOnInit() {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  updateCss() {
    if (!this.b.selectedBlock) return;
    this.b.updateCss();
    this.ch.detectChanges();
  }

  clearStyle(styleKey: keyof Partial<CSSStyleDeclaration>) {
    if (!this.b.selectedBlock || !this.b.selectedBlock.data || !this.b.selectedBlock.data.style) return;
    delete this.b.selectedBlock.data.style[styleKey];
  }

  async saveAsBlockDefinition() {
    const { SaveAsBlockDialogComponent } = await import(
      '../_dialogs/save-as-block-dialog/save-as-block-dialog.component'
    );
    this.matDialog
      .open(SaveAsBlockDialogComponent, {
        data: {
          block: this.b.selectedBlock,
        },
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.b.getCustomBlocks();
        }
      });
  }
}
