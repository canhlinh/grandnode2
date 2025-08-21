namespace Grand.Domain.Directory;

/// <summary>
///     Represents a District
/// </summary>
public class District : BaseEntity
{
    public string Name { get; set; }
    public string ProvinceId { get; set; }
    public int Version { get; set; }
    public int DisplayOrder { get; set; }

    public int NhanhVnId { get; set; } // Nhanh.vn District ID
}
