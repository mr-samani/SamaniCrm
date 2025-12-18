import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@app/app-component-base'; 
import { SharedModule } from '@shared/shared.module';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: true,
  imports: [CommonModule, SharedModule,],
})
export class DashboardComponent extends AppComponentBase implements OnInit {
  editMode = false;
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
