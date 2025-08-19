using Microsoft.AspNetCore.Mvc;

namespace AzureBrasilCloudVaga.ApiService.Models.Request;

public class TenantGroupRequest
{
    [FromQuery(Name ="limit")]
    public int Limit { get; set; }


    /// <summary>
    /// Propriedade específica do Azure para buscar a próxima página.
    /// </summary>

    [FromQuery(Name = "odataNextLink")]
    public string OdataNextLink { get; set; }
}
