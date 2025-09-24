import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BreadcrumbService } from '@shared/services/breadcrumb.service';

@Component({
  selector: 'braed-crumb',
  templateUrl: './braed-crumb.component.html',
  styleUrl: './braed-crumb.component.scss',
  standalone: false,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BraedCrumbComponent {
  constructor(public breadcrumb: BreadcrumbService) {}
}
