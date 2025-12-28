namespace UserManagementAPI.Contracts
{
    public class RegisterUserRequest
    {
        required public string Email { get; set; }
        required public string Username { get; set; }
        required public string Password { get; set; }
    }
}
