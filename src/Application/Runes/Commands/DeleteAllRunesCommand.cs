using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Runes.Commands;

public record DeleteAllRunesCommand : IRequest;

public class DeleteAllRunesCommandHandler : IRequestHandler<DeleteAllRunesCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteAllRunesCommandHandler> _logger;

    public DeleteAllRunesCommandHandler(IApplicationDbContext context, ILogger<DeleteAllRunesCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteAllRunesCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM RunePaths;", cancellationToken);

        _logger.LogInformation("{RunePaths} RunePaths deleted", deleted);

        return Unit.Value;
    }
}