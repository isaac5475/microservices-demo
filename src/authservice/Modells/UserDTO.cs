namespace UserManagementAPI.Modells;

public class UserDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    
    public UserDTO(string username, string email)
    {
        Username = username;
        Email = email;
    }
}