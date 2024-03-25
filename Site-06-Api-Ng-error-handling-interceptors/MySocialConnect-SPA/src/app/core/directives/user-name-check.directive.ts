import { Directive, forwardRef } from '@angular/core';
import { AbstractControl, NG_ASYNC_VALIDATORS, Validator } from '@angular/forms';
import { TemplateFormValidatorsService } from '../services/template-form-validators.service';
import { Observable } from 'rxjs';

//https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/
//also check TemplateFormValidatorsService

@Directive({
  selector: '[appUserNameCheck]',
  providers: [{ provide: NG_ASYNC_VALIDATORS, useExisting: forwardRef(() => UserNameCheckDirective), multi: true }]
})
export class UserNameCheckDirective implements Validator {

  constructor(private  tfvs: TemplateFormValidatorsService) { }

  validate(control: AbstractControl): Promise<{} | null> | Promise<{ [key: string]: any }> | Observable<{ [key: string]: any }> {
    const result = this.tfvs.CheckUserNameTakenValidator(control);
    return result;
  }
}
