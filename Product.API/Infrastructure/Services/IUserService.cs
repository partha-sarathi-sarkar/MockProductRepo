using Product.API.Model;
using System.Threading.Tasks;

namespace Product.API.Infrastructure.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
    }
}
