using Grand.Domain.Directory;
using Grand.SharedKernel.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grand.Module.Installer.Services;

public partial class InstallationService
{
    protected virtual async Task InstallCountriesAndStates()
    {
        var cUsa = new Country {
            Name = "United States",
            AllowsBilling = true,
            AllowsShipping = true,
            TwoLetterIsoCode = "US",
            ThreeLetterIsoCode = "USA",
            NumericIsoCode = 840,
            SubjectToVat = false,
            DisplayOrder = 2,
            Published = true
        };
        await _countryRepository.InsertAsync(cUsa);

        var cVn = new Country
        {
            Name = "Viet Nam",
            AllowsBilling = true,
            AllowsShipping = true,
            TwoLetterIsoCode = "VN",
            ThreeLetterIsoCode = "VNM",
            NumericIsoCode = 704,
            SubjectToVat = false,
            DisplayOrder = 1,
            Published = true
        };
        await _countryRepository.InsertAsync(cVn);

        // Load data from Vietnam's Administrative divisions in the vietnam_administrative_divisions file
        // This file contains the hierarchical structure of provinces, districts, and wards
        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data/Resources/vietnam_administrative_divisions_v1.json");
        var json = await File.ReadAllTextAsync(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var locations = JsonSerializer.Deserialize<List<Location>>(json, options);
        foreach (var loc in locations!)
        {
            Console.WriteLine($"{loc.CityName} ({loc.CityId}) - {loc.DistrictName} - {loc.WardName}");
            var province = await _provinceRepository.GetOneAsync(x => x.NhanhVnId == loc.CityId && x.Version == 1);
            if (province == null)
            {
                province = new Province() {
                    CountryId = cVn.Id,
                    Name = loc.CityName,
                    NhanhVnId = loc.CityId,
                    Version = 1,
                };
                await _provinceRepository.InsertAsync(province);
            };
            
            var district = await _districtRepository.GetOneAsync(x => x.NhanhVnId == loc.DistrictId && x.ProvinceId == province.Id && x.Version == 1);
            if (district == null)
            {
                district = new District() {
                    Name = loc.DistrictName,
                    ProvinceId = province.Id,
                    NhanhVnId = loc.DistrictId,
                    Version = 1,
                };
                await _districtRepository.InsertAsync(district);
            }
            
            var ward = await _wardRepository.GetOneAsync(x => x.NhanhVnId == loc.WardId && x.DistrictId == district.Id && x.ProvinceId == province.Id && x.Version == 1);
            if (ward != null) continue;
            ward = new Ward() {
                Name = loc.WardName,
                DistrictId = district.Id,
                ProvinceId = province.Id,
                NhanhVnId = loc.WardId,
                Version = 1,
            };
            await _wardRepository.InsertAsync(ward);
        }
    }
    
    protected abstract class Location
    {
        [JsonPropertyName("city_id")]
        public int CityId { get; set; }
        
        [JsonPropertyName("city_name")]
        public string? CityName { get; set; }
        
        [JsonPropertyName("district_id")]
        public int DistrictId { get; set; }
        
        [JsonPropertyName("district_name")]
        public string? DistrictName { get; set; }
        
        [JsonPropertyName("ward_id")]
        public int WardId { get; set; }
        
        [JsonPropertyName("ward_name")]
        public string? WardName { get; set; }
    }
}


