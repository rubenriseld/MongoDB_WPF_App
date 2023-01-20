using MongoDataAccess.Models;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace MongoDataAccess.DataAccess;

public class DataAccess
{
    private string? ConnectionString;
    private const string DatabaseName = "gallery_db";

    private const string ArtworkCollection = "artworks";
    private const string CustomerCollection = "customers";

    public void SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
    }
    public async Task<bool> CheckConnection()
    {
        try
        {
            var client = new MongoClient(ConnectionString);
            var databases = await client.ListDatabasesAsync();

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

    //******************
    //    ARTWORKS
    //******************
    public async Task<List<ArtworkModel>> GetAllArtworks()
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        var results = await artworksCollection.FindAsync(_ => true);
        return results.ToList();
    }
    public async Task<ArtworkModel> GetArtworkByIndex(int index)
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        var indexFilter = Builders<ArtworkModel>.Filter.Eq("Index", index);
        var document = await artworksCollection.Find(indexFilter).FirstOrDefaultAsync();
        return document;
    }
    public Task CreateArtwork(ArtworkModel artwork)
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        return artworksCollection.InsertOneAsync(artwork);
    }
    public Task UpdateArtwork(ArtworkModel artwork)
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        var filter = Builders<ArtworkModel>.Filter.Eq("Id", artwork.Id);
        return artworksCollection.ReplaceOneAsync(filter, artwork, new ReplaceOptions { IsUpsert = true });
    }
    public Task DeleteArtwork(ArtworkModel artwork)
    {
        var artworksCollection = ConnectToMongo<ArtworkModel>(ArtworkCollection);
        return artworksCollection.DeleteOneAsync(a => a.Id == artwork.Id);
    }

    //******************
    //    CUSTOMERS
    //******************

    public async Task<List<CustomerModel>> GetAllCustomers()
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        var results = await customersCollection.FindAsync(_ => true);
        return results.ToList();
    }
    public async Task<CustomerModel> GetCustomerByIndex(int index)
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        var indexFilter = Builders<CustomerModel>.Filter.Eq("Index", index);
        var document = await customersCollection.Find(indexFilter).FirstOrDefaultAsync();
        return document;
    }
    public Task CreateCustomer(CustomerModel customer)
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        return customersCollection.InsertOneAsync(customer);
    }
    public Task UpdateCustomer(CustomerModel customer)
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        var filter = Builders<CustomerModel>.Filter.Eq("Id", customer.Id);
        return customersCollection.ReplaceOneAsync(filter, customer, new ReplaceOptions { IsUpsert = true });
    }
    public Task DeleteCustomer(CustomerModel customer)
    {
        var customersCollection = ConnectToMongo<CustomerModel>(CustomerCollection);
        return customersCollection.DeleteOneAsync(a => a.Id == customer.Id);
    }
}