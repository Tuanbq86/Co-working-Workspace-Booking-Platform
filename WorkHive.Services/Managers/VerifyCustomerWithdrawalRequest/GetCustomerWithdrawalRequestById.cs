using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Managers.VerifyCustomerWithdrawalRequest;

public record GetCustomerWithdrawalRequestByIdQuery(int CustomerRequestId) : IQuery<GetCustomerWithdrawalRequestByIdResult>;
public record GetCustomerWithdrawalRequestByIdResult(CustomerWithdrawalRequestByIdDTO CustomerWithdrawalRequestByIdDTOs);
public record CustomerWithdrawalRequestByIdDTO(
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

public class GetCustomerWithdrawalRequestById(IUserUnitOfWork userUnit)
    : IQueryHandler<GetCustomerWithdrawalRequestByIdQuery, GetCustomerWithdrawalRequestByIdResult>
{
    public async Task<GetCustomerWithdrawalRequestByIdResult> Handle(GetCustomerWithdrawalRequestByIdQuery query, 
        CancellationToken cancellationToken)
    {
        var checkNull = await userUnit.CustomerWithdrawalRequest.GetByIdAsync(query.CustomerRequestId);

        if(checkNull is null)
        {
            return new GetCustomerWithdrawalRequestByIdResult(new CustomerWithdrawalRequestByIdDTO(
                0,
                "N/A",
                "N/A",
                "N/A",
                null,
                0,
                null,
                "N/A",
                "N/A",
                "N/A",
                0,
                "N/A",
                null
            ));
        }
        var customerWithdrawalRequestByIdDTO = new CustomerWithdrawalRequestByIdDTO(
            checkNull.Id,
            checkNull.Title,
            checkNull.Description,
            checkNull.Status,
            checkNull.CreatedAt,
            checkNull.UserId,
            checkNull.ManagerId,
            checkNull.BankName,
            checkNull.BankNumber,
            checkNull.BankAccountName,
            checkNull.Balance ?? 0,
            checkNull.ManagerResponse ?? "N/A",
            checkNull.UpdatedAt
        );

        return new GetCustomerWithdrawalRequestByIdResult(customerWithdrawalRequestByIdDTO);
    }
}
