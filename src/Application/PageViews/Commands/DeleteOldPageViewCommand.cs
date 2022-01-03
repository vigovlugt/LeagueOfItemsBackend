using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.PageViews.Commands;

public record DeleteOldPageViewCommand : IRequest;

public class DeleteOldPageViewCommandHandler: IRequestHandler<DeleteOldPageViewCommand>
{
    private readonly IPageViewRepository _repository;
    private readonly ILogger<DeleteOldPageViewCommandHandler> _logger;

    public DeleteOldPageViewCommandHandler(ILogger<DeleteOldPageViewCommandHandler> logger, IPageViewRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    public async Task<Unit> Handle(DeleteOldPageViewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting old PageView data");
        var deletedAmount = await _repository.DeleteOldAsync();
        _logger.LogInformation("{Count} PageView rows deleted", deletedAmount);
        
        return Unit.Value;
    }
}