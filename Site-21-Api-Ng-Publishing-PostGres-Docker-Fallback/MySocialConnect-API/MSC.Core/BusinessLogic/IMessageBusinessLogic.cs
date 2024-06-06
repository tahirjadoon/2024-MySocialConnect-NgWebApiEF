using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.BusinessLogic;

public interface IMessageBusinessLogic
{
    Task<BusinessResponse> AddMessageWithReadRecipt(MessageCreateDto msg, int senderId, bool markMsgAsRead);
    Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId);
    Task<BusinessResponse> DeleteMessage(int currentUserId, Guid msgGuid);
    void DeleteMessage(UserMessage message);
    Task<UserMessage> GetMessage(int id);
    Task<UserMessage> GetMessage(Guid guid);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageSearchParamDto search);
    Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId);
}
