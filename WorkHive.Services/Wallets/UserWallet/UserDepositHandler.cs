using Microsoft.Extensions.Configuration;
using Net.payOS.Types;
using Net.payOS;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Data.Models;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Services.Constant.Wallet;

namespace WorkHive.Services.Wallets.UserWallet;
public record UserDepositCommand(int UserId, int Amount) 
    : ICommand<UserDepositResult>;
public record UserDepositResult(int CustomerWalletId, string Bin, string AccountNumber, int Amount, string Description,
    long OrderCode, string PaymentLinkId, string Status, string CheckoutUrl, string QRCode);

public class UserDepositHandler(IUserUnitOfWork userUnit, IConfiguration configuration)
    : ICommandHandler<UserDepositCommand, UserDepositResult>
{
    private readonly string ClientID = configuration["PayOS:ClientId"]!;
    private readonly string ApiKey = configuration["PayOS:ApiKey"]!;
    private readonly string CheckSumKey = configuration["PayOS:CheckSumKey"]!;
    public async Task<UserDepositResult> Handle(UserDepositCommand command, 
        CancellationToken cancellationToken)
    {
        var checkUserWallet = userUnit.CustomerWallet.GetAll()
            .Where(cw => cw.UserId.Equals(command.UserId)).FirstOrDefault();

        if(checkUserWallet is null)
        {
            var wallet = new Wallet
            {
                Balance = 0,
                Status = WalletStatus.Active.ToString()
            };
            await userUnit.Wallet.CreateAsync(wallet);

            var customerWallet = new CustomerWallet
            {
                Status = WalletStatus.Active.ToString(),
                WalletId = wallet.Id,
                UserId = command.UserId
            };
            await userUnit.CustomerWallet.CreateAsync(customerWallet);

            //Integrate payOS for deposit
            var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);
            var items = new List<ItemData>();

            items.Add(new ItemData(userUnit.User.GetById(command.UserId).Name, 1, command.Amount));

            //create order code with time increasing by time
            var depositeCode = long.Parse(DateTime.UtcNow.Ticks.ToString()[^10..]);

            var domain = configuration["PayOS:Domain"]!;
            var paymentLinkRequest = new PaymentData(
                    orderCode: depositeCode,
                    amount: command.Amount,
                    description: "depopayment",
                    returnUrl: domain + "/success",
                    cancelUrl: domain + "/fail",
            items: items
            );

            var link = await payOS.createPaymentLink(paymentLinkRequest);
            return new UserDepositResult(customerWallet.Id, link.bin, link.accountNumber, link.amount, link.description,
            link.orderCode, link.paymentLinkId, link.status, link.checkoutUrl, link.qrCode);
        }
        else
        {
            //checkUserWallet is not null
            //Integrate payOS for deposit
            var payOS = new PayOS(ClientID, ApiKey, CheckSumKey);
            var items = new List<ItemData>();

            items.Add(new ItemData(userUnit.User.GetById(command.UserId).Name, 1, command.Amount));

            //create order code with time increasing by time
            var timestamp = DateTime.UtcNow.Ticks.ToString()[^6..]; // Lấy 6 chữ số cuối của timestamp
            var depositeCode = long.Parse($"{checkUserWallet.Id}{timestamp}"); // Kết hợp bookingId và timestamp
            //Tạo thời gian hết hạn cho link thanh toán
            var expiredAt = DateTimeOffset.Now.AddMinutes(3).ToUnixTimeSeconds();

            var domain = configuration["PayOS:Domain"]!;
            var paymentLinkRequest = new PaymentData(
                    orderCode: depositeCode,
                    amount: command.Amount,
                    description: "userdeposit",
                    returnUrl: domain + "/success",
                    cancelUrl: domain + "/fail",
                    expiredAt: expiredAt,
                    items: items
            );

            var link = await payOS.createPaymentLink(paymentLinkRequest);

            return new UserDepositResult(checkUserWallet.Id, link.bin, link.accountNumber, link.amount, link.description,
            link.orderCode, link.paymentLinkId, link.status, link.checkoutUrl, link.qrCode);
        }
    }
}
