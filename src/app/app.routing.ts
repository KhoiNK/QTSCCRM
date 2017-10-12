import { NgModule } from '@angular/core';
import { CommonModule, } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { Routes, RouterModule } from '@angular/router';

import { DashboardComponent } from './dashboard/dashboard.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { TableListComponent } from './table-list/table-list.component';
import { TypographyComponent } from './typography/typography.component';
import { IconsComponent } from './icons/icons.component';
import { MapsComponent } from './maps/maps.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { UserLogin } from './user-profile/user-login.component';
import {MarketingComponent} from './marketing/marketing.component';
import {MarketingDetailComponent} from './marketingdetail/marketing-detail.component';
import {OpportunityComponent} from './opportunity/opportunity.component';
import {ExportComponent} from './export/export.component';
import {OpportunityDetailComponent} from './opportunity-detail/opportunity-detail.component';

const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent },
  { path: 'user-profile', component: UserProfileComponent },
  { path: 'table-list', component: TableListComponent },
  { path: 'marketing', component: MarketingComponent },
  { path: 'marketing/marketing-detail/:id', component: MarketingDetailComponent },
  { path: 'typography', component: TypographyComponent },
  { path: 'opportunity', component: OpportunityComponent },
  { path: 'export', component: ExportComponent },
  { path: 'opportunity/opportunity-detail/:id', component: OpportunityDetailComponent },
  { path: 'icons', component: IconsComponent },
  { path: 'maps', component: MapsComponent },
  { path: 'notifications', component: NotificationsComponent },
  { path: 'login', component: UserLogin },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];

@NgModule({
  imports: [
    CommonModule,
    BrowserModule,
    RouterModule.forRoot(routes)
  ],
  exports: [
  ],
})
export class AppRoutingModule { }
