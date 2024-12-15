using System;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Guid GetAccount_Id(string token);
    }
}
