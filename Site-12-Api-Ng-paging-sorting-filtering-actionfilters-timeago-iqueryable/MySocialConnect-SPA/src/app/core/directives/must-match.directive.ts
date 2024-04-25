import { Directive, Input } from '@angular/core';
import { AbstractControl, FormGroup, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';
import { TemplateFormValidatorsService } from '../services/template-form-validators.service';

//https://www.freecodecamp.org/news/how-to-validate-angular-template-driven-forms/
//https://blog.angular-university.io/angular-custom-validators/ without service and using validators
//also check TemplateFormValidatorsService
@Directive({
  selector: '[appMustMatch]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MustMatchDirective, multi: true }]
})
export class MustMatchDirective implements Validator {
  @Input('appMustMatch') MustMatch: string[] = []

  constructor(private tfvs: TemplateFormValidatorsService) { }

  validate(formGroup: FormGroup): ValidationErrors | null {
    const result = this.tfvs.MustMatchValidator(this.MustMatch[0], this.MustMatch[1])(formGroup);
    return result;
  }
}
