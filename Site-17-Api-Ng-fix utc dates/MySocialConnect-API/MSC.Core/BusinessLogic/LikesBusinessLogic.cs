using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Repositories;

namespace MSC.Core.BusinessLogic;

public class LikesBusinessLogic : ILikesBusinessLogic
{
    private readonly ILikesRepository _likeRepo;
    private readonly IUserRepository _userRepo;

    public LikesBusinessLogic(ILikesRepository likeRepo, IUserRepository userRepo)
    {
        _likeRepo = likeRepo;
        _userRepo = userRepo;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        var like = await _likeRepo.GetUserLike(sourceUserId, targetUserId);
        return like;
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        var like = await _likeRepo.GetUserWithLikes(userId);
        return like;
    }

    //Get UsersLiked and LikedBy
    public async Task<IEnumerable<LikeDto>> GetUserLikes(int userId, string predicate)
    {
        var users = await _likeRepo.GetUserLikes(userId, predicate);
        return users;
    }

    //Get UsersLiked and LikedBy
    public async Task<PagedList<LikeDto>> GetUserLikes(LikeSearchParamDto search)
    {
        var users = await _likeRepo.GetUserLikes(search);
        return users;
    }

    //Add Like
    public async Task<BusinessResponse> AddLike(int userId, UserClaimGetDto claims)
    {
        //get source user
        var sourceUser = await _likeRepo.GetUserWithLikes(claims.Id);
        if(sourceUser == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Logged in user not found");

        //get likedUser
        var likedUser = await _userRepo.GetUserAsync(userId, includePhotos: false);        
        if(likedUser == null)
           return new BusinessResponse(HttpStatusCode.NotFound, "Liked user not found");         

        if(likedUser.UserName == sourceUser.UserName)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot like your self");

        var userLike = await _likeRepo.GetUserLike(sourceUser.Id, likedUser.Id);
        if(userLike != null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You already liked this user");

        //save - add to the source user
        userLike = new UserLike(){ SourceUserId = sourceUser.Id, TargetUserId = likedUser.Id };
        if(sourceUser.LikedUsers == null) sourceUser.LikedUsers = new List<UserLike>();
        sourceUser.LikedUsers.Add(userLike);
        if(await _userRepo.SaveAllAsync())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to add like");
    }
}
