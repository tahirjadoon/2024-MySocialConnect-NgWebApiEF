using MSC.Core.Constants;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.Dtos;

public class UsersSearchParamDto : PaginationParams
{
    public string Gender { get; set; }
    public int MinAge { get; set; } = DataConstants.MinAge;
    public int MaxAge { get; set; } = DataConstants.MaxAge;
    public string OrderBy { get; set; } = DataConstants.LastActive;
}
