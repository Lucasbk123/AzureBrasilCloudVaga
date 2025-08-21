namespace AzureBrasilCloudVaga.Web.Models
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];

        public long TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}