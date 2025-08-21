using Microsoft.AspNetCore.Mvc;

namespace AzureBrasilCloudVaga.ApiService.Models.Request.Shared
{
    public abstract class PaginatedBaseRequest
    {
        [FromQuery]
        public int PageNumber { get; set; }

        [FromQuery]
        public int PageSize { get; set; }
    }
}
