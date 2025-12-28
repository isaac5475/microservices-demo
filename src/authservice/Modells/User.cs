namespace UserManagementAPI.Modells
{
    public class User
    {
        private User(Guid id, string passwordHash, string email, string username)
        {
            Id = id;
            PasswordHash = passwordHash;
            Email = email;
            Username = username;
        }

        public Guid Id { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public static User Create(Guid id, string passwordHash, string email, string username)
        {
            return new User(id, passwordHash, email, username);
        }
    }
}
