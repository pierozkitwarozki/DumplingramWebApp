using System.Threading.Tasks;
using Dumplingram.API.Dtos;

namespace Dumplingram.API.Services
{
    public interface IAuthService
    {
         Task<UserForDetailedDto> RegisterAsync(UserForRegisterDto userForRegisterDto);
         Task<object> LoginAsync(UserForLoginDto userForLogin);
    }
}