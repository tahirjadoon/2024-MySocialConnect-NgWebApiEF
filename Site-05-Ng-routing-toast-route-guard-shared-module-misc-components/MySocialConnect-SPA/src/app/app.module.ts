import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './site/nav/nav.component';
import { HomeComponent } from './site/home/home.component';
import { RegisterComponent } from './site/register/register.component';

//directive for using the validator in template driven forms
import { MustMatchDirective } from './core/directives/must-match.directive';
import { PasswordStrengthDirective } from './core/directives/password-strength.directive';
import { UserNameAllowedDirective } from './core/directives/user-name-allowed.directive';
import { UserNameCheckDirective } from './core/directives/user-name-check.directive';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MustMatchDirective, //for validation template driven form
    PasswordStrengthDirective, //for validation template driven form
    UserNameAllowedDirective, UserNameCheckDirective //for validation template driven form
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule, 
    FormsModule, 
    ReactiveFormsModule, 
    BsDropdownModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
