import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription, take } from 'rxjs';

import { MemberService } from 'src/app/core/services/member.service';
import { AccountService } from 'src/app/core/services/account.service';

import { Pagination } from 'src/app/core/models-interfaces/pagination/pagination.model';
import { UserDto } from 'src/app/core/models-interfaces/user-dto.model';
import { UserParamsDto } from 'src/app/core/models-interfaces/user-params-dto.model';
import { PaginatedResult } from 'src/app/core/models-interfaces/pagination/paginated-result.model';

import { AppConstants } from 'src/app/core/constants/app-constants';
import { LoggedInUserDto } from 'src/app/core/models-interfaces/logged-in-user-dto.model';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit, OnDestroy {

  members: UserDto[] = [];
  membersAsync$: Observable<UserDto[]> | undefined;

  //pagination and filtering
  pagination!: Pagination;
  userParams!: UserParamsDto;
  user!: LoggedInUserDto;

  membersSubscription!: Subscription;

  //filtering
  genderList = [
    { value: AppConstants.Male, display: 'Males' },
    { value: AppConstants.Female, display: 'Females' }
  ];

  constructor(private memberService: MemberService, private accountService: AccountService){
    this.userParams = this.memberService.userParamsGet();
  }

  ngOnDestroy(): void {
    if(this.membersSubscription) this.membersSubscription.unsubscribe();
  }

  ngOnInit(): void {
    //not using load members any more but using membersAsync$
    this.loadMembers();
    //this.membersAsync$ = this.memberService.getMembers();
  }

  loadMembers(){
    if(!this.userParams) return;

    //reset user params
    this.memberService.userParamsUpdate(this.userParams);

    this.membersSubscription = this.memberService.getMembers(this.userParams).subscribe({
      next: (response: PaginatedResult<UserDto[]>) => {
        if(response && response.result && response.pagination){
          this.members = response.result;
          this.pagination = response.pagination;
        }
      },
      error: e => {},
      complete: () => {}
    })
  }

  onPageChanged(event: any){
    if(this.userParams.pageNumber !== event.page){
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

  onResetFilters(){
    this.memberService.userParamsSet();
    this.userParams = this.memberService.userParamsGet();
    this.memberService.memberCacheClear();
    this.loadMembers();
  }

  onGenderSelect() {
    //reset the page number to 1 since the page the user is on my not exist when the gender is changed 
    this.userParams.pageNumber = 1;
  }
}
