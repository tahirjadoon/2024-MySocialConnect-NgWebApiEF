using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MSC.Core.DB.Entities;

[Index(nameof(Guid))]
[Index(nameof(UserName))]
public class AppUser
{
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Guid { get; set; }  = Guid.NewGuid(); 

    [Required]
    public string UserName { get; set; }

    [Required]
    public byte[] PasswordHash {get; set;} //actual password

    [Required]
    public byte[] PasswordSalt { get; set; } //the salt to hash the password
}
