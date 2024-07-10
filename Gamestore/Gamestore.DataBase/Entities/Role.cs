using System.ComponentModel.DataAnnotations.Schema;

namespace Gamestore.Database.Entities;
public class Role
{
    /// <summary>
    /// Gets or sets the unique identifier of the Role.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Role permissions contract.
    /// </summary>
    [NotMapped]
    public string[] RolePermissionContract { get; set; }
}
