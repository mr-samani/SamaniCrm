import { AfterViewInit, Component, ElementRef, Injector, Input, OnInit } from '@angular/core';
import { FormBuilderService } from '../form-builder.service';
import { BlockDefinition } from './block-registry';

export class BlockBase {
  block!: BlockDefinition;
  index!: number;
  b: FormBuilderService;
  el: ElementRef<HTMLElement>;
  constructor(injector: Injector) {
    this.b = injector.get(FormBuilderService);
    this.el = injector.get(ElementRef);
  }
}
