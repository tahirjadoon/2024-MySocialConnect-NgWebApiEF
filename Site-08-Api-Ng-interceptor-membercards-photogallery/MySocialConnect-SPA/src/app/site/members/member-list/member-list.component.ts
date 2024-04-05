import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserDto } from 'src/app/core/models-interfaces/user-dto.model';
import { MemberService } from 'src/app/core/services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit, OnDestroy {
  members: UserDto[] = [];

  membersSubscription!: Subscription;

  constructor(private memberService: MemberService){}

  ngOnDestroy(): void {
    if(this.membersSubscription) this.membersSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers(){
    this.membersSubscription = this.memberService.getMembers().subscribe({
      next: (members: UserDto[]) => {
        this.members = members;
      },
      error: e => {},
      complete: () => {}
    })
  }

}
