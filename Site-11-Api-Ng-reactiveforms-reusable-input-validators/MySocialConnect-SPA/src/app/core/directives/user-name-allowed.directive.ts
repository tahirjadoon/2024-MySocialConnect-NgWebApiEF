import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, Validator } from '@angular/forms';
import { TemplateFormValidatorsService } from '../services/template-form-validators.service';

//https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/
//also check TemplateFormValidatorsService

@Directive({
  selector: '[appUserNameAllowed]',
  providers: [{ provide: NG_VALIDATORS, useExisting: UserNameAllowedDirective, multi: true }]
})
export class UserNameAllowedDirective implements Validator {

  constructor(private tfvs: TemplateFormValidatorsService) { }

  validate(control: AbstractControl): { [key: string]: any } | null {
    const result = this.tfvs.UserNameAllowedValidator()(control);
    return result;
  }
}
