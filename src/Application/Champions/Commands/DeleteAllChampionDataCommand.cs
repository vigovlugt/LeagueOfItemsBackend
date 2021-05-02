using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Champions.Commands
{
    public record DeleteAllChampionDataCommand : IRequest
    {
    }

    public class DeleteAllChampionDataCommandHandler : IRequestHandler<DeleteAllChampionDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteAllChampionDataCommandHandler> _logger;

        public DeleteAllChampionDataCommandHandler(IApplicationDbContext context,
            ILogger<DeleteAllChampionDataCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAllChampionDataCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM ChampionData;", cancellationToken);

            _logger.LogInformation("{ChampionDataAmount} ChampionData rows deleted", deleted);

            return Unit.Value;
        }
    }
}