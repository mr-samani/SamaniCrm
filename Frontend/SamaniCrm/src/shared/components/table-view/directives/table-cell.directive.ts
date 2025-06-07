import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
  selector: '[tableCell]',
  standalone: false,
})
export class TableCellDirective {
  @Input('tableCell') columnName!: string;
  constructor(public template: TemplateRef<any>) {}
}
