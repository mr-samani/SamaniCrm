import { Component } from '@angular/core';
import { BreadcrumbService } from '@shared/services/breadcrumb.service';

@Component({
  selector: 'braed-crumb',
  templateUrl: './braed-crumb.component.html',
  styleUrl: './braed-crumb.component.scss',
  standalone: false,
})
export class BraedCrumbComponent {
  constructor(public breadcrumb: BreadcrumbService) {}
}
