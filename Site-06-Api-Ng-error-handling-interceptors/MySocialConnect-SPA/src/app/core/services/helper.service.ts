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
    if(!this.isLogConsole) return;
    console.log(text);
  }

  public logIfFrom(msg: any, from: string){
    if(!this.isLogConsole) return;
    if(from) this.logIf(`***${from}***`)
    if(msg) this.logIf(msg);
  }

  public logIfError(error: any, from: string){
    if(!this.isLogConsole) return;
    this.logIf("***displayError***")
    if(from) this.logIf(from);
    if(error) this.logIf(error);
  }

  public replaceKeyValue(text: string, key: string, value: any, printtext: string = ""): string{
    let newText = text.replace(key, value.toString());
    if(printtext){
      this.logIf(`${printtext}: ${newText}`);
    }
    return newText;
  }

  public BaseUrlServer: string = this.baseUrlServer;
  public BaseUrlApi: string = this.baseUrlApi;

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
  public urlAccountCheckUser: string = `${this.urlAccount}/checkUser/${this.keyName}`;

  //sampe controller urls for testing errors
  private urlSample = `${this.baseUrlApi}/sample`;
  public urlSampleAuth: string = `${this.urlSample}/auth`;
  public urlSampleUserNotFound: string = `${this.urlSample}/usernotfound`;
  public urlSampleServerError: string = `${this.urlSample}/servererror`;
  public urlSampleBadRequest: string = `${this.urlSample}/badrequest`;
  public urlSampleLogin: string = `${this.urlSample}/samplelogin`;
}
