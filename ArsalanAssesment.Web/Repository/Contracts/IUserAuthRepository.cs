using ArsalanAssesment.Web.DTOs;
using ArsalanAssesment.Web.DTOs.UserDTOs;

namespace ArsalanAssesment.Web.Repository.Contracts
{
    public interface IUserAuthRepository
    {
        Task<ResponseDTO> RegisterUser(RegisterUserDTO registerUserDTO);
        Task<ResponseDTO> Login(LoginUserDTO loginUserDTO);
        Task<ResponseDTO> GetUser(Guid id);
        Task<ResponseDTO> UpdateUser(Guid id, UpdateUserDTO updateUserDTO);
        Task<ResponseDTO> GetAllUser();
        Task<ResponseDTO> DeleteUser(Guid id);
        String GenerateTokenString(string userEmail, Guid userGuid, List<string> roles);
    }
}
