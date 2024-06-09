import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';

import { HelperService } from '../helper.service';

import { LoggedInUserDto } from '../../models-interfaces/logged-in-user-dto.model';
import { MessageDto } from '../../models-interfaces/message-dto.model';
import { SignalRGroup } from '../../models-interfaces/signalr/signalr-group.model';
import { SpinnerBusyService } from '../spinner-busy.service';


@Injectable({
  providedIn: 'root'
})
export class MessageHubService {

  private hubConnecton!: HubConnection;

  //events from messageHub api
  private _keyReceiveMessageThread = "ReceiveMessageThread";
  private _keyNewMessage = "NewMessage";
  private _keyUpdatedGroup = "UpdatedGroup";

  //create message end point inside MessageHub api
  private _endPointCreateMessage = "CreateMessage";

  private messageThreadSource = new BehaviorSubject<MessageDto[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private helperService: HelperService, private busyService: SpinnerBusyService) { }

  //create Hub Connection
  createHubConnection(user: LoggedInUserDto, otherUserName: string, otherUserId: number){
    const otherParams = `?otherUserName=${otherUserName}&otherUserId=${otherUserId}`;
    const url = `${this.helperService.urlSignalrMessage}${otherParams}`;
    this.helperService.logIfFrom(url, "MessageHubService Conn url");

    //show spinner
    this.busyService.busy();

    this.hubConnecton = new HubConnectionBuilder()
                            .withUrl(url, {
                              accessTokenFactory: () => user.token
                            })
                            .withAutomaticReconnect()
                            .build();

    //start the connection
    //hide the spinner on finally
    this.hubConnecton.start()
                    .catch(e => {
                      this.helperService.logIfFrom(e, "MessageHubService start Error");
                    })
                    .finally(() => this.busyService.idle())
                    ;

    //listen for event ReceiveMessageThread
    this.hubConnecton.on(this._keyReceiveMessageThread, (messages: MessageDto[]) => {
      this.fireMessageThread(messages, this._keyReceiveMessageThread);
    });

    //on new message add, 
    //when the message is added via createMessage below the hub returns a NewMessage event
    this.hubConnecton.on(this._keyNewMessage, (message: MessageDto) => {
      //create a new array of messages and replace it. Add the new message to it also
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages: MessageDto[]) => {
          if(message){
            //without mutating the array add on the new message. this is a new array
            const newMessages = [...messages, message];
            this.fireMessageThread(newMessages, this._keyNewMessage);
          }
        }
      });
    });

    //tracking groups
    this.hubConnecton.on(this._keyUpdatedGroup, (group: SignalRGroup) => {
      //if the user joins our group then the user wil not receive above ReceiveMessageThread
      //mark the messages as read in this case
      if(group.connections.some(x => x.userName == otherUserName)){
        this.messageThread$.pipe(take(1)).subscribe({
          next: (messages: MessageDto[]) => {
            if(!messages) return;

            messages.forEach(message => {
              if(!message.dateMessageRead)
                message.dateMessageRead = new Date(Date.now());
            });
            this.fireMessageThread([...messages], this._keyUpdatedGroup);
          }
        });
      }
    });

  }

  //stop hub connection
  stopHubConnection(){
    try{
      if(this.hubConnecton){
        //when the user moves away then remove the messages
        this.messageThreadSource.next([]);
        this.hubConnecton.stop()
                      .catch(e => {
                        this.helperService.logIfFrom(e, "PresenceHubService stop Error");
                      });
      }
    }
    catch(e){
      this.helperService.logIfFrom(e, "MessageHubService stop Exception");
    }
  }

  private fireMessageThread(messages: MessageDto[], from: string){
    this.helperService.logIfFrom("From " + from, "MessageHub fireMessageThread");
    this.messageThreadSource.next(messages);
  }

  //the return is a promise. To force it make it an async method
  async createMessage(recipientId: number, messageContent: string)
  {
    //since the property names are the same we can do short cut as in second line as well
    //const data = {recipientId: recipientId, messageContent: messageContent};
    const data = { recipientId, messageContent};

    return this.hubConnecton
              .invoke(this._endPointCreateMessage, data)
              .catch(e => this.helperService.logIfFrom(e, "MessageHubService createMessage Exception"))
  }
}
