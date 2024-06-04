import { Component, OnDestroy, OnInit } from '@angular/core';

import { HelperService } from '../../core/services/helper.service';
import { MemberService } from '../../core/services/member.service';

import { ZUserLikeType } from '../../core/enums/z-user-like-type';
import { AppConstants } from '../../core/constants/app-constants';

import { LikeParamsDto } from '../../core/models-interfaces/like-params-dto.model';
import { UserDto } from '../../core/models-interfaces/user-dto.model';
import { Subscription } from 'rxjs';
import { PaginatedResult } from 'src/app/core/models-interfaces/pagination/paginated-result.model';
import { Pagination } from 'src/app/core/models-interfaces/pagination/pagination.model';



@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit, OnDestroy {

  pagination!: Pagination;
  members: Partial<UserDto[]> = []
  likeParams: LikeParamsDto;
  userTypeString: string;

  //to be used in the component
  zUserLiketype = ZUserLikeType;

  likesSubscription!: Subscription;

  constructor(private memberService: MemberService, private helperService: HelperService){
    this.likeParams = new LikeParamsDto();
    this.userTypeString = ZUserLikeType[this.likeParams.userLikeType];

    //this.helperService.logIfFrom(this.userTypeString, "ListsComponenet");
    //this.helperService.logIf(this.likeParams);
    
  }

  ngOnDestroy(): void {
    if(this.likesSubscription) this.likesSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    const userTypeString = ZUserLikeType[this.likeParams.userLikeType];
    //if the user has asked for a different type then reset the number
    if(this.userTypeString != userTypeString){
      this.userTypeString = userTypeString;
      this.likeParams.pageNumber = AppConstants.PageNumber;
    }

    this.likesSubscription = this.memberService.getLike(this.likeParams).subscribe({
      next: (response: PaginatedResult<Partial<UserDto[]>>) => {
        if(response.result)
          this.members = response.result
        if(response.pagination)
          this.pagination = response.pagination;
      },
      error: e => {},
      complete: () => {}
    });

  }

  onPageChanged(event: any){
    if(this.likeParams) 
      this.likeParams.pageNumber = event.page;
    this.loadLikes();
  }
}
