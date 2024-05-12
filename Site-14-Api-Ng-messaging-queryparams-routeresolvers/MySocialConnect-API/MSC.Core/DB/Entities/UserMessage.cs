using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Entities;

namespace MSC.Core.DB.Entities;

//table name comes from datacontext as "Messages"
//primary key will be cretaed as part of the convention
//check db context for more details
[Index(nameof(Guid))]
public class UserMessage
{
    [Column(Order = 1)]
    public int Id { get; set; }

    [Column(Order = 2)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Guid { get; set; } = Guid.NewGuid(); 

    [Column(Order = 3)]
    [Required]
    public int SenderId { get; set; }

    [Column(Order = 4)]
    [Required]
    public string SenderUserName { get; set; }

    public AppUser Sender { get; set; }

    [Column(Order = 5)]
    public bool SenderDeleted { get; set; } = false;

    [Column(Order = 6)]
    [Required]
    public int RecipientId { get; set; }

    [Column(Order = 7)]
    [Required]
    public string RecipientUserName { get; set; }

    public AppUser Recipient { get; set; }

    [Column(Order = 8)]
    public bool RecipientDeleted { get; set; } = false;

    [Column(Order = 9)]
    public string MessageContent { get; set; }

     [Column(Order = 10)]
    public DateTime DateMessageSent { get; set; } = DateTime.UtcNow;

    [Column(Order = 11)]
    public DateTime? DateMessageRead { get; set; }
}
