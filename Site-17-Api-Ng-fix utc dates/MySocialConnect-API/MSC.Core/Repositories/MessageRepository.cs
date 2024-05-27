using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Enums;

namespace MSC.Core.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(UserMessage message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(UserMessage message)
    {
       _context.Messages.Remove(message);
    }

    public async Task<UserMessage> GetMessage(int id)
    {
        //we can do projectto to fill the entities or like following
        var message = await _context.Messages
                            .Include(u => u.Recipient)
                            .Include(u => u.Sender)
                            .SingleOrDefaultAsync(x => x.Id == id);
        return message;
    }

    public async Task<UserMessage> GetMessage(Guid guid)
    {
        //we can do projectto to fill the entities or like following
        var message = await _context.Messages
                            .Include(u => u.Recipient)
                            .Include(u => u.Sender)
                            .SingleOrDefaultAsync(x => x.Guid == guid);
        return message;
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageSearchParamDto search)
    {
        var query = _context.Messages.OrderByDescending(m => m.DateMessageSent).AsQueryable();
        query = search.MessageType switch
        {
            //recipient of the message
            zMessageType.Inbox => query.Where(u => u.RecipientId == search.UserId && !u.RecipientDeleted),
            //recipient of the message and not read it
            zMessageType.InboxUnread => query.Where(u => u.RecipientId == search.UserId && u.DateMessageRead == null && !u.RecipientDeleted),
            //default sender outbox
            _ => query.Where(u => u.Sender.Id == search.UserId && !u.SenderDeleted) 
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        var pageList = await PagedList<MessageDto>.CreateAsync(messages, search.PageNumber, search.PageSize);

        return pageList;
    }

    //message thread between two users so check for both ways. Also eagily load photos for both receipent and sender
    public async Task<IEnumerable<UserMessage>> GetMessageThread(int currentUserId, int receipientId)
    {
        var messages = await _context.Messages
                            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                            .Include(u => u.Sender).ThenInclude(p => p.Photos)
                            .Where(m => 
                                (m.RecipientId == currentUserId && m.SenderId == receipientId && !m.RecipientDeleted) || 
                                (m.RecipientId == receipientId && m.SenderId == currentUserId && !m.SenderDeleted)
                            )
                            .OrderBy(m => m.DateMessageSent)
                            .ToListAsync();
        var unreadMessages = messages.Where(m => m.DateMessageRead == null && m.Recipient.Id == currentUserId).ToList();

        if(unreadMessages != null && unreadMessages.Any())
        {
            //update the date
            unreadMessages.ForEach(x => {x.DateMessageRead = DateTime.UtcNow;});
            await _context.SaveChangesAsync();
        }
        return messages;
    }

    public async Task<bool> SaveAllSync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
