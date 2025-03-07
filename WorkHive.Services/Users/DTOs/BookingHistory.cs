using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.Users.DTOs;

public sealed class BookingHistory
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }

}