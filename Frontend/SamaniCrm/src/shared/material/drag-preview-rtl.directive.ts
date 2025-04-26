import { CdkDragStart } from '@angular/cdk/drag-drop';
import { Directive, HostListener } from '@angular/core';

/**
 * fix bug drag drop in RTL
 */
@Directive({
  selector: '[cdkDrag]',
  standalone: true,
})
export class DragPreviewRtlDirective {
  isRtl = false;
  constructor() {}

  ngOnInit(): void {}

  checkPageDirection() {
    const htmlIsRtl = document.documentElement.dir == 'rtl';
    const bodyIsRtl = document.body.dir == 'rtl';
    this.isRtl = htmlIsRtl || bodyIsRtl;
  }

  @HostListener('cdkDragStarted', ['$event'])
  onMouseDown(event: CdkDragStart) {
    this.checkPageDirection();
    if (this.isRtl) {
      let drgPrview = document.querySelector('.cdk-drag.cdk-drag-preview');
      if (drgPrview) drgPrview.removeAttribute('popover');
    }
  }
}
