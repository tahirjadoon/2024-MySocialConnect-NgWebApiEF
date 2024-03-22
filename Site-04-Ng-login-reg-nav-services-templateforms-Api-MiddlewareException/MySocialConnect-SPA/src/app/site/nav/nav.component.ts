import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';

import { HelperService } from '../../core/services/helper.service';
import { AccountService } from '../../core/services/account.service';

import { LoginDto } from '../../core/models-interfaces/login-dto.model';
import { LoggedInUserDto } from '../../core/models-interfaces/logged-in-user-dto.model';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, AfterViewInit, OnDestroy {
  //put focus on page load in username
  @ViewChild('username', { static: false }) userNameElement!: ElementRef;

  private isExecutingLogin: boolean = false;
  putFocus = false;
  title = "";
  isLoggedIn: boolean = false;
  loggedInUser: LoggedInUserDto | null = <LoggedInUserDto>{};
  loginDto: LoginDto = <LoginDto>{};

  //subscriptions
  loginSubscription!: Subscription;
  loggedInUserSubscription!: Subscription;

  constructor(private helperService: HelperService, private accountService: AccountService) { }
  
  ngAfterViewInit(): void {
    this.putFocus = true;
    this.focusUserName();
  }

  ngOnInit(): void {
    this.title = this.helperService.Title;
    this.getCurrentLoggedInUser(); //persisted user in local storage
  }

  ngOnDestroy(): void {
    if(this.loginSubscription) this.loginSubscription.unsubscribe();
    if(this.loggedInUserSubscription) this.loggedInUserSubscription.unsubscribe();
  }

  private focusUserName(){
    //put a focus in userName element
    if(!this.isLoggedIn && this.putFocus && this.userNameElement.nativeElement){
      this.userNameElement.nativeElement.focus();
    }
  }

  //login method will fire a behavioral subject from inside the account service so tap into it
  //alternatively in login and logout can handle as well. 
  getCurrentLoggedInUser(){
    this.loggedInUserSubscription = this.accountService.currentLoggedInUser$.subscribe({
      next: r => {
        this.isLoggedIn = !!r;
        this.loggedInUser = r;
      },
      error: e => {
        this.helperService.logIfError(e, "getCurrentLoggedInUser")
        this.isLoggedIn = false;
        this.loggedInUser = <LoggedInUserDto>{};
      }
    })
  }

  onLogin(){
    this.helperService.logIfFrom(this.loginDto, 'Login')

    //set executing login true
    this.isExecutingLogin = true;

    this.loginSubscription = this.accountService.login(this.loginDto).subscribe({
      next: r => {
        this.helperService.logIfFrom(r, 'Login.User Response');
        //clear the login form
        this.loginDto = <LoginDto>{};
        //this.isLoggedIn = true;
        //this.loggedInUser = r;
      }, error: e => {
        this.helperService.logIfError(e, 'NavComponent.OnLogin');
        this.focusUserName();
      }, complete: () => {
        //set executing login false
        this.isExecutingLogin = false;
      }
    })
  }

  onLogout(){
    //logout the user
    //this.isLoggedIn = false;
    this.accountService.logout();
    setTimeout(() => {
      this.focusUserName();
    }, 500);
  }
}
