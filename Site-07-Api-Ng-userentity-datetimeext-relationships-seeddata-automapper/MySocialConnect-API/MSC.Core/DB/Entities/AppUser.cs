using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MSC.Core.Extensions;

namespace MSC.Core.DB.Entities;

[Index(nameof(Guid))]
[Index(nameof(UserName))]
public class AppUser
{
    [Column(Order = 1)]
    public int Id { get; set; }

    [Column(Order = 2)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public Guid Guid { get; set; }  = Guid.NewGuid(); 

    [Column(Order = 3)]
    [Required]
    public string UserName { get; set; }

    [Column(Order = 4)]
    [Required]
    public byte[] PasswordHash {get; set;} //actual password

    [Column(Order = 5)]
    [Required]
    public byte[] PasswordSalt { get; set; } //the salt to hash the password

    [Column(Order = 6)]
    public DateOnly DateOfBirth { get; set; }

    [Column(Order = 7)]
    public string DisplayName { get; set; }

    [Column(Order = 8)]
    public string Gender { get; set; }

    [Column(Order = 9)]
    public string Introduction { get; set; }

    [Column(Order = 10)]
    public string LookingFor { get; set; }

    [Column(Order = 11)]
    public string Interests { get; set; }

    [Column(Order = 12)]
    public string City { get; set; }

    [Column(Order = 13)]
    public string Country { get; set; }

    public List<Photo> Photos { get; set; } = new();  //new List<Photo>();

    [Column(Order = 14)]
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    
    [Column(Order = 15)]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [Column(Order = 16)]
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

    /*
    public int GetAge(){
        var age = DateOfBirth.CalculateAge();
        return age;
    }
    */
}