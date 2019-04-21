using Shared.ViewModel.Users;
using System.Collections.Generic;

namespace Business.Users
{
    public interface IUsersService
    {
        List<UserModel> Get();
        UserModel Get(int id);
        int Post(UserModel value);
        int Put(int id, UserModel value);
        int Delete(int id);
    }
}
