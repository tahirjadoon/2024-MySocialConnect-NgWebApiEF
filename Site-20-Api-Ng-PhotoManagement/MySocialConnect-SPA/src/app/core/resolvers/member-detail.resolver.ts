import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';

import { UserDto } from '../models-interfaces/user-dto.model';
import { MemberService } from '../services/member.service';

//add the resolver to the app-routing.module, member-detail route

export const memberDetailResolver: ResolveFn<UserDto> = (route, state) => {
  const memberService = inject(MemberService);
  //get the route params
  //this.guidParam = this.activatedRoute.snapshot.paramMap.get('guid');
  //this.nameParam = this.activatedRoute.snapshot.paramMap.get("name");
  /*
  this.paramSubscription = this.activatedRoute.params.subscribe(params => {
      this.guid = params['guid']; 
      this.name = params['name'];
    });

  this.queryParamSubscription = this.route.queryParams.subscribe({
      next: params => {
        const tab: number = +params['tab'];
        const abc: stirng = params['abc']
      }, 
      error: e => { },
      complete: () => {}
    });
  */

  const guid = route.paramMap.get('guid');
  const name = route.paramMap.get('name'); 
  return memberService.getMemberByGuid(guid!);
};
