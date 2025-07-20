import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuestCreateComponent } from './guest-create.component';

describe('GuestCreateComponent', () => {
  let component: GuestCreateComponent;
  let fixture: ComponentFixture<GuestCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuestCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuestCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
