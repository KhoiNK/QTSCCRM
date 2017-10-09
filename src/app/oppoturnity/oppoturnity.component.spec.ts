import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OppoturnityComponent } from './oppoturnity.component';

describe('OppoturnityComponent', () => {
  let component: OppoturnityComponent;
  let fixture: ComponentFixture<OppoturnityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OppoturnityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OppoturnityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
