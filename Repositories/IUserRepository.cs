using UserService.Models;

namespace UserService.Repositories
{
	public interface IUserRepository
	{
		List<User> GetAllUsers();
		User GetUserById(string id);
		User GetUserByUsername(string username);
		User CreateUser(User user);
		void UpdateUser(string id, User userIn);
		void RemoveUser(string id);
	}
}
