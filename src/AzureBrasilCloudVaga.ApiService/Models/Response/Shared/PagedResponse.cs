namespace AzureBrasilCloudVaga.ApiService.Models.Response.Shared
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public long TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }
}