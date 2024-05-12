import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

import { MessageService } from '../../../core/services/message.service';

import { MessageDto } from '../../../core/models-interfaces/message-dto.model';

import { UserDto } from '../../../core/models-interfaces/user-dto.model';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
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
  @Input() messagesIn: MessageDto[] = [];

  messageContent: string = '';

  messageSubscription!: Subscription;

  constructor(private messageService: MessageService, private toastr: ToastrService){}

  ngOnDestroy(): void {
    if(this.messageSubscription) this.messageSubscription.unsubscribe();
  }

  ngOnInit(): void {
  }

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
        this.messagesIn.push(message);
        this.msgForm.reset();
        //this.messageContent = "";
      },
      error: e => {},
      complete: () => {}
    })

  }

}
