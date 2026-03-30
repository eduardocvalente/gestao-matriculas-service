using MatriculasService.Application.Common;
using MediatR;

namespace MatriculasService.Application.Primitives;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : BaseQuery, IRequest<Result<TResponse>>
    where TResponse : BaseResponse
{
}
