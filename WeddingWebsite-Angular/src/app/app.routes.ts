import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RsvpComponent } from './rsvp/rsvp.component';
import { FaqComponent } from './faq/faq.component';
import { InfoComponent } from './info/info.component';
import { GuestAuthGuard } from './guest-auth.guard';
import { GuestLoginComponent } from './guest-login/guest-login.component';
import { AdminLoginComponent } from './auth/admin-login/admin-login.component';
import { AdminAuthGuard } from './auth/admin-auth.guard';
import { AdminDashboardComponent } from './auth/admin-dashboard/admin-dashboard.component';
import { GuestCreateComponent } from './auth/guest/guest-create/guest-create.component';
import { GuestDetailComponent } from './auth/guest/guest-detail/guest-detail.component';
import { GuestListComponent } from './auth/guest/guest-list/guest-list.component';
import { GuestEditComponent } from './auth/guest/guest-edit/guest-edit.component';
import { InviteListComponent } from './auth/invites/invite-list/invite-list.component';
import { InviteCreateComponent } from './auth/invites/invite-create/invite-create.component';
import { InviteDetailComponent } from './auth/invites/invite-detail/invite-detail.component';
import { InviteEditComponent } from './auth/invites/invite-edit/invite-edit.component';
import { AdminRsvpComponent } from './auth/invites/admin-rsvp/admin-rsvp.component';

export const routes: Routes = [
    { path: '', component: HomeComponent, canActivate: [GuestAuthGuard]},
    { path: 'rsvp', component: RsvpComponent, canActivate: [GuestAuthGuard]},
    { path: 'faq', component: FaqComponent, canActivate: [GuestAuthGuard]},
    { path: 'info', component: InfoComponent, canActivate: [GuestAuthGuard]},
    { path: 'guest-login', component: GuestLoginComponent},
    {
        path: 'admin',
        children: [
          {
            path: 'login', 
            component: AdminLoginComponent
          },
          {
            path: 'dashboard',
            component: AdminDashboardComponent,
            canActivate: [AdminAuthGuard]
          },
          { 
            path: 'guests', 
            children: [
              { path: '', component: GuestListComponent },
              { path: 'create', component: GuestCreateComponent },
              { path: ':id', component: GuestDetailComponent },
              { path: ':id/edit', component: GuestEditComponent }
            ] 
          },
                    { 
            path: 'invites', 
            children: [
              { path: '', component: InviteListComponent },
              { path: 'create', component: InviteCreateComponent },
              { path: ':id', component: InviteDetailComponent },
              { path: ':id/edit', component: InviteEditComponent },
              { path: ':id/rsvp', component: AdminRsvpComponent}
            ] 
          },
          {
            path: '',
            redirectTo: 'dashboard',
            pathMatch: 'full'
          }
        ]
      },
    { path: '**', redirectTo: '' } //Redirect unknown routes 
];
