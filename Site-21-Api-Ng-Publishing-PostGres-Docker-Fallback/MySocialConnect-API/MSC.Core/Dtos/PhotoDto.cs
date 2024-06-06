namespace MSC.Core.Dtos;

public class PhotoDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    //Added IsApproved with with PhotoManagement and removed PublicId since this is not being used on the client. 
    //public string PublicId { get; set; }
    public bool IsApproved { get; set; }
}
