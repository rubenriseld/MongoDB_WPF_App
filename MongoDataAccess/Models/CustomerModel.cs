using MongoDataAccess.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Models;

public class CustomerModel
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("index")]
    public int Index { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; }

    [BsonElement("emailAddress")]
    public string EmailAddress { get; set; }

}
