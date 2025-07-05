import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { DynamicRendererComponent } from '../dynamic-renderer.component';
import { NgxDragDropKitModule } from 'ngx-drag-drop-kit';
import { BlockBase } from '../block-base';

@Component({
  selector: 'block-row',
  standalone: true,
  imports: [CommonModule, NgxDragDropKitModule, DynamicRendererComponent],
  templateUrl: './row.component.html',
  styleUrl: './row.component.scss',
})
export class BlockRowComponent extends BlockBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
