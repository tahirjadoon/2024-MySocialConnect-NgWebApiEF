import { Injectable } from '@angular/core';
import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  
  //setup the keys for different items
  public _keyUser: string = "MySocialConnectUser";

  constructor() { }

  getStorage(): any[]{
    let s: any[] = [];
    for(let i=0; i<localStorage.length; i++){
      const key: string = localStorage.key(i)!;
      if(key){
        const value: string = localStorage.getItem(key)!;
        s.push({key: key, value: value});
      }
    }
    return s;
  }

  getItemJson(key: string): any|null{
    const item = this.getItem(key);
    if(!item) return null;
    return JSON.parse(item);
  }

  getUser(): LoggedInUserDto{
    return this.getItemJson(this._keyUser)
  }

  getItem(key: string){
    return localStorage.getItem(key);
  }

  setItemJson(key: string, value: any){
    const item = JSON.stringify(value);
    this.setItem(key, item);
  }

  setUser(user: LoggedInUserDto){
    this.setItemJson(this._keyUser, user);
  }

  setItem(key: string, value: string){
    localStorage.setItem(key, value)
  }

  removeItem(key: string){
    const item = this.getItem(key);
    if(!item) return;
    localStorage.removeItem(key);
  }

  removeUser(){
    this.removeItem(this._keyUser)
  }

  //helper items to get some of the common pieces
  public getUserName: string = this.getUser()?.userName;
  public getUserToken: string = this.getUser()?.token;
  public getUserGuid: string = this.getUser()?.guid;
  public getUserGender: string = this.getUser()?.gender;
  public getUserPhotoUrl: string = this.getUser()?.mainPhotoUrl;
  public getUserDisplayName: string = this.getUser()?.displayName;
}
