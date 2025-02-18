using MediatR;

namespace WorkHive.BuildingBlocks.CQRS;

/*TCommand, TResponse is a flexible data type defining by coder*/
/*in and out keywords sign for the datatype entry and datatype exit for the command*/
/*TResponse is flexible for data type and where key word is sign the respone not null*/
/*The response will be Unit which be same as void in return function datatype and sign for complete*/
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}
