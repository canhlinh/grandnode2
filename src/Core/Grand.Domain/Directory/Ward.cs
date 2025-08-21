namespace Grand.Domain.Directory;

/// <summary>
///     Represents a Ward
/// </summary>
public class Ward : BaseEntity
{
    public string Name { get; set; }
    public string DistrictId { get; set; }
    public string ProvinceId { get; set; }

    public int Version { get; set; }
    public int DisplayOrder { get; set; }

    public int NhanhVnId { get; set; } // Nhanh.vn Ward ID
}
