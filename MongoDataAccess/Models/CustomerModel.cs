using MongoDataAccess.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Models;

public class CustomerModel : IModel
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

    [BsonElement("emailAdress")]
    public string EmailAddress { get; set; }

    [BsonElement("adress")]
    public string Address { get; set; }

}
