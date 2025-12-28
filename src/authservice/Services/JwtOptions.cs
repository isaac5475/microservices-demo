namespace UserManagementAPI.Services
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiresHours { get; set; }
        public int RefreshTokenExpiresDays { get; set; }
    }
}