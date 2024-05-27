import { Component, EventEmitter, Input } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';

import { SiteRole } from '../../../core/models-interfaces/site-role.model';
import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { ZRoles } from '../../../core/enums/z-roles';


@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent {
  //when changes have been made and X/Cancel clicked, the roles are still getting updated. 
  //In this case no update should happen. 
  //Two ways, to do this. either set a boolean flag on Save or emit an input 
  isSave: boolean = false;
  @Input() updatedRoles = new EventEmitter();


  user: UserDto = <UserDto>{};
  roles: SiteRole[] = [];

  hasChanges: boolean = false;

  zRole = ZRoles;

  constructor(public bsModalRef: BsModalRef, private toastr: ToastrService){
  }

  onCheckedChangeRole(roleValue: any){
    this.roleChange(roleValue);
    this.hasChanges = true;
  }

  private roleChange(roleValue: any){
    const role = this.roles.find(x => x.value == roleValue);
    if(!role) return;
    role.isSelected = !role.isSelected;
  }

  onUpdateSelectedRoles(){
    if(!this.hasChanges){
      this.toastr.info("No changes made!", "No Changes");
      this.bsModalRef.hide();
    }

    const selectedItems = this.roles.filter(x => x.isSelected === true);
    if(!selectedItems || selectedItems.length <= 0){
      this.toastr.error(`Atleast member role must be added for ${this.user.displayName}`, "No Roles");
      return;
    }

    //all users must have a member role
    const memberRole = selectedItems.find(x => x.name === this.zRole.Member);
    if(!memberRole){
      this.toastr.info(`Every user must be a ${this.zRole.Member} also so this will be added automatically.`);
      selectedItems.push(new SiteRole(this.zRole.Member, this.zRole.Member, true));
      this.roleChange(this.zRole.Member);
    }

    //either set the flag to say Save was clicked
    this.isSave = true;
    //or emit an input 
    this.updatedRoles.emit(this.roles);
    this.bsModalRef.hide();

    //after hide the rest of the work will be performed by the user-management.component.ts
  }

}
