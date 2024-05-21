import { Injectable } from '@angular/core';
import { HelperService } from './helper.service';
import { HttpClientService } from './http-client.service';
import { UserDto } from '../models-interfaces/user-dto.model';
import { SiteRole } from '../models-interfaces/site-role.model';
import { ZRoles } from '../enums/z-roles';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private _siteRoles: SiteRole[] = [];

  constructor(private helperService: HelperService, private httpClientService: HttpClientService) { }

  //build static list of site roles
  getSiteRoles(): SiteRole[]{
    if(this._siteRoles && this._siteRoles.length > 0)
      return this._siteRoles;

    this._siteRoles = [
      new SiteRole(ZRoles.Admin, ZRoles.Admin, false),
      new SiteRole(ZRoles.Moderator, ZRoles.Moderator, false),
      new SiteRole(ZRoles.Member, ZRoles.Member, false)
    ];
    return this._siteRoles;
  }

  //mark site roles as selected or not selected per the user roles
  translateUserRolesToSiteRoles(userRoles: string[]) : SiteRole[]{
    const roles: SiteRole[] = [];
    const siteRoles: SiteRole[] = [...this.getSiteRoles()];

    siteRoles.forEach(role => {
      let isSelected = false;
      if(userRoles && userRoles.length > 0)
        isSelected = userRoles.includes(role.name);
      
      role.isSelected = isSelected;
      roles.push(role);
    })
    return roles;
  }

  /*
  Pass back an array of object so you Partial<UserDto>
  [{"id": 11,"userName": "admin","displayName": "Admin","guId": "4ce8e019-d3d1-4744-8e49-9e97737867b7","roles": ["Admin","Moderator"]}]  
  */
  getUsersWithRoles()
  {
    const url = this.helperService.urlAdminGetUsersWithRoles;
    this.helperService.logIfFrom(url, "AdminService getUsersWithRoles");
    return this.httpClientService.get<Partial<UserDto[]>>(url);
  }

  //passing query string / query parameters with post 
  updateUserRoles(userGuid: string, roles: string[]){
    let url = this.helperService.replaceKeyValue(this.helperService.urlAdminEditRoles, this.helperService.keyGuid, userGuid);
    this.helperService.logIfFrom(url, "AdminService updateUserRoles")
    this.helperService.logIf(roles);
    //if(roles && roles.length > 0) url += "?roles" + roles;
    const paramsSet = new HttpParams().set("roles", roles.join(","));
    let httpOptions = { params: paramsSet };
    return this.httpClientService.post<string[]>(url, {}, httpOptions);
  }
}
