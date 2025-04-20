using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.BuildingBlocks.CQRS;
using WorkHive.Repositories.IUnitOfWork;

namespace WorkHive.Services.Google_Login
{
    public record UpdateUserPhoneCommand(int UserId, string Phone) : ICommand<UpdateUserPhoneResult>;

    public record UpdateUserPhoneResult(string Notification);

    public class UpdateUserPhoneValidator : AbstractValidator<UpdateUserPhoneCommand>
    {
        public UpdateUserPhoneValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId không hợp lệ");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone không được để trống");
        }
    }

    public class UpdateUserPhoneHandler(IUserUnitOfWork userUnitOfWork)
        : ICommandHandler<UpdateUserPhoneCommand, UpdateUserPhoneResult>
    {
        public async Task<UpdateUserPhoneResult> Handle(UpdateUserPhoneCommand command, CancellationToken cancellationToken)
        {
            var user = await userUnitOfWork.User.GetByIdAsync(command.UserId);
            if (user == null)
            {
                return new UpdateUserPhoneResult($"Không tìm thấy người dùng với ID {command.UserId}");
            }

            user.Phone = command.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            await userUnitOfWork.SaveAsync();

            return new UpdateUserPhoneResult($"Số điện thoại người dùng '{user.Name}' đã được cập nhật thành công.");
        }
    }
}
