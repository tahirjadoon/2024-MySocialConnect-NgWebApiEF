import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

import { ToastrService } from 'ngx-toastr';

import { HelperService } from '../../core/services/helper.service';
import { AccountService } from '../../core/services/account.service';

import { LoginDto } from '../../core/models-interfaces/login-dto.model';
import { LoggedInUserDto } from '../../core/models-interfaces/logged-in-user-dto.model';

import { ZRoles } from '../../core/enums/z-roles';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit, AfterViewInit, OnDestroy {
  //put focus on page load in username
  @ViewChild('username', { static: false }) userNameElement!: ElementRef;
  @ViewChild('navbarCollapse') navbarCollapseElement!: ElementRef;

  private isExecutingLogin: boolean = false;
  putFocus = false;
  title = "";
  isLoggedIn: boolean = false;
  loggedInUser: LoggedInUserDto | null = <LoggedInUserDto>{};
  loginDto: LoginDto = <LoginDto>{};
  returnUrl: string = "";

  //to be used with appIsInRole directive on admin link. The link will be hidden
  zRole = ZRoles;

  //subscriptions
  loginSubscription!: Subscription;
  loggedInUserSubscription!: Subscription;
  queryParamsubscription!: Subscription;

  constructor(private helperService: HelperService, private accountService: AccountService, 
            private router: Router, 
            private toastr: ToastrService, 
            private activatedRoute: ActivatedRoute, 
            private el: ElementRef, 
            private renderer: Renderer2) { }
  
  ngAfterViewInit(): void {
    this.putFocus = true;
    this.focusUserName();
  }

  ngOnInit(): void {
    this.title = this.helperService.Title;
    this.getCurrentLoggedInUser(); //persisted user in local storage

    //fill return url if avaialble 
    //method #1
    //const returnUrl1 = this.activatedRoute.snapshot.queryParamMap.get('returnUrl');
    //if(returnUrl1) 
    //  this.returnUrl = returnUrl1;
    //method #2
    this.queryParamsubscription = this.activatedRoute.queryParamMap.subscribe({
      next: (params: ParamMap) => {
        const returnUrl = params.get("returnUrl");
        if(returnUrl) 
          this.returnUrl = returnUrl;
      }
    });
  }

  ngOnDestroy(): void {
    if(this.loginSubscription) this.loginSubscription.unsubscribe();
    if(this.loggedInUserSubscription) this.loggedInUserSubscription.unsubscribe();
    if(this.queryParamsubscription) this.queryParamsubscription.unsubscribe();
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

    if(!this.loginDto || !this.loginDto.userName || !this.loginDto.password){
      this.focusUserName();
      this.toastr.error('Login info missing');
      return;
    }

    //set executing login true
    this.isExecutingLogin = true;

    //instead of r can also use () or _
    this.loginSubscription = this.accountService.login(this.loginDto).subscribe({
      next: r => {
        this.onNavBarItemClickCloseNavBar();
        this.helperService.logIfFrom(r, 'Login.User Response');
        //clear the login form
        this.loginDto = <LoginDto>{};
        //this.isLoggedIn = true;
        //this.loggedInUser = r;

        //route to members
        const returnUrl = this.returnUrl ? this.returnUrl : '/members';
        this.router.navigateByUrl(returnUrl);

      }, error: e => {
        this.helperService.logIfError(e, 'NavComponent.OnLogin');
        this.focusUserName();
        //this.toastr.error(e.error); removed after error interceptor
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
    this.onNavBarItemClickCloseNavBar();
    setTimeout(() => {
      this.focusUserName();
      this.returnUrl = "";
      this.router.navigateByUrl("/");
    }, 500);
  }

  //hide the navbar in mobile mode after an action has been performed 
  onNavBarItemClickCloseNavBar() {
    console.log("onNavBarItemClickCloseNavBar");
    const classToRemove = "show";

    /*
    //renderer2 method
    var navBarCollapseTargetItem = this.el.nativeElement.querySelector('#navbarCollapse');
    this.renderer.removeClass(navBarCollapseTargetItem, classToRemove);
    */
    
    //view child method
    if(this.navbarCollapseElement && this.navbarCollapseElement.nativeElement.classList.contains(classToRemove)){
      this.navbarCollapseElement.nativeElement.classList.remove(classToRemove);
    }
  }
}
