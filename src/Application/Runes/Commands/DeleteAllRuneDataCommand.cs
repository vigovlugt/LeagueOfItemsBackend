using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Runes.Commands;

public record DeleteAllRuneDataCommand : IRequest;

public class DeleteAllRuneDataCommandHandler : IRequestHandler<DeleteAllRuneDataCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteAllRuneDataCommandHandler> _logger;

    public DeleteAllRuneDataCommandHandler(IApplicationDbContext context,
        ILogger<DeleteAllRuneDataCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteAllRuneDataCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM RuneData;", cancellationToken);

        _logger.LogInformation("{RuneDataAmount} RuneData rows deleted", deleted);

        return Unit.Value;
    }
}