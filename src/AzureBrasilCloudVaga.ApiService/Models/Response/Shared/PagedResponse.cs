namespace AzureBrasilCloudVaga.ApiService.Models.Response.Shared
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public int TotalRecords { get; set; }

        public string OdataNextLink { get; set; }
    }
}