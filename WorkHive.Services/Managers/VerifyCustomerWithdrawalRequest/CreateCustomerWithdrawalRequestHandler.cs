using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkHive.BuildingBlocks.CQRS;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record CreateCustomerWithdrawalRequestCommand(string Title, string Description, int CustomerId) : ICommand<CreateCustomerWithdrawalRequestResult>;
public record CreateCustomerWithdrawalRequestResult(string Notification);

public class CreateCustomerWithdrawalRequestHandler
{
}
