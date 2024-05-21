import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';

import { HttpClientService } from './http-client.service';
import { HelperService } from './helper.service';
import { LocalStorageService } from './local-storage.service';

import { LoginDto } from '../models-interfaces/login-dto.model';
import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';
import { UserRegisterDto } from '../models-interfaces/user-register-dto.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  
  private serviceName: string = "AccountService";

  //behavioral subject observable for loggedInUser
  private loggedInUserSource = new BehaviorSubject<LoggedInUserDto | null>(null);
  currentLoggedInUser$ = this.loggedInUserSource.asObservable();

  //for all http requests use the HttpClientService rather than making the call locally
  //helper service has the urls etc, this will build the base url properly
  constructor(private httpClientService: HttpClientService, 
              private helperService: HelperService, 
              private localStorageService: LocalStorageService) { }

  checkUser(userName: string){
    const url = this.helperService.replaceKeyValue(this.helperService.urlAccountCheckUser, this.helperService.keyName, userName);
    return this.httpClientService.get<boolean>(url);
  }

  //fire the user as null so that any one subscribing to it can update
  logout(){
    //remove the user
    this.localStorageService.removeUser();
    this.fireCurrentUser(null);
  }

  //set the user in local storage and fire the user so that any one subscribing to it can action
  login(loginDto: LoginDto){
    this.localStorageService.removeUser();

    const url = this.helperService.urlAccountLogin;
    this.helperService.logIfFrom(url, `${this.serviceName}.Login Url`);

    //we are getting back LoggedInUserDto
    //persist the loggedinUser info in the local browser storage
    return this.httpClientService.post<LoggedInUserDto>(url, loginDto)
    .pipe(
      map((r: LoggedInUserDto) => {
        const user:LoggedInUserDto  = r;
        if(user)
          this.setAndFireCurrentUser(user);
        //return user;
      })
    );
  }

  register(user: UserRegisterDto){
    this.localStorageService.removeUser();

    const url = this.helperService.urlAccountRegister;
    this.helperService.logIfFrom(url, `${this.serviceName}.Register Url`); 

    //we are getting back LoggedInUserDto
    //persist the loggedinUser info in the local browser storage
    return this.httpClientService.post<LoggedInUserDto>(url, user)
    .pipe(
      map((r: LoggedInUserDto) => {
        const user:LoggedInUserDto  = r;
        if(user)
          this.setAndFireCurrentUser(user);
        //return user;
      })
    );

  }

  private fireCurrentUser(user: LoggedInUserDto | null){
    this.loggedInUserSource.next(user);
  }

  setAndFireCurrentUser(user: LoggedInUserDto){
    //pick roles from the token, for this need to decode the token. 
    //keep in mind users may only have single role as string or a string array of roles
    user.roles = [];
    const roles = this.getDecodedToken(user.token)?.role;
    if(roles){
      if(Array.isArray(roles))
        user.roles = roles;
      else 
        user.roles.push(roles);
    }
    
    this.localStorageService.setUser(user);
    this.fireCurrentUser(user);
  }

  getAndFireCurrentUser(){
    const user: LoggedInUserDto = this.localStorageService.getUser();
    if(!user) {
      this.fireCurrentUser(null);
      return;
    }
    this.fireCurrentUser(user);
  }

  getDecodedToken(token: string) :any {
    //token is not encypted, the signature is.
    //get the user roles from the token
    //The atob() method decodes a string that has been encoded by the btoa() method
    //token comes in three parts seperated by the . It is Header, Payload and signature
    //interested in the middle part
    var parsedToken = token.split(".")[1];
    var decoded = JSON.parse(atob(parsedToken));
    return decoded;
  }
}
