import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  //setup the keys for different items
  public _keyUser: string = "MySocialConnectUser";

  constructor() { } 

  getItemJson(key: string) {
    var item = this.getItem(key);
    return item ? JSON.parse(item) : null;
  }

  getItemJson2<T>(key: string) {
    var item = this.getItem(key);
    return item ? <T>JSON.parse(item) : null;
  }

  getItem(key: string){
    var item = sessionStorage.getItem(key);
    return item;
  }

  saveItemJson(key: string, obj: any) {
    this.saveItem(key, JSON.stringify(obj));
  }

  saveItem(key: string, value: string){
    sessionStorage.setItem(key, JSON.stringify(value));
  }

  removeItem(key: string) {
    const item = this.getItem(key);
    if(item) sessionStorage.removeItem(key);
  }

  
}
