using MongoDataAccess.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Models;

public class ArtworkModel
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("index")]
    public int? Index { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("sold")]
    public bool Sold { get; set; }

    [BsonElement("soldTo")]
    public CustomerModel? SoldTo { get; set; }

}
