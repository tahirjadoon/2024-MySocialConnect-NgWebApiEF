import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

import { HelperService } from '../helper.service';

import { LoggedInUserDto } from '../../models-interfaces/logged-in-user-dto.model';
import { UserDto } from '../../models-interfaces/user-dto.model';

@Injectable({
  providedIn: 'root'
})
export class PresenceHubService {

  private hubConnecton!: HubConnection;

  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  //events from presence hub web api
  private _keyUserIsOnline = 'UserIsOnline';
  private _keyUserIsOffline = 'UserIsOffline';
  private _keyUsersOnline = 'GetOnlineUsers';
  //this is fired by the messageHub using presenceHub on CreateMessage 
  private _keyNewMessageReceived = 'NewMessageReceived';

  constructor(private helperService: HelperService, 
              private toastr: ToastrService, 
              private router: Router) { }

  //create Hub Connection
  createHubConnection(user: LoggedInUserDto){
    
    const url = this.helperService.urlSignalrPresence;
    this.helperService.logIfFrom(url, "PresenceHubService Conn url")

    //build
    this.hubConnecton = new HubConnectionBuilder()
                        .withUrl(url, {
                          accessTokenFactory: () => user.token
                        })
                        .withAutomaticReconnect()
                        .build();
    
    //start the connection
    this.hubConnecton.start()
                    .catch(e => {
                      this.helperService.logIfFrom(e, "PresenceHubService start Error");
                    });

    //listen for event UserIsOnline, returns userName
    this.hubConnecton.on(this._keyUserIsOnline, userName => {
      this.toastr.info(`${userName} has connected!`);
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (userNames) => {
          const newUserNames = [...userNames, userName];
          this.fireOnlineUsers(newUserNames);
        }
      });
    });

    //listen for event UserIsOffline, returns userName
    this.hubConnecton.on(this._keyUserIsOffline, userName => {
      this.toastr.warning(`${userName} has disconnected!`);
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (userNames) => {
          //filter method cretaed a new array so no need to use the spread operator
          const newUserNames = userNames.filter(x => x !== userName);
          this.fireOnlineUsers(newUserNames);
        }
      });

    });

    //listen for event GetOnlineUsers, returns string[]
    this.hubConnecton.on(this._keyUsersOnline, (userNames: string[]) => {
      this.fireOnlineUsers(userNames);
    });

    //fired from messageHub when new message is sent and the user is not on message page, same group as sender
    //this is using the core/strategy/CustomRouteReuseStrategy
    this.hubConnecton.on(this._keyNewMessageReceived, (sender: UserDto) => {

      this.toastr.info(`${sender.displayName} has sent you a new message! Click me to see it`)
      .onTap
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.router.navigateByUrl(`members/detail/${sender.guId}/${sender.displayName}?tab=messages`);
        }
      });
    });
    
  }

  //stop hub connection
  stopHubConnection(){
    try{
      if(this.hubConnecton){
        this.hubConnecton.stop()
                      .catch(e => {
                        this.helperService.logIfFrom(e, "PresenceHubService stop Error");
                      });
      }
    }
    catch(e){
      this.helperService.logIfFrom(e, "PresenceHubService stop Exception");
    }
  }

  private fireOnlineUsers(userNames: string[]){
    this.onlineUsersSource.next(userNames);
  }
}
