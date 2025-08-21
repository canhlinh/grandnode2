namespace Grand.Domain.Directory;

/// <summary>
///     Represents a Province
/// </summary>
public class Province : BaseEntity
{
    public string Name { get; set; }
    public string CountryId { get; set; }
    public int Version { get; set; }
    public int DisplayOrder { get; set; }

    public int NhanhVnId { get; set; } // Nhanh.vn Province ID
}