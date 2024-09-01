namespace UserService.DTO
{
	public class UserCreateDto
	{
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public string Email { get; set; }
	}
}
