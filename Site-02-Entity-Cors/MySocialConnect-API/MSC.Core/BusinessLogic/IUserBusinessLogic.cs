﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Core.DB.Entities;

namespace MSC.Core.BusinessLogic;

public interface IUserBusinessLogic
{
    Task<IEnumerable<AppUser>> GetUsersAsync();

    Task<AppUser> GetUserAsync(int id);

    Task<AppUser> GetUserAsync(string userName);

    Task<AppUser> GetUserAsync(Guid guid);
}
