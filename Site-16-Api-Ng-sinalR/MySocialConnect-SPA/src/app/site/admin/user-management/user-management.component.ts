import { Component, OnDestroy, OnInit, afterNextRender } from '@angular/core';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';

import { AdminService } from '../../../core/services/admin.service';
import { HelperService } from '../../../core/services/helper.service';

import { SiteRole } from '../../../core/models-interfaces/site-role.model';
import { UserDto } from '../../../core/models-interfaces/user-dto.model';

import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit, OnDestroy {
  
  users: Partial<UserDto[]> = [];
  usersSubscription!: Subscription;
  rolesUpdateSubscription!: Subscription;

  //modal reference for modal window
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();

  constructor(private helperService: HelperService, 
              private adminService: AdminService,
              private toastr: ToastrService, 
              private modalService: BsModalService){
  }

  ngOnDestroy(): void {
    if(this.usersSubscription) this.usersSubscription.unsubscribe();
    if(this.rolesUpdateSubscription) this.rolesUpdateSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadUserWithRoles();
  }

  loadUserWithRoles(){
    this.usersSubscription = this.adminService.getUsersWithRoles().subscribe({
      next: (users: Partial<UserDto[]>) => {
        this.users = users;
      }
    });
  }

  openRolesModal(user?: UserDto) {
    if(!user){
      this.toastr.error("Unable to get user", "Error");
      return;
    }

    //convert the user assigned roles to site roles and mark isSelected=true for the roles that are assigned
    var roles = this.adminService.translateUserRolesToSiteRoles(user.roles);
    this.helperService.logIfFrom(user.displayName, "openRolesModal");
    this.helperService.logIf(roles);
  
    this.handleRolesModal(user, roles);
  }

  private handleRolesModal(user: UserDto, roles: SiteRole[]){
    //default is overwritten check followig for the defaults
    //https://valor-software.com/ngx-bootstrap/#/components/modals?tab=overview
    
    /*
    const modalOptions: ModalOptions = {
      initialState: {
        roles: roles,
        user: user
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, modalOptions);
    */

    //spread operator doesnt work, deep copy not happening
    //const newRolesObject = [...roles.slice()]; 
    const newRolesObject = JSON.parse(JSON.stringify(roles)) as typeof roles;
    const config = {
      class: 'modal-dialog-center',
      initialState: {
        roles: newRolesObject,
        user: user
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    
    //this.bsModalRef.content!.closeBtnName = 'Close'; just local button

    //on modal hide/save do the stuff
    //subscribe for updatedRoles, in this case do not do onHide subscription
    this.bsModalRef.content!.updatedRoles.subscribe({
      next: (rolesFromModal: SiteRole[]) => {
        this.performRoleUpdate(rolesFromModal, user, roles);
      }
    })

    //check for isSave=true if subscribing onHide. this case do not do above updatedRoles.subscribe 
    /*
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        //get the roles from modal
        const rolesFromModal = this.bsModalRef.content?.roles;
        //get the isSave flag as well
        const isSaveFromModal = this.bsModalRef.content?.isSave;

        if(!isSaveFromModal) return;

        this.performRoleUpdate(rolesFromModal!, user, roles);
      }
    });
    */
  }

  private performRoleUpdate(rolesFromModal: SiteRole[], user: UserDto, roles: SiteRole[]){
    if(!rolesFromModal || rolesFromModal.length <= 0){
      this.toastr.error("No data found!", "No data")
      return;
    }

    if(this.arrRolesEqual(roles, rolesFromModal!)){
      this.toastr.info("No changes detected!", "No change")
      return;
    }

    //pick the selected roles
    var selected = [...rolesFromModal!.filter(r => r.isSelected === true).map(m => m.name)];
    if(!selected || selected.length <= 0){
      this.toastr.info("No selected roles detected!", "No selection")
      return;
    }

    //do update
    this.doRoleUpdate(user, selected);
  }

  private arrRolesEqual(rolesInitial: SiteRole[], rolesAfter: SiteRole[]){
    return JSON.stringify(rolesInitial.sort()) === JSON.stringify(rolesAfter);
  }

  private doRoleUpdate(user: UserDto, selectedRoles: string[]){
    this.rolesUpdateSubscription = this.adminService.updateUserRoles(user.guId, selectedRoles).subscribe({
      next: (roles: string[]) => {
        //update the user roles
        if(roles){
          user.roles = [...roles];
          user.roles.sort();
        }
        this.toastr.success("Roles updated successfully!", "Success");
      }
    })
  }

}
