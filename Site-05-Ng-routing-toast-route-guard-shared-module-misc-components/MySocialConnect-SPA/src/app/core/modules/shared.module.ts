import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

//app routing module
import { AppRoutingModule } from '../../app-routing.module';

//directive for using the validator in template driven forms
import { MustMatchDirective } from '../directives/must-match.directive';
import { PasswordStrengthDirective } from '../directives/password-strength.directive';
import { UserNameAllowedDirective } from '../directives/user-name-allowed.directive';
import { UserNameCheckDirective } from '../directives/user-name-check.directive';

//components
import { HomeComponent } from '../../site/home/home.component';
import { RegisterComponent } from '../../site/register/register.component';
import { MemberListComponent } from '../../site/members/member-list/member-list.component';
import { MemberDetailComponent } from '../../site/members/member-detail/member-detail.component';
import { ListsComponent } from '../../site/lists/lists.component';
import { MessagesComponent } from '../../site/messages/messages.component';
import { MemberCardComponent } from '../../site/members/member-card/member-card.component';
import { MemberEditComponent } from '../../site/members/member-edit/member-edit.component';
import { PhotoEditorComponent } from '../../site/members/photo-editor/photo-editor.component';
import { NotLoggedInComponent } from '../../site/errors/not-logged-in/not-logged-in.component';
import { DisplayFGErrorsComponent } from '../../errors/display-fgerrors/display-fgerrors.component';

import { RegisterReactiveformComponent } from '../../site/register-reactiveform/register-reactiveform.component';



@NgModule({
  declarations: [
    MustMatchDirective, //for validation template driven form
    PasswordStrengthDirective, //for validation template driven form
    UserNameAllowedDirective, //for validation template driven form
    UserNameCheckDirective, //for validation template driven form

    HomeComponent,
    RegisterComponent, //template driven form
    RegisterReactiveformComponent, //reactive form
    MemberListComponent, 
    MemberDetailComponent, 
    ListsComponent, 
    MessagesComponent, 
    MemberCardComponent, 
    MemberEditComponent, 
    PhotoEditorComponent, 
    NotLoggedInComponent, 
    DisplayFGErrorsComponent, 
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule, 
    ReactiveFormsModule, 
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({ positionClass: 'toast-bottom-right'}),
  ],
  exports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule, 
    ReactiveFormsModule, 
    BsDropdownModule,
    ToastrModule,
  ]
})
export class SharedModule { }
