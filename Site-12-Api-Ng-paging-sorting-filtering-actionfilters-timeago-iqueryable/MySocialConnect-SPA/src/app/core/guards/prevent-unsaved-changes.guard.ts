import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../../site/members/member-edit/member-edit.component';

//this will not prevent when user will type a different URL
//for different URL check member-edit.component.ts

//export const preventUnsavedChangesGuard: CanDeactivateFn<unknown> = (component, currentRoute, currentState, nextState) => {
export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  if(component.editForm?.dirty){
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost.');
  }
  return true;
};
