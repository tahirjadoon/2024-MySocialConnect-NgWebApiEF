import { Component, OnInit } from '@angular/core';
import { HelperService } from '../../core/services/helper.service';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  title = "";
  registerMode = false;

  constructor(private helperService: HelperService, public accountService: AccountService){}

  ngOnInit(): void {
    this.title = this.helperService.Title;
  }

  onRegisterToggle(){
    this.registerMode = !this.registerMode;
  }

  onCancelRegisterMode(event: boolean){
    this.registerMode = event;
  }
}
