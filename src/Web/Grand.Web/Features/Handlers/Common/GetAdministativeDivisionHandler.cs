using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure;
using Grand.Web.Features.Models.Common;
using Grand.Web.Models.Common;
using MediatR;

namespace Grand.Web.Features.Handlers.Common;

public class GetAdministativeDivisionHandler : IRequestHandler<GetAdministrativeDivision, IList<AdministrativeDivisionModel>>
{
    private readonly ICountryService _countryService;
    private readonly ITranslationService _translationService;
    private readonly IContextAccessor _contextAccessor;

    public GetAdministativeDivisionHandler(ICountryService countryService, IContextAccessor contextAccessor,
        ITranslationService translationService)
    {
        _countryService = countryService;
        _contextAccessor = contextAccessor;
        _translationService = translationService;
    }

    public async Task<IList<AdministrativeDivisionModel>> Handle(GetAdministrativeDivision request, CancellationToken cancellationToken)
    {
        var model = new List<AdministrativeDivisionModel>();
        var emptySelection = "Select a division";
        switch (request.DivisionType)
        {
            case "province":
                emptySelection = "Chọn tỉnh/thành phố";
                
                var provinces = await _countryService.GetProvincesByCountryId(request.ParentId, request.Version);
                model = provinces.Select(s => new AdministrativeDivisionModel
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();
                break;
            case "district":
                emptySelection = "Chọn quận/huyện";
                
                var districts = await _countryService.GetDistrictsByProvinceId(request.ParentId, request.Version);
                model = districts.Select(s => new AdministrativeDivisionModel
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();
                break;
            case "ward":
                emptySelection = "Chọn phường/xã";
                
                var wards = await _countryService.GetWardsByDistrictId(request.ParentId, request.Version);
                model = wards.Select(s => new AdministrativeDivisionModel
                {
                    id = s.Id,
                    name = s.Name
                }).ToList();
                break;
            default:
                // Handle other division types if necessary
                break;
        }
        if (request.AddSelectStateItem)
            model.Insert(0,
                new AdministrativeDivisionModel { id = "", name = emptySelection});
        return model;
    }
}