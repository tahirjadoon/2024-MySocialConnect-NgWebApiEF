import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './site/home/home.component';
import { MemberListComponent } from './site/members/member-list/member-list.component';
import { MemberDetailComponent } from './site/members/member-detail/member-detail.component';
import { ListsComponent } from './site/lists/lists.component';
import { MessagesComponent } from './site/messages/messages.component';
import { MemberEditComponent } from './site/members/member-edit/member-edit.component';
import { SampleComponent } from './site/errors/sample/sample.component';
import { NotFoundComponent } from './site/errors/not-found/not-found.component';
import { ServerErrorComponent } from './site/errors/server-error/server-error.component';
import { NotLoggedInComponent } from './site/errors/not-logged-in/not-logged-in.component';

import { authGuard } from './core/guards/auth.guard';
import { preventUnsavedChangesGuard } from './core/guards/prevent-unsaved-changes.guard';

import { memberDetailResolver } from './core/resolvers/member-detail.resolver';
import { AdminPanelComponent } from './site/admin/admin-panel/admin-panel.component';
import { adminGuard } from './core/guards/admin.guard';


const routes: Routes = [
  { path: '', component: HomeComponent },
  //dummy route to group secure resources together
  {
    path: '', 
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard], 
    children: [
      { path: 'members', component: MemberListComponent },
      //resolver used, property member will get the data so use 'member' in member-detail page
      { path: 'members/detail/:guid/:name', component: MemberDetailComponent, resolve: {member: memberDetailResolver} },
      { path: 'members/edit', component: MemberEditComponent, canDeactivate: [preventUnsavedChangesGuard] },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'admin', component: AdminPanelComponent, canActivate: [adminGuard] },
    ]
  },
  { path: 'errors/sample', component: SampleComponent},
  { path: 'errors/notloggedin', component: NotLoggedInComponent },
  { path: 'errors/notfound', component: NotFoundComponent },
  { path: 'errors/servererror', component: ServerErrorComponent },
  { path: '**', component: NotFoundComponent, pathMatch: 'full' } //invalid route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
