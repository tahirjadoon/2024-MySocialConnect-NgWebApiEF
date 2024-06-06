using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Core.DB.Data;
using MSC.Core.DB.Entities;
using MSC.Core.Dtos;

namespace MSC.Core.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public PhotoRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync()
    {
        /*var photos = await _context.Photos
                            .IgnoreQueryFilters()
                            .Where(p => p.IsApproved == false)
                            .Select(u => new PhotoForApprovalDto{
                                Id = u.Id,
                                UserName = u.AppUser.UserName,
                                UserGuid = u.AppUser.Guid,
                                UserId = u.AppUser.Id, 
                                Url = u.Url,
                                IsApproved = u.IsApproved
                            }).ToListAsync();*/

        var photos = await _context.Photos
                            .IgnoreQueryFilters()
                            .Where(p => p.IsApproved == false)
                            .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
                            .ToListAsync();

        return photos;
    }

    public async Task<Photo> GetPhotoByIdAsync(int id)
    {
        var photo = await _context.Photos
                            .IgnoreQueryFilters()
                            .SingleOrDefaultAsync(x => x.Id == id);
        return photo;
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
