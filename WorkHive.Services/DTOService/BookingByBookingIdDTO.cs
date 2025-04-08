using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.DTOService;

public class BookingByBookingIdDTO
{
    public int? BookingId { get; set; }
    public DateTime? Start_Date { get; set; }
    public DateTime? End_Date { get; set; }
    public decimal? Price { get; set; }
    public string? Status { get; set; }
    public DateTime? Created_At { get; set; }
    public string? Payment_Method { get;set; }
    public int UserId { get; set; }
    public int WorkspaceId { get; set; }
    public int? PromotionId { get; set; }
    public List<BookingAmenityByBookingId>? Amenities { get; set; }
    public List<BookingBeverageByBookingId>? Beverages { get; set; }
}

public record BookingAmenityByBookingId(int AmenityId, int Quantity, string AmenityName, string Image, decimal UnitPrice);
public record BookingBeverageByBookingId(int BeverageId, int Quantity, string BeverageName, string Image, decimal UnitPrice);
