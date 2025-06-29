import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'block-div',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div></div>
  `,
})
export class BlockDivComponent {
  @Input() data!: {};
}
