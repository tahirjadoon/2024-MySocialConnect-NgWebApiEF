import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { AbstractControl, AbstractControlOptions, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';

import { HelperService } from '../../core/services/helper.service';
import { AccountService } from '../../core/services/account.service';

import { UserRegisterDto } from '../../core/models-interfaces/user-register-dto.model';

import { InputValidator } from '../../core/validators/input-validators';

@Component({
  selector: 'app-register-reactiveform',
  templateUrl: './register-reactiveform.component.html',
  styleUrls: ['./register-reactiveform.component.css']
})
export class RegisterReactiveformComponent implements OnInit, OnDestroy {
  

  //tell the parent homeComponent that cancel is clicked
  @Output('cancelRegister') cancelRegister = new EventEmitter();

  register: UserRegisterDto = <UserRegisterDto>{};

  //reactive form
  registerForm!: FormGroup;

  registerSubscription!: Subscription;

  constructor(private helperService: HelperService, private accountService: AccountService, 
    private toastr: ToastrService){}

  ngOnDestroy(): void {
    if(this.registerSubscription) this.registerSubscription.unsubscribe();
  }

  ngOnInit(): void {
    //this.initializeFormWithLocalValidator();  
    this.initializeForm(); //this uses /code/validators
  }

  //convenience getter for easy access to form fields
  get rf() {
    return this.registerForm.controls;
  }

  rf2(key: string) {
    return this.registerForm.get(key) as FormControl;
  }

  initializeFormWithLocalValidator(){
    this.registerForm = new FormGroup({
      username: new FormControl('', [Validators.required, Validators.minLength(5)]),
      password: new FormControl('', [Validators.required, Validators.minLength(10)]),
      confirmpassword: new FormControl('', [Validators.required, this.matchValues('password')]),
    });
    //above will only do validation when the confirm passwors is changed. We need to do the same when password is changed as well
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmpassword'].updateValueAndValidity()
    });
  }

  //using local validator
  matchValues(matchTo: string): ValidatorFn{
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  //two different approaches used here for the password and conform password match check 
  //either use matchValue or matchValueTwo do not use both
  //password strength returns multiple items check back
  initializeForm(){
  
    this.registerForm = new FormGroup({
        username: new FormControl('', 
                                    [Validators.required, Validators.minLength(5), InputValidator.userNameAllowed()], 
                                    [InputValidator.userNameExistAsync(this.accountService)]),
        password: new FormControl('', [Validators.required, Validators.minLength(10), InputValidator.passwordStrength()]),
        confirmpassword: new FormControl('', [
          Validators.required //, InputValidator.matchValuesTwo('password')
        ]),
      },
      {validators: [InputValidator.matchValues('password','confirmpassword')]} as AbstractControlOptions,
    );

    //second approach for MatchValuesTwo
    //above will only do validation when the confirm passwors is changed. We need to do the same when password is changed as well
    /*
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmpassword'].updateValueAndValidity()
    });
    */
  }

  onCancel(){
    this.helperService.logIfFrom("Register Cancelled", "OnCancel TemplateForm");
    this.cancelRegister.emit(false);
  }

  onRegister(){
    this.helperService.logIfFrom(this.registerForm?.value, "Register Reactive");
    /*
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
    */
  }

}
