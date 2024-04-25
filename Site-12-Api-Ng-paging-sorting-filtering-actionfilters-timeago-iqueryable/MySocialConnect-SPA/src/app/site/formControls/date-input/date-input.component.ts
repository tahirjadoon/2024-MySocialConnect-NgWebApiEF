import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() label: string = '';
  @Input() placeHolder: string = '';
  @Input() maxDate!: Date;

  //config for datepicker, partial infront makes all the properties optional
  bsConfig: Partial<BsDatepickerConfig> | undefined;

  //Self 
  //angular will check if it is used recently and will reuse as kept in memory
  //when it comes to inputs we do not want to resue any other control that was already in memory
  //we want to make sure that this NgControl is unique to the inputs that we are updating to in the DOM
  constructor(@Self() public ngControl: NgControl){
    this.ngControl.valueAccessor = this; //this represents DateInputComponent class

    this.bsConfig = {
      containerClass: 'theme-red', //default is theme-green
      dateInputFormat: 'YYYY-MM-DD', //DD MMMM YYYY
    }
  }
  
  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {}
  
  registerOnTouched(fn: any): void {}
  
  //optional so commented
  setDisabledState?(isDisabled: boolean): void {}

  get getcontrol(): FormControl{
    return this.ngControl.control as FormControl;
  }

}
