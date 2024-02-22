using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MSC.Core.DB.Entities;

[Index(nameof(Guid))]
[Index(nameof(UserName))]
public class AppUser
{
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Guid { get; set; } 
    public string UserName { get; set; }
}
