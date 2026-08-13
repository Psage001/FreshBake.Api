namespace FreshBake.API.DTOs.Auth
{
    public class UserListDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int AccessLevelId { get; set; }
        public string AccessLevelName { get; set; }
    }
}
