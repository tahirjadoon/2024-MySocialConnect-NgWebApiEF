import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Subscription, combineLatest, forkJoin } from 'rxjs';
import { AccountService } from '../../../core/services/account.service';
import { HelperService } from '../../../core/services/helper.service';

@Component({
  selector: 'app-not-logged-in',
  templateUrl: './not-logged-in.component.html',
  styleUrls: ['./not-logged-in.component.css']
})
export class NotLoggedInComponent implements OnInit, OnDestroy {

  returnUrl: string = "";
  isLoggedIn: boolean = false;

  querySubscription!: Subscription;
  loggedInUserSubscription!: Subscription;
  forkjoinSubscription!: Subscription;
  combineLatestSubscription!: Subscription;

  constructor(private activateRoute: ActivatedRoute, 
            private accountService: AccountService, 
            private helperService: HelperService, 
            private router: Router){}

  ngOnDestroy(): void {
    if(this.querySubscription) this.querySubscription.unsubscribe();
    if(this.loggedInUserSubscription) this.loggedInUserSubscription.unsubscribe();
    if(this.forkjoinSubscription) this.forkjoinSubscription.unsubscribe();
    if(this.combineLatestSubscription) this.combineLatestSubscription.unsubscribe();
  }

  ngOnInit(): void {
    //this.individual();
    this.together();
  }

  individual(){
    this.loggedInUserSubscription = this.accountService.currentLoggedInUser$.subscribe({
      next: r => {
        this.isLoggedIn = !!r;
        this.getParams();
      },
      error: e => {
        this.helperService.logIfError(e, "getCurrentLoggedInUser")
      }
    });
  }

  getParams(){
    //one way of getting the query param
    /*
    const returnUrl = this.activateRoute.snapshot.queryParamMap.get("returnUrl");
    if(returnUrl) 
      this.returnUrl = returnUrl;
    */

    //second way of getting the query param - use this
    this.querySubscription = this.activateRoute.queryParamMap.subscribe({
      next: (params: ParamMap) => {
        const returnUrl = params.get("returnUrl");
        if(returnUrl) 
          this.returnUrl = returnUrl;

        if(this.isLoggedIn){
          if(this.returnUrl)
            this.router.navigateByUrl(this.returnUrl);
          else 
            this.router.navigateByUrl('/members');
        }
      }
    });
  }

  together(){
    const user$ = this.accountService.currentLoggedInUser$;
    const param$ = this.activateRoute.queryParamMap;

    /*
    forkjoin the subscription must end
    this.forkjoinSubscription = forkJoin([user$, param$]).subscribe({
      next: result => {
        this.doAction(result);
      },
      error: e => {
        console.log(e);
      },
      complete: () => {
        console.log('complete');
      }
    });
    */

    
    this.combineLatestSubscription = combineLatest([user$,param$]).subscribe({
      next: r => {
        this.doAction(r);
      }
    });
  }

  doAction(r: any){
    const userResult = r[0];
    const paramResult = r[1];

    this.isLoggedIn = !!userResult;

    const temp = paramResult.get('returnUrl');
    if(temp) this.returnUrl = temp;

    if(this.isLoggedIn){
      let redirectTo = "/members";
      if(temp)
        redirectTo = temp;
      this.router.navigateByUrl(redirectTo);
    }
  }
}
