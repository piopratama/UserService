using MongoDB.Driver;
using UserService.Models;

namespace UserService.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IMongoCollection<User> _users;

		public UserRepository(IConfiguration config, string databaseName= "UserServiceDb")
		{
			var client = new MongoClient(config.GetConnectionString("MongoDb"));
			var database = client.GetDatabase(databaseName);
			_users = database.GetCollection<User>("Users");
		}

		public List<User> GetAllUsers() => _users.Find(user => true).ToList();

		public User GetUserById(string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

		public User GetUserByUsername(string username) => _users.Find<User>(user => user.Username == username).FirstOrDefault();

		public User CreateUser(User user)
		{
			_users.InsertOne(user);
			return user;
		}

		public void UpdateUser(string id, User userIn) => _users.ReplaceOne(user => user.Id == id, userIn);

		public void RemoveUser(string id) => _users.DeleteOne(user => user.Id == id);
	}
}
