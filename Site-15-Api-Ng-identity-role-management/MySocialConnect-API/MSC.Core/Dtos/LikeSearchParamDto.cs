using MSC.Core.Dtos.Pagination;
using MSC.Core.Enums;

namespace MSC.Core.Dtos;

public class LikeSearchParamDto : PaginationParams
{
    public zUserLikeType UserLikeType {get; set;} 
    public int UserId {get; set;}
}
