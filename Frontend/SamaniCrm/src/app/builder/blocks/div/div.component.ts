import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { DynamicRendererComponent } from '../dynamic-renderer.component';

@Component({
  selector: 'block-div',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent],
  templateUrl: './div.component.html',
})
export class BlockDivComponent {
  @Input() block?: BlockDefinition;

  constructor(public b: FormBuilderService) {}
}
