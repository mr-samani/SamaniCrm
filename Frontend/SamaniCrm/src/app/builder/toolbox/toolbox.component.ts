import { Component,  OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base';
import { BlockTypeEnum, BlockDefinition } from '../blocks/block-registry';
import { FormBuilderService } from '../services/form-builder.service';
import { PageBuilderServiceProxy } from '@shared/service-proxies/api/page-builder.service';

@Component({
  selector: 'toolbox',
  templateUrl: './toolbox.component.html',
  styleUrls: ['./toolbox.component.scss'],
  standalone: false,
})
export class ToolboxComponent extends AppComponentBase implements OnInit {
  constructor(
    public b: FormBuilderService,
    private pageBuilderService: PageBuilderServiceProxy,
  ) {
    super();
  }

  ngOnInit() {}

  public get BlockTypeEnum(): typeof BlockTypeEnum {
    return BlockTypeEnum;
  }

  deleteCustomBlock(t: BlockDefinition) {
    if (!t.canDelete || !t.id) return;

    // this.confirmMessage(this.l('AreYouSureForDelete'), t.name ?? '').then((r) => {
    //   if (r.isConfirmed) {
    //     this.pageBuilderService.deleteCustomBlock(t.id).subscribe((result) => {
    //       this.notify.success(this.l('DeleteSuccessfully'));
    //       this.b.getCustomBlocks();
    //     });
    //   }
    // });
  }
}
