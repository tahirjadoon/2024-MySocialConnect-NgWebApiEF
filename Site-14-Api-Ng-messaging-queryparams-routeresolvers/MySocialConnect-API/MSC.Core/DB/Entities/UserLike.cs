namespace  MSC.Core.DB.Entities;

//table name comes from datacontext as "Likes"
//key is combination of SourceUserId+argetUserId
public class UserLike
{
    //fully defining the relationship between AppUser and UserLike. CheckDB Context for relationships
    public AppUser SourceUser { get; set; }
    public int SourceUserId { get; set; }

    //fully defining the relationship between AppUser and UserLike. CheckDB Context for relationships
    public AppUser TargetUser { get; set; }
    public int TargetUserId { get; set; }
}
