import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';

import { ToastrService } from 'ngx-toastr';

import { HelperService } from '../../core/services/helper.service';
import { AccountService } from 'src/app/core/services/account.service';

import { UserRegisterDto } from '../../core/models-interfaces/user-register-dto.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {

  //tell the parent homeComponent that cancel is clicked
  @Output('cancelRegister') cancelRegister = new EventEmitter();

  register: UserRegisterDto = <UserRegisterDto>{};

  registerSubscription!: Subscription 

  constructor(private helperService: HelperService, private accountService: AccountService, 
    private toastr: ToastrService){}
  
  ngOnInit(): void {
    
  }

  ngOnDestroy(): void {
    if(this.registerSubscription) this.registerSubscription.unsubscribe();
  }

  onCancel(){
    this.helperService.logIfFrom("Register Cancelled", "OnCancel TemplateForm");
    this.cancelRegister.emit(false);
  }

  onRegister(){
    this.helperService.logIfFrom(this.register, "onRegister TemplateForm");
    this.registerSubscription = this.accountService.register(this.register).subscribe({
      next: () => {
        this.helperService.logIfFrom("", "register template driven");
        this.onCancel();
      },
      error: e => {
        this.helperService.logIfError(e, "register error template driven");
        this.toastr.error(e.error);
      },
      complete: () => {
        this.helperService.logIfFrom("register complete", "register template driven");
      }
    });
  }
}
