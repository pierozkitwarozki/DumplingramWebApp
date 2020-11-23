using System.Threading.Tasks;
using Dumplingram.API.Dtos;

namespace Dumplingram.API.Services
{
    public interface IAuthService
    {
         Task<UserForDetailedDto> Register(UserForRegisterDto userForRegisterDto);
         Task<object> Login(UserForLoginDto userForLogin);
    }
}