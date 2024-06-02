import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

import { MessageService } from '../../../core/services/message.service';
import { MessageHubService } from '../../../core/services/signalr/message-hub.service';

import { MessageDto } from '../../../core/models-interfaces/message-dto.model';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush, //due to scrolling the messages
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit, OnDestroy {

  //messge template driven form
  @ViewChild('messageForm') msgForm!: NgForm;

  @Input() memberIn!:UserDto;
  //not passing the messages any more after message hub iplementation
  @Input() messagesIn: MessageDto[] = [];

  messageContent: string = '';

  messageSubscription!: Subscription;
  messageHubSubscription!: Subscription;

  constructor(private messageService: MessageService, 
              private toastr: ToastrService, 
              private messageHubService: MessageHubService){}

  ngOnDestroy(): void {
    if(this.messageSubscription) this.messageSubscription.unsubscribe();
    if(this.messageHubSubscription) this.messageHubSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.loadMessagesFromMessageHub();
  }

  loadMessagesFromMessageHub(){
    this.messageHubSubscription = this.messageHubService.messageThread$.subscribe({
      next: (messages: MessageDto[]) => {
        //just use the input messages here
        this.messagesIn = messages;
      }
    })
  }

  //not using this any more
  //using the message hub create message methos
  onSubmitMessage(){
    if(!this.memberIn || !this.memberIn.id){
      this.toastr.error('User error', 'Error');
      return;
    }

    this.messageSubscription = this.messageService.createMessage(this.memberIn.id, this.messageContent).subscribe({
      next: (message: MessageDto) => {
        if(!message){
          this.toastr.error("Unable to get back the created message. Refresh page...", "Error");
          return;
        }
        //not using it after the message hub implementation
        this.messagesIn.push(message);
        this.msgForm.reset();
        //this.messageContent = "";
      },
      error: e => {},
      complete: () => {}
    })
  }

  OnSubmitMessageUsingHub(){
    if(!this.memberIn || !this.memberIn.id){
      this.toastr.error('User error', 'Error');
      return;
    }
    //this is returning a promise
    //The newly added message will show via loadMessagesFromMessageHub 
    this.messageHubService.createMessage(this.memberIn.id, this.messageContent)
                          .then(() => {
                            this.msgForm.reset();
                          });

  }


}
