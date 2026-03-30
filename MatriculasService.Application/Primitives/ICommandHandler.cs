using MatriculasService.Application.Common;
using MediatR;

namespace MatriculasService.Application.Primitives;

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : BaseCommand, IRequest<Result<TResponse>>
    where TResponse : BaseResponse
{
}
