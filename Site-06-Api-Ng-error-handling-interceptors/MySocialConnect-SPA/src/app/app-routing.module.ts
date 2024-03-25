import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './site/home/home.component';
import { MemberListComponent } from './site/members/member-list/member-list.component';
import { MemberDetailComponent } from './site/members/member-detail/member-detail.component';
import { ListsComponent } from './site/lists/lists.component';
import { MessagesComponent } from './site/messages/messages.component';
import { MemberEditComponent } from './site/members/member-edit/member-edit.component';
import { NotLoggedInComponent } from './site/errors/not-logged-in/not-logged-in.component';

import { authGuard } from './core/guards/auth.guard';

const routes: Routes = [
  { path: '', component: HomeComponent },
  //dummy route to group secure resources together
  {
    path: '', 
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard], 
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'members/detail/:guid/:name', component: MemberDetailComponent },
      { path: 'members/edit', component: MemberEditComponent },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
    ]
  },
  { path: 'errors/notloggedin', component: NotLoggedInComponent },
  { path: '**', component: HomeComponent, pathMatch: 'full' } //invalid route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
