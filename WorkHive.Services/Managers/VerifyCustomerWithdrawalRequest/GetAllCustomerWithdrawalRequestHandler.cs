using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record CustomerWithdrawalRequestDTO(
        int Id,
        string Title,
        string Description,
        string Status,
        DateTime? CreatedAt,
        int CustomerId,
        int? ManagerId,
        string BankName,
        string BankNumber,
        string BankAccountName,
        decimal Balance,
        string ManagerResponse,
        DateTime? UpdatedAt
    );

public record GetAllCustomerWithdrawalRequestByCustomerIdQuery(int CustomerId) : IQuery<GetAllCustomerWithdrawalRequestByCustomerIdResult>;
public record GetAllCustomerWithdrawalRequestByCustomerIdResult(List<CustomerWithdrawalRequestDTO> CustomerWithdrawalRequests);

public class GetAllCustomerWithdrawalRequestHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllCustomerWithdrawalRequestByCustomerIdQuery, GetAllCustomerWithdrawalRequestByCustomerIdResult>
{
    public async Task<GetAllCustomerWithdrawalRequestByCustomerIdResult> Handle(GetAllCustomerWithdrawalRequestByCustomerIdQuery request, 
        CancellationToken cancellationToken)
    {
        var requests = await userUnit.CustomerWithdrawalRequest.GetByCustomerIdAsync(request.CustomerId);

        List<CustomerWithdrawalRequestDTO> result = new List<CustomerWithdrawalRequestDTO>();

        foreach(var item in requests)
        {
            result.Add(new CustomerWithdrawalRequestDTO(
                item.Id,
                item.Title,
                item.Description,
                item.Status,
                item.CreatedAt,
                item.UserId,
                item.ManagerId,
                item.BankName,
                item.BankNumber,
                item.BankAccountName,
                item.Balance ?? 0,
                item.ManagerResponse ?? "N/A",
                item.UpdatedAt
                ));
        }
        return new GetAllCustomerWithdrawalRequestByCustomerIdResult(result);
    }
}
