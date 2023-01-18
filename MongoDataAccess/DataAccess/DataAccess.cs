using MongoDataAccess.Interfaces;
using MongoDataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDataAccess.DataAccess;

public class DataAccess
{
    private string? ConnectionString;
    private const string DatabaseName = "gallery_db";

    private const string ArtworkCollection = "artworks";
    private const string CustomerCollection = "customers";
    public string SelectedCollection { get; set; }

    public void SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }
    public async Task<bool> CheckConnection()
    {
        try
        {
            var client = new MongoClient(ConnectionString);
            var databases = client.ListDatabasesAsync().Result;
            //databases.MoveNextAsync();
            if (client.Cluster.Description.State == ClusterState.Connected)
            {
                return true;
            }
            
        }
        catch (Exception)
        {
            return false;
        }
        return false;
    }

    private IMongoCollection<T> ConnectToMongo<T>(string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<ArtworkModel>> GetAllArtworks()
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        var results = await artworksCollection.FindAsync(_ => true);
        return results.ToList();
    }
    
    public async Task<List<CustomerModel>> GetAllCustomers()
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        var results = await customersCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<IModel>> GetAll()
    {
        MongoClient dbClient = new MongoClient(ConnectionString);
        var database = dbClient.GetDatabase(DatabaseName);
        var collection = database.GetCollection<IModel>(SelectedCollection);
        var results = await collection.FindAsync(_ => true);
        return results.ToList();
    }
}
