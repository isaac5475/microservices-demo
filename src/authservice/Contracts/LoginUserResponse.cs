using UserManagementAPI.Modells;

namespace UserManagementAPI.Contracts
{
    public class LoginUserResponse
    {
        required public string AccessToken { get; set; }
        required public string RefreshToken { get; set; }
        
        required public UserDTO User { get; set; }
        
    }
}
