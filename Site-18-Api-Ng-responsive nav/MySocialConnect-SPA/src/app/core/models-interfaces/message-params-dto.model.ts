import { HttpParams } from "@angular/common/http";
import { ZMessageType } from "../enums/z-message-type";
import { PageParams } from "./pagination/page-params.model";

export class MessageParamsDto extends PageParams {
    messageType: ZMessageType = ZMessageType.inboxUnread;

    constructor(){
        super();
    }

    //helper metod to build search params
    getSearchParams() : HttpParams {
        let params = super.getPaginationSearchParams();
        params = params.append('messageType', ZMessageType[this.messageType]);
        return params;
    }
}
