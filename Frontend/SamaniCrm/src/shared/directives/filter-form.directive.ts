import { Directive, ElementRef, Input, Renderer2, TemplateRef, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[filterForm]',
  host: {},
  standalone: false,
})
export class FilteFormrDirective {
  private isVisible = false;

  @Input() set filterForm(condition: boolean) {
    if (condition && !this.isVisible) {
      this.showForm();
    } else if (!condition && this.isVisible) {
      this.hideForm();
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private renderer: Renderer2,
    private el: ElementRef,
  ) {}

  private showForm() {
    this.viewContainer.createEmbeddedView(this.templateRef);
    this.isVisible = true;
  }

  private hideForm() {
    const formElement = this.viewContainer.element.nativeElement.previousSibling;
    this.renderer.addClass(formElement, 'hide');
    setTimeout(() => {
      this.viewContainer.clear();
      this.isVisible = false;
    }, 500);
  }
}
