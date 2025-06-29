import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { BlockDefinition } from '../block-registry';
import { FormBuilderService } from '@app/builder/form-builder.service';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { IDropEvent, NgxDragDropKitModule } from 'ngx-drag-drop-kit';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent implements OnInit {
  @Input() block?: BlockDefinition;

  constructor(public b: FormBuilderService) {}

  ngOnInit(): void {}


}
