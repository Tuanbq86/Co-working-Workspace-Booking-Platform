namespace WorkHive.Services.DTOService;

public class RatingByUserIdDTO
{
    public Byte? Rate { get; set; }
    public string? Comment { get; set; }
    public DateTime? Created_At { get; set; }
    public string? Workspace_Name { get; set; }
    public string? Owner_Name { get; set; }
    public int RatingId { get; set; }
    public int UserId {  get; set; }
    public List<RatingImageDTO>? Images { get; set; }
}
public record RatingImageDTO(string Url);
