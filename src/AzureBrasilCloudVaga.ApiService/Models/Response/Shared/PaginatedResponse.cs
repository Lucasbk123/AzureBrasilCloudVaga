namespace AzureBrasilCloudVaga.ApiService.Models.Response.Shared
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public long TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}