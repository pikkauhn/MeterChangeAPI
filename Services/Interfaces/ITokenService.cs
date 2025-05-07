using MeterChangeApi.Models;

namespace MeterChangeApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(Users user);
    }
}
