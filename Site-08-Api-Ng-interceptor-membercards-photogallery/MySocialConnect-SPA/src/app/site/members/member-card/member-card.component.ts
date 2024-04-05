import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UserDto } from 'src/app/core/models-interfaces/user-dto.model';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit, OnDestroy {

  @Input() memberIn: UserDto | undefined; // = <UserDto>{};

  constructor(){}

  ngOnDestroy(): void {
    
  }

  ngOnInit(): void {
    
  }


}
