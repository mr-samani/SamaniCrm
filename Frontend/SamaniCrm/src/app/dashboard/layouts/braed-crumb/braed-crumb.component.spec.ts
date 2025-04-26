import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BraedCrumbComponent } from './braed-crumb.component';

describe('BraedCrumbComponent', () => {
  let component: BraedCrumbComponent;
  let fixture: ComponentFixture<BraedCrumbComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BraedCrumbComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BraedCrumbComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
