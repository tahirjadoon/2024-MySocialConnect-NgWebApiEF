import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Observable, map, of } from 'rxjs';

import { LocalStorageService } from './local-storage.service';
import { HelperService } from './helper.service';
import { HttpClientService } from './http-client.service';

import { AppConstants } from '../constants/app-constants';

import { UserDto } from '../models-interfaces/user-dto.model';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  //to pass the auth token to the api, later will use interceptor
  private httpOptions;

  //state mangement. We dont want to pull the user info every time
  members: UserDto[] = [];

  constructor(private localStorageService: LocalStorageService, 
            private helperService: HelperService, 
            private httpClientService: HttpClientService) { 
    //to pass the auth token to the api, later will user interceptor
    this.httpOptions = {
      headers: new HttpHeaders({
        Authorization: AppConstants.Bearer + this.localStorageService.getUserToken
      })
    };
  }

  /**
   * A GET users method
   * @returns Observable userDto[]
   */
  getMembers(): Observable<UserDto[]>{
    
    //state
    if(this.members.length > 0){
      return of(this.members);
    }

    const url = this.helperService.urlUsersAll;
    this.helperService.logIfFrom(url, "memberService getMembers");

    //return this.httpClientService.get<UserDto[]>(url, this.httpOptions)
    return this.httpClientService.get<UserDto[]>(url, this.httpOptions) //jwt interceptor
    .pipe(
      map((members: UserDto[]) => {
        this.members = members;
        return members;
      })
    );
  }

  /**
   * A GET user by guid method
   * @returns Observable userDto
   */
  getMemberByGuid(guid: string): Observable<UserDto>{
    
    //state
    if(this.members.length > 0){
      const member = this.members.find(x => x.guId === guid);
      if(member){
        return of(member);
      }
    }

    const url = this.helperService.replaceKeyValue(this.helperService.urlUserGetByGuid, this.helperService.keyGuid, guid, "");
    this.helperService.logIfFrom(url, "memberService getMemberByGuid");

    //return this.httpClientService.get<UserDto>(url, this.httpOptions)
    return this.httpClientService.get<UserDto>(url, this.httpOptions) //jwt interceptor
      .pipe(
        map((member: UserDto) => {
          return member;
        })
      );
  }

  updateMember(user: UserDto){
    const url = this.helperService.urlUserUpdate;
    this.helperService.logIfFrom(url, "memberService updateMember");

    return this.httpClientService.put(url, user).pipe(
      map(() => {
        const index = this.members.indexOf(user);
        //this.members[index] = user;
        this.members[index] = {...this.members[index], ...user};
      })
    );
  }

  setMainPhoto(photoId: number){
    const url = this.helperService.replaceKeyValue(this.helperService.urlUserPhotoSetMain, this.helperService.keyPhotoId, photoId, "");
    this.helperService.logIfFrom(url, "memberService setMainPhoto");
    return this.httpClientService.put(url, {});
  }

  deletePhoto(photoId: number){
    const url = this.helperService.replaceKeyValue(this.helperService.urlUserPhotoDelete, this.helperService.keyPhotoId, photoId, "");
    this.helperService.logIfFrom(url, "memberService deletePhoto");
    return this.httpClientService.delete(url);
  }

}
