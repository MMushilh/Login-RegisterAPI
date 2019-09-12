using System.Threading.Tasks;

namespace UserRE.API.Data
{
    public interface IAuthRepository
    {
         Task<Model.User> Register (Model.User user, string password);
         Task<Model.User> Login (string username , string password);
         Task<bool> UserExsist (string username);
    }
}