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
}
