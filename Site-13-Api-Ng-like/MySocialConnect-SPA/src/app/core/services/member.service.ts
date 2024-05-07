import { Injectable } from '@angular/core';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map, of, take } from 'rxjs';

import { LocalStorageService } from './local-storage.service';
import { HelperService } from './helper.service';
import { HttpClientService } from './http-client.service';
import { AccountService } from './account.service';

import { AppConstants } from '../constants/app-constants';

import { UserDto } from '../models-interfaces/user-dto.model';
import { UserParamsDto } from '../models-interfaces/user-params-dto.model';
import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';
import { LikeParamsDto } from '../models-interfaces/like-params-dto.model';
import { PaginatedResult } from '../models-interfaces/pagination/paginated-result.model';

import { ZMemberGetBy } from '../enums/z-member-get-by';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  //to pass the auth token to the api, later will use interceptor
  private httpOptions;

  //state mangement. We dont want to pull the user info every time
  private members: UserDto[] = []; //not using it any more due to pagination, keeping it but made it private

  //properties after pagination

  //state management- use javascript object map

  private memberCache = new Map();

  private userParams!: UserParamsDto;
  private logedInUser!: LoggedInUserDto;

  constructor(private localStorageService: LocalStorageService, 
            private helperService: HelperService, 
            private httpClientService: HttpClientService, 
            private accountService: AccountService) { 

    //to pass the auth token to the api, later will use interceptor
    this.httpOptions = {
      headers: new HttpHeaders({
        Authorization: AppConstants.Bearer + this.localStorageService.getUserToken
      })
    };

    //set the logged in user and default params
    this.setLoggedInUserAndDefaultParams()
  }

  private setLoggedInUserAndDefaultParams(){
    //get the logged in use. no need to unsubscribe since using take(1)
    this.accountService.currentLoggedInUser$.pipe((take(1))).subscribe({
      next: user => {
        if(user){
          this.logedInUser = user;
          this.userParamsSet();
        }
      },
      error: e => { },
      complete: () => { }
    })
  }

  memberCacheClear(){
    if(!this.memberCache) return;
    this.memberCache.clear();
  }

  userParamsSet(){
    if(!this.logedInUser) return;
    this.userParams = new UserParamsDto(this.logedInUser);
  }
  
  userParamsUpdate(userParams: UserParamsDto) {
    this.userParams = userParams;
  }

  userParamsGet(): UserParamsDto{
    return this.userParams;
  }

  private getCacheKey(userParams: UserParamsDto) : string{
    const key = Object.values(userParams).join('-');
    return key;
  }

  /**
   * A GET users method
   * @returns Observable userDto[]
   */
  /*
  //not using this any more due to pagination
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
  */

  getMembers(userParams: UserParamsDto){
    //maake sure that the user is in cache
    const key = this.getCacheKey(userParams);
    this.helperService.logIfFrom(key, "getMembers cachekey");

    const members = this.memberCache.get(key);
    if(members)
      return of(members);

    //add the pagination and search params. Following method gives both pagination and user search params
    //return HttpParams
    const params = userParams.getDefaultMemberSearchParams();
    //url
    const url = this.helperService.urlUsersAll;
    //make the call
    return this.getPaginatedResult<UserDto[]>(url, params).pipe(
      map(response=> {
        //add the members to the cache
        this.memberCache.set(key, response)
        return response;
      })
    );
    
  }

  private getPaginatedResult<T>(url: string, params: HttpParams){
    this.helperService.logIfFrom(url, "MemberService getPaginatedResult Request");
    this.helperService.logIf(params);
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.httpClientService.getWithFullResponse<T>(url, params).pipe(
      map(response => {
        //result - members list
        if(response.body)
          paginatedResult.result = response.body;
        //pagination infor from the header
        const header = response.headers.get(AppConstants.PaginationHeader);
        if(header != null)
          paginatedResult.pagination = JSON.parse(header)
        this.helperService.logIfFrom(paginatedResult, "MemberService getPaginatedResult");
        return paginatedResult;
      })
    );
  }

  /**
   * A GET user by guid method
   * @returns Observable userDto
   */
  /*
  //using the new getMemberByGuid method
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
  */
  getMemberByGuid(guid: string): Observable<UserDto>{
    //first try getting the use from the cache
    const member = this.getMemberCache(guid, ZMemberGetBy.guid);
    if(member)
      return of(member);

    const url = this.helperService.replaceKeyValue(this.helperService.urlUserGetByGuid, this.helperService.keyGuid, guid, "");
    this.helperService.logIfFrom(url, "memberService getMemberByGuid");
    return this.httpClientService.get<UserDto>(url);
  }

  private getMemberFromArray(key: any, by: ZMemberGetBy): UserDto | undefined{
    let member: UserDto | undefined;
    if(!by || !key || this.members.length <= 0) return member;

    switch(by){
      case ZMemberGetBy.id:
        member = this.members.find(x => x.id === +key);
        break;
      case ZMemberGetBy.userName:
        member = this.members.find(x => x.userName === key);
        break;
      case ZMemberGetBy.guid:
        member = this.members.find(x => x.guId === key);
        break;
    }
    return member;
  }

  private getMemberCache(key: any, by: ZMemberGetBy): UserDto | undefined{
    let member: UserDto | undefined;
    if (!key || !by) return member;
    
    //using map and reduce to create a single array out of key value array pairs
    const members = [...this.memberCache.values()].reduce((arr, elem) => arr.concat(elem.result), []);
    this.helperService.logIfFrom(members, "MemberService getMemberCache")

    if (members.length <= 0) return member;

    switch (by) {
      case ZMemberGetBy.id: 
        member =  members.find((x: UserDto) => x.id === +key);
        break;
      case ZMemberGetBy.userName:
        member =  members.find((x: UserDto) => x.userName === key);
        break;
      case ZMemberGetBy.guid: 
        member =  members.find((x: UserDto) => x.guId === key);
        break; 
    }

    return member;
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

  /**
   * A add method to add likes
   * @param id id of the user getting liked 
   * 
   * @returns returns Created
   */
  addLike(id: number){
    var url = this.helperService.replaceKeyValue(this.helperService.urlLikeAdd, this.helperService.keyId, id);
    this.helperService.logIfFrom(url, "memberService addLike");
    
    return this.httpClientService.post(url, {});
  }

  /**
   * A add method to get likes
   * 
   * @returns returns UserDto[]
   */
  getLike(likeParams: LikeParamsDto){
    //add the pagination and filtering params
    const params = likeParams.getLikeSearchParams();
    const url = this.helperService.urlLikeGetUsers;
    this.helperService.logIfFrom(url, "memberService getLike")
    this.helperService.logIf(params);

    //make the call, the result is not full userDto, has some properties
    return this.getPaginatedResult<Partial<UserDto[]>>(url, params);
  }


}
