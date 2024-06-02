﻿using System.Threading.Tasks;
using MSC.Core.DB.Entities.SignalR;

namespace MSC.Core.Repositories;

public interface ISignalRRepository
{
    //Task<bool> SaveAllSync();

    void AddGroup(SignalRGroup group);
    void RemoveConnection(SignalRConnection connection);
    Task<SignalRGroup> GetMessageGroup(string groupName);
    Task<SignalRGroup> GetGroupByConnection(string connectionId);
    Task<SignalRConnection> GetConnection(string connectionId);
}
