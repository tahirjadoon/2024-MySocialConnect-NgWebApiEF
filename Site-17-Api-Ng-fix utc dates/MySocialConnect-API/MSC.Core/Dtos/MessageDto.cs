using System;

namespace MSC.Core.Dtos;

public class MessageDto
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public int SenderId { get; set; }
    public Guid SenderGuid { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public Guid RecipientGuid { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientPhotoUrl { get; set; }
    public string MessageContent { get; set; }
    public DateTime? DateMessageRead { get; set; }
    public DateTime DateMessageSent { get; set; }
}
