using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace WorkHive.Services.Users.DTOs;

public sealed class BookingHistory
{
    public DateTime Booking_StartDate { get; set; }
    public DateTime Booking_EndDate { get; set; }
    public decimal? Booking_Price { get; set; }
    public string? Booking_Status { get; set; }
    public DateTime Booking_CreatedAt { get; set; }
    public string? Payment_Method { get; set; }
    public string? Workspace_Name { get; set; }
    public int? Workspace_Capacity { get; set; }
    public string? Workspace_Category { get; set; }
    public string? Workspace_Description { get; set; }
    public int? Workspace_Area { get; set; }
    public int? Workspace_CleanTime { get; set; }
    public string? Promotion_Code { get; set; }
    public int? Discount { get; set; }
    public string? License_Name { get; set; }
    public string? License_Address { get; set; }
    //public string? google_map_url { get; set; }
    public List<BookingHistoryAmenity>? BookingHistoryAmenities { get; set; }
    public List<BookingHistoryBeverage>? BookingHistoryBeverages { get; set; }
    public List<BookingHistoryWorkspaceImage>? BookingHistoryWorkspaceImages { get; set; }
}

public record BookingHistoryAmenity(int Quantity, string Name, decimal UnitPrice, string ImageUrl);
public record BookingHistoryBeverage(int Quantity, string Name, decimal UnitPrice, string ImageUrl);
public record BookingHistoryWorkspaceImage(string ImageUrl);
