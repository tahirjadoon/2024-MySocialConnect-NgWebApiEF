import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { AbstractControl, AbstractControlOptions, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { HelperService } from '../../core/services/helper.service';
import { AccountService } from '../../core/services/account.service';

import { UserRegisterDto } from '../../core/models-interfaces/user-register-dto.model';

import { InputValidator } from '../../core/validators/input-validators';

@Component({
  selector: 'app-register-reactiveform-reusablecontrols',
  templateUrl: './register-reactiveform-reusablecontrols.component.html',
  styleUrls: ['./register-reactiveform-reusablecontrols.component.css']
})
export class RegisterReactiveformReusablecontrolsComponent implements OnInit, OnDestroy {
   //tell the parent homeComponent that cancel is clicked
   @Output('cancelRegister') cancelRegister = new EventEmitter();

   //register: UserRegisterDto = <UserRegisterDto>{};
 
   //reactive form
   registerForm!: FormGroup;
 
   registerSubscription!: Subscription;

   //must be minimum 18 years old
    maxDate!: Date;

    validationErrors: string[] = [];

    //gender list
    genderList = [
      { id: 'female', value: 'female', label: 'Female' },
      { id: 'male', value: 'male', label: 'Male' },
    ];
 
   constructor(private helperService: HelperService, private accountService: AccountService, 
     private toastr: ToastrService, 
     private fb: FormBuilder, 
     private router: Router){}
 
   ngOnDestroy(): void {
     if(this.registerSubscription) this.registerSubscription.unsubscribe();
   }
 
   ngOnInit(): void {
     //this.initializeFormWithLocalValidator();  
     //this.initializeForm(); //this uses /code/validators
     this.initializeForm_FormBuilder();

     this.maxDate = new Date();
     this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
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
       userName: new FormControl('', [Validators.required, Validators.minLength(5)]),
       password: new FormControl('', [Validators.required, Validators.minLength(10)]),
       confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
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
         userName: new FormControl('', 
                                     [Validators.required, Validators.minLength(5), InputValidator.userNameAllowed()], 
                                     [InputValidator.userNameExistAsync(this.accountService)]),
         password: new FormControl('', [Validators.required, Validators.minLength(10), InputValidator.passwordStrength()]),
         confirmPassword: new FormControl('', [
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

   initializeForm_FormBuilder(){
      this.registerForm = this.fb.group({
        userName: ['', 
                  [Validators.required, Validators.minLength(5), InputValidator.userNameAllowed()], 
                  [InputValidator.userNameExistAsync(this.accountService)]],
        password: ['', [Validators.required, Validators.minLength(10), InputValidator.passwordStrength()]],
        confirmPassword: ['', [Validators.required ]],
        gender: ['male'],
        displayName: ['', [Validators.required, Validators.minLength(5), InputValidator.onlyAlphaNumeric()]],
        dateOfBirth: ['', [Validators.required]],
        city: ['', [Validators.required, InputValidator.onlyCharWithSpace()]],
        country: ['', [Validators.required, InputValidator.onlyCharWithSpace()]],
      },
      {validators: [InputValidator.matchValues('password','confirmpassword')]} as AbstractControlOptions,
      );
   }
 
   onCancel(){
     this.helperService.logIfFrom("Register Cancelled", "OnCancel TemplateForm");
     this.cancelRegister.emit(false);
   }
 
   onRegister(){
     this.helperService.logIfFrom(this.registerForm?.value, "Register Reactive");
     this.validationErrors = [];//reset
     //when invalid do not proceed further
     if (!this.isReactiveFormGood()) return;
     
    //do registration
     this.doRegisteration();
   }

   private isReactiveFormGood(): boolean {
    if (this.registerForm.invalid) {
      this.showErrorsOnSubmit();
      this.toastr.error("Please fix errors and try again", "Validation Error(s)")
      return false;
    }
    return true;
  }

  private showErrorsOnSubmit() {
    Object.keys(this.registerForm.controls).forEach(field => {
      const control = this.registerForm.get(field);
      if (control?.errors)
        control.markAsTouched({onlySelf: true});
    });
  }

  private getDateOnly(dob: string | undefined){
    if(!dob) return;
    let theDob = new Date(dob);
    let isoDateString = new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString();
    let theDate = isoDateString.slice(0, 10); //get the characters from 0 to 10

    this.helperService.logIfFrom(isoDateString, "getDateOnly isoDateString");
    this.helperService.logIfFrom(theDate, "getDateOnly theDate");

    return theDate;
  }

  private getControlValue(key: string): any{
    //this.registerForm.value['userName']
    //or
    //this.registerForm.controls['userName'].value
    return this.registerForm.controls[key].value;
  }

  private mapFormToUserRegisterDto(): UserRegisterDto{
    const registerUser = new UserRegisterDto(this.getControlValue('userName'),
                                              this.getControlValue('password'),
                                              this.getControlValue('confirmPassword'),
                                              this.getControlValue('gender'),
                                              this.getControlValue('displayName'),
                                              this.getControlValue('dateOfBirth'),
                                              this.getControlValue('city'),
                                              this.getControlValue('country')
                                            );
    this.helperService.logIfFrom(registerUser, "mapFormToUserRegisterDto registerUser");
    return registerUser;
  }

  private mapRawValuesToUserRegisterDto(rawvalues: any): UserRegisterDto{
    const registerUser = new UserRegisterDto(rawvalues.userName,
                                              rawvalues.password,
                                              rawvalues.confirmPassword,
                                              rawvalues.gender,
                                              rawvalues.displayName,
                                              rawvalues.dateOfBirth,
                                              rawvalues.city,
                                              rawvalues.country
                                            );
    this.helperService.logIfFrom(registerUser, "mapRawValuesToUserRegisterDto registerUser");
    return registerUser;
  }

  private doRegisteration() {
    const dob = this.getDateOnly(this.getControlValue('dateOfBirth'));
    //fill
    /*
    1. can use this.registerForm.value
    2. const registerUser = this.mapFormToUserRegisterDto();
    3. use spread operator, which we are doing below as we ned to overwite the dob
    */
    var rawvalues = {...this.registerForm.value, dateOfBirth: dob};
    this.helperService.logIfFrom(rawvalues, "doRegisteration rawvalues");
    
    //can call like
    //this.registerSubscription = this.accountService.register(rawvalues)
    //or like
    const registerUser2 = this.mapRawValuesToUserRegisterDto(rawvalues);

    this.registerSubscription = this.accountService.register(registerUser2).subscribe({
      next: () => {
        //go to members page
        this.router.navigateByUrl('/members');
      },
      error: e => {
        //due to error intercepter we are getting a flat array of validation items so for modal validation need to check that
        //check array and length > 0
        //other cases the error interceptor is displaying the error
        if(e?.length){
          this.toastr.error("An error occured");
          this.validationErrors = e;
        }
      }
    });
    
  }
}
