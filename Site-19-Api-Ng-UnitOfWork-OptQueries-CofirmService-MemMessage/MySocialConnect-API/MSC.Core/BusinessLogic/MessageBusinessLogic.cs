using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MSC.Core.DB.Entities;
using MSC.Core.DB.UnitOfWork;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.BusinessLogic;

public class MessageBusinessLogic : IMessageBusinessLogic
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MessageBusinessLogic(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<BusinessResponse> AddMessageWithReadRecipt(MessageCreateDto msg, int senderId, bool markMsgAsRead)
    {
        var result = await AddMessageHandle(msg, senderId, markMsgAsRead);
        return result;
    }

    public async Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId)
    {
        var result = await AddMessageHandle(msg, senderId, markMsgAsRead: false);
        return result;
    }

    private async Task<BusinessResponse> AddMessageHandle(MessageCreateDto msg, int senderId, bool markMsgAsRead = false)
    {
        if(msg == null || msg.RecipientId <= 0 || string.IsNullOrWhiteSpace(msg.MessageContent))
            return new BusinessResponse(HttpStatusCode.BadRequest, "Message not good");

        //get source user
        var sender = await _uow.UserRepo.GetUserAsync(senderId, includePhotos: true);
        if(sender == null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Logged in user not found");
        if(sender.Id == msg.RecipientId)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot send message to yourself");

        var recipient = await _uow.UserRepo.GetUserAsync(msg.RecipientId, includePhotos: true);
        if(recipient == null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Recipient not found");

        var message = new UserMessage{
            Sender = sender, 
            SenderUserName = sender.UserName,
            Recipient = recipient,  
            RecipientUserName = recipient.UserName,
            MessageContent = msg.MessageContent
        };

        if(markMsgAsRead){
            message.DateMessageRead = DateTime.UtcNow;
        }

        _uow.MessageRepo.AddMessage(message);
        if(await _uow.SaveChangesAsync())
        {
            var msgDto = _mapper.Map<MessageDto>(message);
            return new BusinessResponse(HttpStatusCode.OK, "", msgDto);
        }

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to send message");
    }

    public async Task<BusinessResponse> DeleteMessage(int currentUserId, Guid msgGuid)
    {
        var message = await _uow.MessageRepo.GetMessage(msgGuid);
        if(message == null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "No message found");
        if(message.Sender.Id != currentUserId && message.Recipient.Id != currentUserId)
            return new BusinessResponse(HttpStatusCode.Unauthorized);

        //due to EF only the sender will marked as deleted
        if(message.Sender.Id == currentUserId)
            message.SenderDeleted = true;

        //due to EF only the receipent will be marked as deleted
        if(message.Recipient.Id == currentUserId)
            message.RecipientDeleted = true;

        //when both have deleted it then delete it altogether
        if(message.SenderDeleted && message.RecipientDeleted)
            _uow.MessageRepo.DeleteMessage(message);

        //update
        if(await _uow.SaveChangesAsync())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to delete message");
    }

    public void DeleteMessage(UserMessage message)
    {
        throw new NotImplementedException();
    }

    public async Task<UserMessage> GetMessage(int id)
    {
        var message = await _uow.MessageRepo.GetMessage(id);
        return message;
    }

    public async Task<UserMessage> GetMessage(Guid guid)
    {
        var message = await _uow.MessageRepo.GetMessage(guid);
        return message;
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageSearchParamDto search)
    {
        var messages = await _uow.MessageRepo.GetMessagesForUser(search);
        return messages;
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId)
    {
        //now return MessageDto so no need to do mapping below
        var messages = await _uow.MessageRepo.GetMessageThread(currentUserId, receipientId);
        if(messages == null)
            return null;

        if(_uow.HasChanges())
            await _uow.SaveChangesAsync();

        //var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);
        //return messagesDto;
        return messages;
    }
}
