namespace UserManagementAPI.Contracts
{
    public class LoginUserRequest
    {
        required public string Username { get; set; }
        required public string Password { get; set; }
    }
}
