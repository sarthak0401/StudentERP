using Project_StudentERP.DTOs;

namespace Project_StudentERP.Interfaces
{
    public interface IAuthService
    {
        LoginResponseDTO UserLogin(LoginRequestDTO dto);
    }
}
