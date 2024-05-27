import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription, take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../../../core/services/account.service';
import { MemberService } from '../../../core/services/member.service';

import { LoggedInUserDto } from '../../../core/models-interfaces/logged-in-user-dto.model';
import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit, OnDestroy {
  
  //access the editForm 
  @ViewChild('editForm') editForm: NgForm | undefined;

  //We have CanDeactiavte PreventUnsavedChanges guard. It will prevent the user from clicking away when have changes
  //However, the user can type a different url in the broweser 
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if(this.editForm?.dirty){
      $event.returnValue = true;
    }
  }


  member: UserDto | undefined;
  user: LoggedInUserDto | null = null;

  memberSubscription!: Subscription;
  memberUpdateSubscription!: Subscription;

  constructor(private accountService: AccountService, 
              private memberService: MemberService,
              private toastr: ToastrService
  ){
    //we can also get the user from the local storage service as well. every thing is there
    //no need to unsubscribe due to take(1)
    this.accountService.currentLoggedInUser$.pipe(take(1)).subscribe({
      next: (user: LoggedInUserDto | null) => {
        this.user = user;
      }
    })
  }

  ngOnDestroy(): void {
    if(this.memberSubscription) this.memberSubscription.unsubscribe();
    if(this.memberUpdateSubscription) this.memberUpdateSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    if(!this.user) return;

    this.memberSubscription = this.memberService.getMemberByGuid(this.user.guid).subscribe({
      next: (member: UserDto) => {
        this.member = member;
      }
    })
  }

  onUpdateMember()
  {
    if(!this.member){
      this.toastr.error("Member issue");
      return;
    }

    this.memberUpdateSubscription = this.memberService.updateMember(this.member).subscribe({
      next: _ => {
        this.toastr.success("Profile updated successfully!");
        this.editForm?.reset(this.member);
      }, 
      error: e => {},
      complete: () => {}
    });
    
  }

}
