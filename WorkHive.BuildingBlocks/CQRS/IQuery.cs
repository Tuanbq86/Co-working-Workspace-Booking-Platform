using MediatR;

namespace WorkHive.BuildingBlocks.CQRS;

/*TCommand, TResponse is a flexible data type defining by coder*/
/*in and out keywords sign for the datatype entry and datatype exit for the command*/
public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull
{

}
