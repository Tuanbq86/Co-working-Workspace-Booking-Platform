using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record CustomerRequestDTO(
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
public record GetAllCustomerRequestQuery() : IQuery<GetAllCustomerRequestResult>;
public record GetAllCustomerRequestResult(List<CustomerRequestDTO> CustomerRequests);

public class GetAllCustomerRequestHandler(IUserUnitOfWork userUnit)
    : IQueryHandler<GetAllCustomerRequestQuery, GetAllCustomerRequestResult>
{
    public async Task<GetAllCustomerRequestResult> Handle(GetAllCustomerRequestQuery query, 
        CancellationToken cancellationToken)
    {
        List<CustomerRequestDTO> result = new List<CustomerRequestDTO>();

        var requests = await userUnit.CustomerWithdrawalRequest.GetAllAsync();

        foreach(var item in requests)
        {
            result.Add(new CustomerRequestDTO(
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

        return new GetAllCustomerRequestResult(result);
    }
}
