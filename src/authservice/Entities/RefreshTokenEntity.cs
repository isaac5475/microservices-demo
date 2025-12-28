namespace UserManagementAPI.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public Guid Token { get; set; }
        public DateTime Expires { get; set; }
        public Guid UserId { get; set; }
    }
}
