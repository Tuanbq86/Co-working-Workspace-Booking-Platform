using MediatR;

namespace WorkHive.BuildingBlocks.CQRS;
/*Sign the completed request with the Unit data type*/
/*TCommand, TResponse is a flexible data type defining by coder*/
public interface ICommand : IRequest<Unit>
{

}
/*Sign the completed request with the flexible TRequest data type*/
/*TCommand, TResponse is a flexible data type defining by coder*/
public interface ICommand<out TResponse> : IRequest<TResponse>
{

}
