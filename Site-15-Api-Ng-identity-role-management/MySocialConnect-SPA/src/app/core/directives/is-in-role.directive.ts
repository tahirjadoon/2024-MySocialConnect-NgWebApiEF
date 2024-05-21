import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';
import { AccountService } from '../services/account.service';

/*
this needs to be structural directive
*appHasRole='["Admin", "Moderator"]'
*/
@Directive({
  selector: '[appIsInRole]'
})
export class IsInRoleDirective implements OnInit {

  @Input() appIsInRole: string[] = [];
  user: LoggedInUserDto = <LoggedInUserDto>{}; //or {} as LoggedInUserDto

  constructor(private viewContainerRef: ViewContainerRef, 
              private templateRef: TemplateRef<any>, 
              private accountService: AccountService) { 

    this.accountService.currentLoggedInUser$.pipe(take(1)).subscribe({
      next: (user: LoggedInUserDto | null) => {
        if(user)
          this.user = user;
      }
    });
  }

  //remove the item from the DOM
  ngOnInit(): void {
    //when no user or no roles for user then clear
    //when no roles passed in then clear
    if(!this.user || !this.user?.roles || !this.appIsInRole || this.appIsInRole.length <= 0){
      this.viewContainerRef.clear();
      return;
    }

    //if the user has roles and passed ones exist in the roles then keep the item
    if(this.user.roles.some(r => this.appIsInRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
      return;
    }

    //clear
    this.viewContainerRef.clear();
  }
}
