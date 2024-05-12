using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Repositories;

namespace MSC.Core.BusinessLogic;

public class MessageBusinessLogic : IMessageBusinessLogic
{
    private readonly IMessageRepository _msgRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public MessageBusinessLogic(IMessageRepository msgRepo, IUserRepository userRepo, IMapper mapper)
    {
        _msgRepo = msgRepo;
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId)
    {
        if(msg == null || msg.RecipientId <= 0 || string.IsNullOrWhiteSpace(msg.MessageContent))
            return new BusinessResponse(HttpStatusCode.BadRequest, "Message not good");

        //get source user
        var sender = await _userRepo.GetUserAsync(senderId, includePhotos: true);
        if(sender == null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Logged in user not found");
        if(sender.Id == msg.RecipientId)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot send message to yourself");

        var recipient = await _userRepo.GetUserAsync(msg.RecipientId, includePhotos: true);
        if(recipient == null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Recipient not found");

        var message = new UserMessage{
            Sender = sender, 
            SenderUserName = sender.UserName,
            Recipient = recipient,  
            RecipientUserName = recipient.UserName,
            MessageContent = msg.MessageContent
        };

        _msgRepo.AddMessage(message);
        if(await _msgRepo.SaveAllSync())
        {
            var msgDto = _mapper.Map<MessageDto>(message);
            return new BusinessResponse(HttpStatusCode.OK, "", msgDto);
        }

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to send message");    
    }

    public async Task<BusinessResponse> DeleteMessage(int currentUserId, Guid msgGuid)
    {
        var message = await _msgRepo.GetMessage(msgGuid);
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
            _msgRepo.DeleteMessage(message);

        //update
        if(await _msgRepo.SaveAllSync())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to delete message");
    }

    public void DeleteMessage(UserMessage message)
    {
        throw new NotImplementedException();
    }

    public async Task<UserMessage> GetMessage(int id)
    {
        var message = await _msgRepo.GetMessage(id);
        return message;
    }

    public async Task<UserMessage> GetMessage(Guid guid)
    {
        var message = await _msgRepo.GetMessage(guid);
        return message;
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageSearchParamDto search)
    {
        var messages = await _msgRepo.GetMessagesForUser(search);
        return messages;
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId)
    {
        var messages = await _msgRepo.GetMessageThread(currentUserId, receipientId);
        if(messages == null)
            return null;

        var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);
        return messagesDto;
    }
}
