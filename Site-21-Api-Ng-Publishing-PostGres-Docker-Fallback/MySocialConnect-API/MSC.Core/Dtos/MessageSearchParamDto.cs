using System;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Enums;

namespace MSC.Core.Dtos;

public class MessageSearchParamDto: PaginationParams
{
    public int UserId {get; set;}
    public zMessageType MessageType {get; set;} = zMessageType.InboxUnread;
}
