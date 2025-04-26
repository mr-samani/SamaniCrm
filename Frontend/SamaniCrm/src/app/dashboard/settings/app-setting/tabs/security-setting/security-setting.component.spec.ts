/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SecuritySettingComponent } from './security-setting.component';

describe('SecuritySettingComponent', () => {
  let component: SecuritySettingComponent;
  let fixture: ComponentFixture<SecuritySettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SecuritySettingComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SecuritySettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
