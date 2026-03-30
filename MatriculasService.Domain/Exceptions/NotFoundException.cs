namespace MatriculasService.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} com ID '{id}' não foi encontrado.") { }
}
