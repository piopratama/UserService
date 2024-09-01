using MongoDB.Driver;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services
{
	public class UserService
	{
		private readonly IUserRepository _userRepository;

		public UserService(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public List<User> Get() => _userRepository.GetAllUsers();

		public User Get(string id) => _userRepository.GetUserById(id);

		public User Create(User user) => _userRepository.CreateUser(user);

		public void Update(string id, User userIn) => _userRepository.UpdateUser(id, userIn);

		public void Remove(User userIn) => _userRepository.RemoveUser(userIn.Id);

		public void Remove(string id) => _userRepository.RemoveUser(id);

		public User Authenticate(string username, string password)
		{
			var user = _userRepository.GetUserByUsername(username);

			if (user == null || !VerifyPassword(password, user.PasswordHash))
				return null;

			return user;
		}

		private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
		{
			return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
		}
	}
}
