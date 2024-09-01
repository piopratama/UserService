using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserService.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("username")]
		public string Username { get; set; }

		[BsonElement("passwordHash")]
		public string PasswordHash { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }
	}
}
