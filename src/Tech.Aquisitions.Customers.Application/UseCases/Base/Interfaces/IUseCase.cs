using Tech.Aquisitions.Customers.Domain.CrossCutting.MethodResultContext;

namespace Tech.Aquisitions.Customers.Application.UseCases.Base.Interfaces;

public interface IUseCase<TInput, TOutput>
{
    public Task<MethodResult<TOutput>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}

public interface IUseCase<TInput>
{
    public Task<MethodResult> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}
