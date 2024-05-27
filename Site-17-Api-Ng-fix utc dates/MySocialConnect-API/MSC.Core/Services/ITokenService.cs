using System.Threading.Tasks;
using MSC.Core.DB.Entities;

namespace MSC.Core.Services;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}
