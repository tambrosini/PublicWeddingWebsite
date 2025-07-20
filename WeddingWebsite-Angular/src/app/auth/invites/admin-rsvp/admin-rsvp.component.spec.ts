import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminRsvpComponent } from './admin-rsvp.component';

describe('AdminRsvpComponent', () => {
  let component: AdminRsvpComponent;
  let fixture: ComponentFixture<AdminRsvpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminRsvpComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminRsvpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
