
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;
using MSC.Core.Dtos.Pagination;
using MSC.Core.Enums;
using MSC.Core.Extensions;

namespace MSC.Core.Repositories;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        var like = await _context.Likes.FindAsync(sourceUserId, targetUserId);
        return like;
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        var user = await _context.Users
                                .Include(l => l.LikedUsers)
                                .FirstOrDefaultAsync(x => x.Id == userId);
        return user;
    }

    public async Task<IEnumerable<LikeDto>> GetUserLikes(int userId, string predicate)
    {
        var userLikeType = zUserLikeType.None;
        if(predicate == "liked")
            userLikeType = zUserLikeType.Liked;
        else if(predicate == "likedBy")
            userLikeType = zUserLikeType.LikedBy;

        var search = new LikeSearchParamDto(){ UserId = userId, UserLikeType = userLikeType };

        var likes = await GetUserLikesExecute(search).ToListAsync();

        return likes;
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikeSearchParamDto search)
    {
        var likes = GetUserLikesExecute(search);

        var users = await PagedList<LikeDto>.CreateAsync(likes, search.PageNumber, search.PageSize);
        return users;
    }

    private IQueryable<LikeDto> GetUserLikesExecute(LikeSearchParamDto search)
    {
        var usersQuery = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likesQuery = _context.Likes.AsQueryable();

        switch(search.UserLikeType)
        {   
            case zUserLikeType.Liked:
                //the logged in user has liked -- SourceUserId and TargetUser
                likesQuery = likesQuery.Where(l => l.SourceUserId == search.UserId);
                usersQuery = likesQuery.Select(l => l.TargetUser);
                break;
            case zUserLikeType.LikedBy:
                //others users that have liked the logged in user -- TargteUserId and SourceUser
                likesQuery = likesQuery.Where(l => l.TargetUserId == search.UserId);
                usersQuery = likesQuery.Select(l => l.SourceUser);
                break;
            default:
                throw new ValidationException($"Unable to GetUserLikes as UserLikeType '{search.UserLikeType.ToString()}' is not known");
                //break;
        }

        var likes = usersQuery.Select(u => new LikeDto{
            UserName = u.UserName,
            DisplayName = u.DisplayName,
            Age = u.DateOfBirth.CalculateAge(),
            PhotoUrl = u.Photos != null && u.Photos.Any(x => x.IsMain) ? u.Photos.FirstOrDefault(x => x.IsMain).Url : string.Empty,
            City = u.City,
            Id = u.Id,
            GuId = u.Guid
        });

        return likes;
    }
}
