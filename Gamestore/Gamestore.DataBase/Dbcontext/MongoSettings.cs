namespace Gamestore.Database.Dbcontext;

/// <summary>
/// Represents settings for a MongoDB connection.
/// </summary>
public class MongoSettings
{
    /// <summary>
    /// Gets or sets the connection string for the MongoDB server.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the database to be used.
    /// </summary>
    public string Database { get; set; }
}
