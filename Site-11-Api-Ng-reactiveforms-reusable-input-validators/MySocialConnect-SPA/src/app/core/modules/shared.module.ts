import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ToastrModule } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';

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

import { RegisterReactiveformComponent } from '../../site/register-reactiveform/register-reactiveform.component';
import { SampleComponent } from '../../site/errors/sample/sample.component';
import { DisplayFgerrorsComponent } from '../../site/errors/display-fgerrors/display-fgerrors.component';




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
    //MemberDetailComponent, this is now a stand alone component so due ng-gallery which is stand alone
    ListsComponent, 
    MessagesComponent, 
    MemberCardComponent, 
    MemberEditComponent, 
    PhotoEditorComponent, 
    NotLoggedInComponent, 
    DisplayFgerrorsComponent,
    SampleComponent
  ],
  imports: [
    CommonModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule, 
    ReactiveFormsModule, 
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    ToastrModule.forRoot({ positionClass: 'toast-bottom-right'}),
    NgxSpinnerModule.forRoot({type: 'line-scale-party'}), 
    FileUploadModule,
  ],
  exports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule, 
    ReactiveFormsModule, 
    BsDropdownModule,
    TabsModule,
    ToastrModule,
    NgxSpinnerModule,
    FileUploadModule,
  ]
})
export class SharedModule { }
