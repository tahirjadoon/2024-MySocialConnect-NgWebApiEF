import { Component, OnDestroy, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { ZMessageType } from 'src/app/core/enums/z-message-type';
import { MessageDto } from 'src/app/core/models-interfaces/message-dto.model';
import { MessageParamsDto } from 'src/app/core/models-interfaces/message-params-dto.model';
import { PaginatedResult } from 'src/app/core/models-interfaces/pagination/paginated-result.model';
import { Pagination } from 'src/app/core/models-interfaces/pagination/pagination.model';
import { HelperService } from 'src/app/core/services/helper.service';
import { MessageService } from 'src/app/core/services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit, OnDestroy {

  isGettingMessage: boolean = false;
  messages!: MessageDto[];
  pagination!: Pagination;

  msgParams: MessageParamsDto; //this has the default for messageType which is inboxUnread
  msgTypeString: string;

  //for use in the componenet
  zMessageType = ZMessageType;

  messageSubscription!: Subscription;
  deleteSubscription!: Subscription;

  fromTo:string = "";
  sentReceived: string = "";

  constructor(private messageService: MessageService, 
            private helperService: HelperService, 
            private toastr: ToastrService){

    this.msgParams = this.messageService.messageSearchParams;
    this.msgTypeString = ZMessageType[this.msgParams.messageType];
  }

  ngOnDestroy(): void {
    if(this.messageSubscription) this.messageSubscription.unsubscribe();
    if(this.deleteSubscription) this.deleteSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.setFromToSentReceived();
    this.loadMessages();
  }

  loadMessages(){
    this.pagination = <Pagination>{};
    this.isGettingMessage = true;
    this.messages = [];
    const msgTypeString = ZMessageType[this.msgParams.messageType];

    //if the user has asked for a different type then reset the number
    if(this.msgTypeString !== msgTypeString){
      this.msgTypeString = msgTypeString;
      this.msgParams.pageNumber = 1;
    }

    this.setFromToSentReceived();

    this.messageSubscription = this.messageService.getMessages(this.msgParams).subscribe({
      next: (response: PaginatedResult<MessageDto[]>) => {
        if(response && response.result && response.pagination){
          this.messages = response.result;
          this.pagination = response.pagination;
        }
      },
      error: e => {},
      complete: () => this.isGettingMessage = false
    })
  }

  onPageChange(event: any){
    if(this.msgParams && this.msgParams.pageNumber != event.page){
      this.msgParams.pageNumber = event.page;
      this.loadMessages;
    }
  }

  onDeleteMessage(guid: string){
    this.deleteSubscription = this.messageService.deleteMessage(guid).subscribe({
      next: () => {
        //remove the message from the messages by passing the index and total number of messages to delete which is one in this case
        if(this.messages && this.messages.length > 0){
          this.messages.splice(this.messages.findIndex(x => x.guid === guid), 1);
          this.toastr.success("Message deleted", "Delete");
        }
      },
      error: e => {},
      complete: () => {}
    })
  }

  private setFromToSentReceived(){
    this.fromTo = "From";
    this.sentReceived = "Received";
    if(this.isOutBox()){
      this.fromTo = "To";
      this.sentReceived = "Sent";
    }

  }

  private isOutBox(): boolean{
    return this.msgParams.messageType == ZMessageType.outbox;
  }

  private getMemberPiece(piece: string, message: MessageDto): string{
    let userName = message.senderUsername;
    let photoUrl = message.senderPhotoUrl;
    let guid = message.senderGuid;
    const detailLink = "/members/detail/";
    let url = `${detailLink}/${message.senderGuid}/${message.senderUsername}`;
    if(this.isOutBox()){
      userName = message.recipientUsername;
      photoUrl = message.recipientPhotoUrl;
      guid = message.recipientGuid;
      url = `${detailLink}/${message.recipientGuid}/${message.recipientUsername}`;
    }

    let result = "";
    switch(piece){
      case "username":
        result = userName;
        break;
      case "photourl":
        result = photoUrl;
        break;
      case "guid":
        result = guid;
        break;
      case "detaillink":
        result = url;
        break;
      default:
        this.toastr.error("Unaable to get piece", "Error");
        break;
    }
    
    return result;
  }

  getMemberUserName(messsage: MessageDto){
    return this.getMemberPiece("username", messsage);
  }

  getMemberPhotoUrl(message: MessageDto){
    return this.getMemberPiece("photourl", message);
  }

  getMemberGuid(message: MessageDto){
    return this.getMemberPiece("guid", message);
  }

  getMemberDetailLink(message: MessageDto){
    return this.getMemberPiece("detaillink", message);
  }

}
