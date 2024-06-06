import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HelperService } from './helper.service';
import { HttpClientService } from './http-client.service';
import { PageService } from './page.service';

import { MessageParamsDto } from '../models-interfaces/message-params-dto.model';
import { MessageDto } from '../models-interfaces/message-dto.model';



@Injectable({
  providedIn: 'root'
})
export class MessageService {

  messageSearchParams: MessageParamsDto = new MessageParamsDto();

  constructor(private httpClientService: HttpClientService,  
              private helperService: HelperService, 
              private pageService: PageService) { }

  getMessages(search: MessageParamsDto){
    this.messageSearchParams = search;
    const params = search.getSearchParams();
    const url = this.helperService.urlMessageGetForUser

    return this.pageService.getPaginatedResult<MessageDto[]>(url, params);
  }

  getMessageThread(recipientId: number) : Observable<MessageDto[]>{
    const url = this.helperService.replaceKeyValue(this.helperService.urlMessageGetThread, this.helperService.keyId, recipientId);
    this.helperService.logIfFrom(url, "MessageService getMessageThread");
    return this.httpClientService.get<MessageDto[]>(url);
  }

  createMessage(recipientId: number, messageContent: string): Observable<MessageDto>{
    const url = this.helperService.urlMessageCreate;
    //since the property names are the same we can do short cut as in second line as well
    //const data = {recipientId: recipientId, messageContent: messageContent};
    const data = { recipientId, messageContent};
    this.helperService.logIfFrom(url, "MessageService createMessage");
    
    return this.httpClientService.post<MessageDto>(url, data);
  }

  deleteMessage(guid: string){
    const url = this.helperService.replaceKeyValue(this.helperService.urlMessageDelete, this.helperService.keyGuid, guid);
    this.helperService.logIfFrom(url, "MessageService deleteMessage");

    return this.httpClientService.delete(url);
  }
  
}
