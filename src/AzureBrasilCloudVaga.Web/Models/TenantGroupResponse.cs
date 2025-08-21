namespace AzureBrasilCloudVaga.Web.Models;

public record TenantGroupResponse(string Id,string DisplayName,string Description,DateTimeOffset CreatedDateTime);


public class TenantGroupViewModel
{
    public TenantGroupViewModel()
    {
        
    }

    public TenantGroupViewModel(string id, string displayName, string description, DateTimeOffset createdDateTime)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        CreatedDateTime = createdDateTime;
    }

    public string Id { get; set; }
    public string DisplayName { get; set; }

    public string Description { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }
}