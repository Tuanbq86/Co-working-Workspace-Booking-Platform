using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.Exceptions;

namespace WorkHive.Services.Exceptions;

public class BookingNotFoundException : NotFoundException
{
    public BookingNotFoundException(string message) : base(message)
    {
        
    }
}
