using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHive.Services.Constant.Wallet;

public enum DepositStatus
{
    PENDING,
    PAID,
    FAILED,
    EXPIRED,
    REFUNDED
}
