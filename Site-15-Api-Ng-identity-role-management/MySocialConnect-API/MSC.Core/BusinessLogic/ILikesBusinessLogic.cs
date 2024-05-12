using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.BusinessLogic;

public interface ILikesBusinessLogic
{
    //get the user like
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);

    Task<AppUser> GetUserWithLikes(int userId);

    //Get UsersLiked and LikedBy
    Task<IEnumerable<LikeDto>> GetUserLikes(int userId, string predicate);

    //Get UsersLiked and LikedBy
    Task<PagedList<LikeDto>> GetUserLikes(LikeSearchParamDto search);
    
    //Add Like
    Task<BusinessResponse> AddLike(int userId, UserClaimGetDto claims);
}
