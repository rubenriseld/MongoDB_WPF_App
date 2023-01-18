using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Interfaces;

public interface IModel
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("index")]
    public int Index { get; set; }
}
