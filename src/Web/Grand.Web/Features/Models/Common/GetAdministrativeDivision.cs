using Grand.Web.Models.Common;
using MediatR;

namespace Grand.Web.Features.Models.Common;

public class GetAdministrativeDivision : IRequest<IList<AdministrativeDivisionModel>>
{
    public int Version { get; set; }
    public string DivisionType { get; set; }
    public string ParentId { get; set; }
    public bool AddSelectStateItem { get; set; }
}