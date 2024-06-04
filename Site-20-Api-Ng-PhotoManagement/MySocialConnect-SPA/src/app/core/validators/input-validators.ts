
/*
1. only one input argument is expected, which is of type AbstractControl
2. the validator function can obtain the value to be validated via the control.value property
3. the validator function needs to return null if no errors were found in the field value, meaning that the value is valid
4. if any validation errors are found, the function needs to return an object of type ValidationErrors 
5. the ValidationErrors object can have as properties the multiple errors found (usually just one), and as values the details about each error.
6. the value of the ValidationErrors object can be an object with any properties that we want, allowing us to provide a lot of useful information about the error if needed
picked from blog angular

Important
1.	https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/ 
2.	https://blog.angular-university.io/angular-custom-validators/ without service and using validators
3.	https://www.thisdot.co/blog/using-custom-async-validators-in-angular-reactive-forms 
4.	also check TemplateFormValidatorsService
Check 
    003 Project4 -Ng-login-reg-nav-services-templateforms.docx. 
    010 Project11 -Ng-reactiveforms-validators-reusable-controls.docx

Check for local validator for matchValues in /site/register-reactiveform.component.ts as well
*/

import { AbstractControl, AsyncValidatorFn, ValidationErrors, ValidatorFn } from "@angular/forms";
import { AccountService } from "../services/account.service";
import { Observable, map, of } from "rxjs";

export class InputValidator{
    
    constructor(){}

    //two functions to match values, check /site/register-reactiveform.component.ts

    static matchValues(source: string, target: string): ValidatorFn{
        return (control: AbstractControl) : ValidationErrors | null => {
            const sourceControl = control.get(source);
            const targControl = control.get(target);

            if(!sourceControl || !targControl) return null;

            if(targControl.errors && !targControl.hasError('notMatching')) return null;

            if(sourceControl.value !== targControl.value){
                targControl.setErrors({ notMatching: true });
                return ({ notMatching: true });
            }

            targControl.setErrors(null);
            return null;
        }
    }

    static matchValuesTwo(matchTo: string): ValidatorFn{
        return (control: AbstractControl) : ValidationErrors | null => {

            const matchToControl = control.parent?.get(matchTo);

            if(!matchToControl) return null;

            if(control.errors && !control.hasError('notMatchingTwo')) return null;

            if(control.value !== matchToControl.value){
                matchToControl.setErrors({ notMatchingTwo: true });
                return ({ notMatchingTwo: true });
            }

            matchToControl.setErrors(null);
            return null;
        }
    }

    static passwordStrength(): ValidatorFn{
        return (control: AbstractControl) : ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (!value) return null;

            /*
            const hasUpperCase = /[A-Z]+/.test(value);
            const hasLowerCase = /[a-z]+/.test(value);
            const hasNumeric = /[0-9]+/.test(value);
            const hasSpecialChar = /[*@!#%&()^~{}]+/.test(value);
            const hasLength = value.toString().length >= 10 && value.toString().length <= 16;

            const valid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecialChar && hasLength;

            if(valid) return null;

            //bad
            //return { passwordStrengthValidator: true };
            return {
                passwordStrength: {
                    hasUpperCase: hasUpperCase, 
                    hasLowerCase: hasLowerCase,
                    hasNumeric: hasNumeric,
                    hasSpecialChar: hasSpecialChar, 
                    hasLength: hasLength
                }
            };
            */

            //if(control.errors && !control.hasError('passwordStrength')) return null;

            //single regex
            const regex = new RegExp(/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{10,16}$/);
            const valid = regex.test(control.value);

            if(!valid){
                control.setErrors({ passwordStrength: true });
                return ({ passwordStrength: true });
            }

            control.setErrors(null);
            return null;
        }
    }

    //async validator
    static userNameExistAsync(accountService: AccountService): AsyncValidatorFn{
        return(control: AbstractControl) : Observable<ValidationErrors | null> => {
            if(!control.value) return of(null);

            if(control.errors && !control.hasError("userNameExistAsync")) return of(null);

            return accountService.checkUser(control.value).pipe(
                map((result: boolean) =>{
                    return result ? {userNameExistAsync: true } : null
                })
            );
        }
    }

    static userNameAllowed(): ValidatorFn{
        return (control: AbstractControl) : ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (!value) return null;

            if(control.errors && !control.hasError('userNameAllowed')) return null;

            const regex = new RegExp(/^[a-zA-Z][A-Za-z0-9]+(?:[_-][A-Za-z0-9]+)*$/);
            const valid = regex.test(control.value);

            if(!valid){
                control.setErrors({ userNameAllowed: true });
                return ({ userNameAllowed: true });
            }

            control.setErrors(null);
            return null;
        }
    }

    static onlyAlphaNumeric(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (value === '') return null;
            
            //if(control.errors && !control.hasError('onlyAlphaNumeric')) return null;

            const regex = new RegExp(/^[a-zA-z0-9]*$/);
            const valid = regex.test(control.value);

            if(!valid){
                control.setErrors({ onlyAlphaNumeric: true });
                return ({ onlyAlphaNumeric: true });
            }

            control.setErrors(null);
            return null;
        };
    }

    static onlyChar(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (value === '') return null;
            
            const regex = new RegExp(/^[a-zA-z]*$/);
            const valid = regex.test(control.value);

            if(!valid){
                control.setErrors({ onlyChar: true });
                return ({ onlyChar: true });
            }

            control.setErrors(null);
            return null;
        };
    }

    static onlyCharWithSpace(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (value === '') return null;

            const regex = new RegExp(/^[a-zA-z ]*$/);
            const valid = regex.test(control.value);

            if(!valid){
                control.setErrors({ onlyCharWithSpace: true });
                return ({ onlyCharWithSpace: true });
            }

            control.setErrors(null);
            return null;
        };
    }
}