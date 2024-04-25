import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  //properties
  @Input() label: string = '';
  @Input() labelMustMatch: string = '';
  @Input() placeHolder: string = '';
  @Input() type: string = 'text';

  //Self 
  //angular will check if it is used recently and will reuse as kept in memory
  //when it comes to inputs we do not want to resue any other control that was already in memory
  //we want to make sure that this NgControl is unique to the inputs that we are updating to in the DOM
  constructor(@Self() public ngControl: NgControl){
    this.ngControl.valueAccessor = this; //this represents TextInputComponent class
  }

  //no need to write code in any of the below nethods that got implemented due to ControlValueAccessor
  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {}

  registerOnTouched(fn: any): void {}

  //optional so commented
  //setDisabledState?(isDisabled: boolean): void {}

  get getcontrol() : FormControl{
    return this.ngControl.control as FormControl;
  } 
}
