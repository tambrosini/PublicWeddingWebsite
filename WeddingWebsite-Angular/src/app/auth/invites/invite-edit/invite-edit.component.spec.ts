import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteEditComponent } from './invite-edit.component';

describe('InviteEditComponent', () => {
  let component: InviteEditComponent;
  let fixture: ComponentFixture<InviteEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InviteEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InviteEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
