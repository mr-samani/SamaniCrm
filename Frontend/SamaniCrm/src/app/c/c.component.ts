import { CommonModule } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AppComponentBase } from '@app/app-component-base';
import { ShadowControlComponent } from '@app/builder/_libs/shadow-control/shadow-control.component';

@Component({
  selector: 'app-c',
  templateUrl: './c.component.html',
  styleUrls: ['./c.component.scss'],
  standalone: true,
  imports: [CommonModule, ShadowControlComponent, FormsModule],
})
export class CComponent extends AppComponentBase implements OnInit {
  shadow =
    'blue 0px 0px 0px 2px inset, rgb(255, 255, 255) 10px -10px 0px -3px, rgb(31, 193, 27) 10px -10px, rgb(255, 255, 255) 20px -20px 0px -3px, rgb(255, 217, 19) 20px -20px, rgb(255, 255, 255) 30px -30px 0px -3px, rgb(255, 156, 85) 30px -30px, rgb(255, 255, 255) 40px -40px 0px -3px, rgb(255, 85, 85) 40px -40px';
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {}
}
