using MSC.Core.DB.Entities;

namespace MSC.Core.Services;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
