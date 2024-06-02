using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.Repositories;

public interface IMessageRepository
{
     void AddMessage(UserMessage message);
    void DeleteMessage(UserMessage message);
    Task<UserMessage> GetMessage(int id);
    Task<UserMessage> GetMessage(Guid guid);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageSearchParamDto search);

    /*Method is now returning MessageDto*/
    //Task<IEnumerable<UserMessage>> GetMessageThread(int currentUserId, int receipientId);
    Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId);
    
    //Task<bool> SaveAllSync();
}