import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SpinnerNotAllowed } from '../models-interfaces/spinner-not-allowed.model';

@Injectable({
  providedIn: 'root'
})
export class HelperService {
  private title: string = environment.title;
  private isProduction: boolean = environment.production;
  private isLogConsole: boolean = environment.displayConsoleLog;
  private baseUrlServer: string = environment.usebaseUrlHttps ? environment.webApiBaseUrlHttps : environment.webApiBaseUrlHttp;
  private baseUrlHub: string = environment.usebaseUrlHttps ? environment.webApiBaseHubsUrlHttps : environment.webApiBaseHubsUrlHttp;

  //private baseUrlApi: string = `${this.baseUrlServer}api`;
  private loadingSpinnerDelayMiliSec: number = environment.loadingSpinnerDelayMiliSec;

  constructor() { 
    if(this.isLogConsole){
      console.log(`HelperService isProduction: ${this.isProduction}`);
      console.log(`HelperService baseUrlServer: ${this.baseUrlServer}`);
      //console.log(`HelperService baseUrlApi: ${this.baseUrlApi}`);
    }
  }

  public Title: string = this.title;
  public IsProduction: boolean = this.isProduction;
  public IsLogConsole: boolean = this.isLogConsole;
  public LoadingSpinnerDelayMiliSec: number = this.loadingSpinnerDelayMiliSec;

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
  //public BaseUrlApi: string = this.baseUrlApi;

  //keys
  public keyId = "[id]";
  public keyName = "[name]";
  public keyGuid = "[guid]";
  public keyPhotoId = "[photoId]";

  //users
  private urlUsers: string = `${this.baseUrlServer}users`;
  public urlUsersAll: string = `${this.urlUsers}/`;
  public urlUserUpdate: string = `${this.urlUsers}/`;
  public urlUserGetById: string = `${this.urlUsers}/${this.keyId}/`;
  public urlUserGetByName: string = `${this.urlUsers}/${this.keyName}/name`;
  public urlUserGetByGuid: string = `${this.urlUsers}/${this.keyGuid}/guid`;
  public urlUserPhotoAdd: string = `${this.urlUsers}/add/photo`;
  public urlUserPhotoSetMain: string = `${this.urlUsers}/set/photo/${this.keyPhotoId}/main`;
  public urlUserPhotoDelete: string = `${this.urlUsers}/delete/${this.keyPhotoId}/photo`;

  //account
  private urlAccount: string = `${this.baseUrlServer}account`;
  public urlAccountRegister: string = `${this.urlAccount}/register`;
  public urlAccountLogin: string = `${this.urlAccount}/login`;
  public urlAccountCheckUser: string = `${this.urlAccount}/checkUser/${this.keyName}`;

  //likes
  private urlLikes: string = `${this.baseUrlServer}likes`;
  public urlLikeAdd: string = `${this.urlLikes}/${this.keyId}`;
  public urlLikeGetUsers: string = `${this.urlLikes}/`;

  //messages
  private urlMessages: string = `${this.baseUrlServer}message`;
  public urlMessageCreate: string = `${this.urlMessages}/create`;
  public urlMessageGetForUser: string = `${this.urlMessages}/user/get/messages`;
  public urlMessageGetThread: string = `${this.urlMessages}/thread/${this.keyId}`;  //recipientId
  public urlMessageDelete: string = `${this.urlMessages}/delete/${this.keyGuid}`; //message guid

  //admin 
  private urlAdmin: string = `${this.baseUrlServer}admin`;
  public urlAdminGetUsersWithRoles: string = `${this.urlAdmin}/users-with-roles`;
  public urlAdminGetPhotosToModerate: string = `${this.urlAdmin}/photos-to-moderate`;
  public urlAdminEditRoles: string = `${this.urlAdmin}/edit-roles/${this.keyGuid}`;
  public urlAdminApprovePhoto: string = `${this.urlAdmin}/photo-to-approve/${this.keyPhotoId}`;
  public urlAdminRejectPhoto: string = `${this.urlAdmin}/photo-to-reject/${this.keyPhotoId}`;


  //signalr end points
  public urlSignalrPresence: string = `${this.baseUrlHub}presence`;
  public urlSignalrMessage: string = `${this.baseUrlHub}message`;

  //paths for which the spinner is not needed
  private spinnerNotAllowedUrls: SpinnerNotAllowed[] = [
    new SpinnerNotAllowed(`${this.urlAccount}/checkUser/`, 'Get')
  ];
  public isSpinnerallowed(url: string, method: string): boolean{
    let isSpinnerAllowed: boolean = true;

    if(!this.spinnerNotAllowedUrls || this.spinnerNotAllowedUrls.length <= 0) return isSpinnerAllowed;
    if(!url) return isSpinnerAllowed;

    for(let item of this.spinnerNotAllowedUrls){
      if(url.toLowerCase().includes(item.url.toLowerCase()) && method.toLowerCase() === item.method.toLowerCase()){
        isSpinnerAllowed = false;
        break;
      }
    }

    return isSpinnerAllowed;
  }


  //sampe controller urls for testing errors
  private urlSample = `${this.baseUrlServer}sample`;
  public urlSampleAuth: string = `${this.urlSample}/auth`;
  public urlSampleUserNotFound: string = `${this.urlSample}/usernotfound`;
  public urlSampleServerError: string = `${this.urlSample}/servererror`;
  public urlSampleBadRequest: string = `${this.urlSample}/badrequest`;
  public urlSampleLogin: string = `${this.urlSample}/samplelogin`;
}
