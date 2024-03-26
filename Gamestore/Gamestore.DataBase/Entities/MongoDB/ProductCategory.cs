using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Gamestore.Database.Entities.MongoDB;

/// <summary>
/// Represents a category with its details.
/// </summary>
public class ProductCategory
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    [NotMapped]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    [Key]
    [Column("Id")]
    [BsonElement("CategoryID")]
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    [BsonElement("CategoryName")]
    public string CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    [BsonElement("Description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the picture associated with the category.
    /// </summary>
    [BsonElement("Picture")]
    public string Picture { get; set; }
}
