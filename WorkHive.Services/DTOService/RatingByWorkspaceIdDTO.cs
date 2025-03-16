namespace WorkHive.Services.DTOService;

public class RatingByWorkspaceIdDTO
{
    public Byte? Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime? Created_At { get; set; }
    public string? Workspace_Name { get; set; }
    public string? Owner_Name { get; set; }
    public string? User_Name { get; set; }
    public string? User_Avatar { get; set; }
    public List<RatingByWorkspaceIdImageDTO>? Images { get; set; }
}
public record RatingByWorkspaceIdImageDTO(string Url);