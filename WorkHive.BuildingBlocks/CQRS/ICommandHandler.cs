using MediatR;

namespace WorkHive.BuildingBlocks.CQRS;

/*in and out keywords sign for the datatype entry and datatype exit for the command*/
/*TCommand, TResponse is a flexible data type defining by coder*/

/*The response will be Unit which be same as void in return function datatype and sign for complete*/
public interface ICommandHandler<in TCommand>
    : ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit>
{

}
/*TResponse is flexible for data type and where key word is sign the respone not null*/
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> where TResponse : notnull
{
}
