using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.SharedKernel.Attributes;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Features.Models.Common;
using Grand.Web.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers;

[ApiGroup(SharedKernel.Extensions.ApiConstants.ApiGroupNameV2)]
public class CountryController : BasePublicController
{
    #region Constructors

    public CountryController(IMediator mediator, ITranslationService translationService)
    {
        _mediator = mediator;
        _translationService = translationService;
    }

    #endregion

    #region States / provinces

    //available even when navigation is not allowed
    [PublicStore(true)]
    [HttpGet]
    public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
    {
        //this action method gets called via an ajax request
        if (string.IsNullOrEmpty(countryId))
            return Json(new List<StateProvinceModel>
                { new() { id = "", name = _translationService.GetResource("Address.SelectState") } });
        var model = await _mediator.Send(new GetStatesProvince
            { CountryId = countryId, AddSelectStateItem = addSelectStateItem });
        return Json(model);
    }
    
    [PublicStore(true)]
    [HttpGet]
    public virtual async Task<IActionResult> GetProvincesByCountryId(string countryId, bool addSelectStateItem)
    {
        //this action method gets called via an ajax request
        if (string.IsNullOrEmpty(countryId))
            return Json(new List<AdministrativeDivisionModel>
                { new() { id = "", name = _translationService.GetResource("Address.SelectState") } });
        var model = await _mediator.Send(new GetAdministrativeDivision()
            { DivisionType = "province", AddSelectStateItem = addSelectStateItem, ParentId = countryId, Version = 1});
        return Json(model);
    }
    
    [PublicStore(true)]
    [HttpGet]
    public virtual async Task<IActionResult> GetDistrictsByProvinceId(string provinceId, bool addSelectStateItem)
    {
        //this action method gets called via an ajax request
        if (string.IsNullOrEmpty(provinceId))
            return Json(new List<AdministrativeDivisionModel>
                { new() { id = "", name = _translationService.GetResource("Address.SelectDistrict") } });
        var model = await _mediator.Send(new GetAdministrativeDivision()
            { DivisionType = "district", AddSelectStateItem = addSelectStateItem, ParentId = provinceId, Version = 1});
        return Json(model);
    }
    
    [PublicStore(true)]
    [HttpGet]
    public virtual async Task<IActionResult> GetWardsByDistrictId(string districtId, bool addSelectStateItem)
    {
        //this action method gets called via an ajax request
        if (string.IsNullOrEmpty(districtId))
            return Json(new List<AdministrativeDivisionModel>
                { new() { id = "", name = _translationService.GetResource("Address.SelectWard") } });
        var model = await _mediator.Send(new GetAdministrativeDivision()
            { DivisionType = "ward", AddSelectStateItem = addSelectStateItem, ParentId = districtId, Version = 1});
        return Json(model);
    }

    #endregion

    #region Fields

    private readonly IMediator _mediator;
    private readonly ITranslationService _translationService;

    #endregion
}