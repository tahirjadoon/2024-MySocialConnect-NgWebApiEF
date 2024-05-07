import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';
import { TemplateFormValidatorsService } from '../services/template-form-validators.service';

//https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/
//https://blog.angular-university.io/angular-custom-validators/ without service and using validators
//also check TemplateFormValidatorsService

@Directive({
  selector: '[appPasswordStrength]',
  providers: [{ provide: NG_VALIDATORS, useExisting: PasswordStrengthDirective, multi: true }]
})
export class PasswordStrengthDirective implements Validator {

  constructor(private tfvs: TemplateFormValidatorsService) { }

  validate(control: AbstractControl): { [key: string]: any } | null {
    const result = this.tfvs.PasswordStrengthValidator()(control);
    return result;
  }

}
