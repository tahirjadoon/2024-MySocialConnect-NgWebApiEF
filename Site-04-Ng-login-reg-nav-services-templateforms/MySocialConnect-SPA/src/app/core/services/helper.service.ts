import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class HelperService {
  private title: string = environment.title;
  private isProduction: boolean = environment.production;
  private isLogConsole: boolean = environment.displayConsoleLog;
  private baseUrlServer: string = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;
  private baseUrlApi: string = `${this.baseUrlServer}api`;

  constructor() { 
    if(this.isLogConsole){
      console.log(`HelperService isProduction: ${this.isProduction}`);
      console.log(`HelperService baseUrlServer: ${this.baseUrlServer}`);
      console.log(`HelperService baseUrlApi: ${this.baseUrlApi}`);
    }
  }

  public Title: string = this.title;
  public IsProduction: boolean = this.isProduction;
  public IsLogConsole: boolean = this.isLogConsole;

  public logIf(text: any){
    if(this.isLogConsole)
      console.log(text);
  }

  public replaceKeyValue(text: string, key: string, value: any, printtext: string = ""): string{
    let newText = text.replace(key, value.toString());
    if(printtext){
      this.logIf(`${printtext}: ${newText}`);
    }
    return newText;
  }

  //public BaseUrlServer: string = this.baseUrlServer;
  //public BaseUrlApi: string = this.baseUrlApi;

  //keys
  public keyId = "[id]";
  public keyName = "[name]";
  public keyGuid = "[guid]";
  public keyPhotoId = "[photoId]";

  //users
  private urlUsers: string = `${this.baseUrlApi}/users`;
  public urlUsersAll: string = `${this.urlUsers}/`;
  public urlUserUpdate: string = `${this.urlUsers}/`;
  public urlUserGetById: string = `${this.urlUsers}/${this.keyId}/`;
  public urlUserGetByName: string = `${this.urlUsers}/${this.keyName}/name`;
  public urlUserGetByGuid: string = `${this.urlUsers}/${this.keyGuid}/guid`;
  public urlUserPhotoAdd: string = `${this.urlUsers}/add/photo`;
  public urlUserPhotoSetMain: string = `${this.urlUsers}/set/photo/${this.keyPhotoId}/main`;
  public urlUserPhotoDelete: string = `${this.urlUsers}/delete/${this.keyPhotoId}/photo`;

  //account
  private urlAccount: string = `${this.baseUrlApi}/account`;
  public urlAccountRegister: string = `${this.urlAccount}/register`;
  public urlAccountLogin: string = `${this.urlAccount}/login`;

  //buggy controller urls
  private urlBuggy = `${this.baseUrlApi}/buggy`;
  public urlBuggyAuth: string = `${this.urlBuggy}/auth`;
  public urlBuggyNotFound: string = `${this.urlBuggy}/not-found`;
  public urlBuggyServerError: string = `${this.urlBuggy}/server-error`;
  public urlBuggyBadRequest: string = `${this.urlBuggy}/bad-request`;
}
