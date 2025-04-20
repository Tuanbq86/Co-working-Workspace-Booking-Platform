using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.Users.DTOs;

public class BookingBeverageDTO
{
    public int Id { get; set; }
    public int? Quantity { get; set; }

    public BookingBeverageDTO()
    {
        
    }

    public BookingBeverageDTO(int Id, int? Quantity)
    {
        this.Id = Id;
        this.Quantity = Quantity;
    }
}
