using System.ComponentModel.DataAnnotations.Schema;

namespace MSC.Core.DB.Entities;

//Database table will be called Photos
//with photo managment created a dbset for photos
[Table("Photos")]
public class Photo
{
    [Column(Order = 1)]
    public int Id { get; set; }

    [Column(Order = 2)]
    public string Url { get; set; }

    [Column(Order = 3)]
    public bool IsMain { get; set; }

    [Column(Order = 4)]
    public bool IsApproved {get; set;}

    [Column(Order = 5)]
    public string PublicId { get; set; }

    //fully defining the relationship between AppUser and Photos
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many
    //alternatively can also do OnModelCreating in Data context
    //[Column(Order = 6)]
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
