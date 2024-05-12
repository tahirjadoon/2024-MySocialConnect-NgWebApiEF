import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { MemberService } from '../../../core/services/member.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit, OnDestroy {

  @Input() memberIn: UserDto | undefined; // = <UserDto>{};

  addLikeSubscription!: Subscription;

  constructor(private memberService: MemberService, private toastrService: ToastrService){}

  ngOnDestroy(): void {
    
  }

  ngOnInit(): void {
    
  }

  onAddLike(member: UserDto){
    this.addLikeSubscription = this.memberService.addLike(member.id).subscribe({
      next: () => {
        this.toastrService.success(`You have liked ${member.displayName}`);
      },
      error: e => {
        //no need to do something here since interceptor will display
      },
      complete: () => {
        //no need to do something here since interceptor will display
      }
    })
  }


}
