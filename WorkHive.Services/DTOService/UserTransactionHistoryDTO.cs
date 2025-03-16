using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.DTOService;

public record UserTransactionHistoryDTO
{
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public DateTime? Created_At { get; set; }
}
