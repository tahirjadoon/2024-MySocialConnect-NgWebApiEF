import { Injectable } from '@angular/core';
import { AbstractControl, FormGroup, ValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';

import { AccountService } from './account.service';

//https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/
@Injectable({
  providedIn: 'root'
})
export class TemplateFormValidatorsService {

  constructor(private accountService: AccountService) { }

  //assign via directive appMustMatch. Check register form - TemplateDriven one
  MustMatchValidator(source: string, target: string) {
    return(formGroup: FormGroup) => {
      const sourceControl = formGroup.get(source);
      const targetControl = formGroup.get(target);

      if (!targetControl || !sourceControl)
          return null;

      if (targetControl.errors && !targetControl.errors["mustMatch"])
          return null;

      //check 
      if (sourceControl && targetControl && sourceControl.value !== targetControl.value) {
          targetControl.setErrors({ mustMatch: true });
          return ({ mustMatch: true });
      }

      targetControl.setErrors(null);
      return null;

    }
  }

  //assign via directive appPasswordStrength
  PasswordStrengthValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) {
        return null;
      }
      const regex = new RegExp(/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{10,16}$/);
      const valid = regex.test(control.value);
      return valid ? null : { passwordStrength: true };
    };
  }

  //assign via appUserNameAllowed directive
  UserNameAllowedValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) {
        return null;
      }
      const regex = new RegExp(/^[a-zA-Z][A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*$/);
      const valid = regex.test(control.value);
      return valid ? null : { userNameAllowed: true };
    };
  }

  //assign via appUserNameCheck directive
  CheckUserNameTakenValidator(control: AbstractControl): Promise<{} | null> | Promise<{ [key: string]: any }> | Observable<{ [key: string]: any }> {
    return new Promise(resolve => {
      if(control.errors?.['required'] || control.errors?.['minlength'] || control.errors?.['userNameAllowed'])
        resolve(null);
      else if(!control.value)
        resolve(null);
      else{
        this.accountService.checkUser(control.value).subscribe({
          next: response => {
            if(response){
              resolve({ userNameNotAvailable: true }); //tap into this
            }
            else{
              resolve(null); 
            }
          },
          error: e => {
            resolve(null);
          },
          complete: () => {}
        });
      }
    });
  }
}
