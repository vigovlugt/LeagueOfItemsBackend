using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Champions.Commands
{
    public record DeleteAllChampionsCommand : IRequest
    {
    }

    public class DeleteAllChampionsCommandHandler : IRequestHandler<DeleteAllChampionsCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteAllChampionsCommandHandler> _logger;

        public DeleteAllChampionsCommandHandler(IApplicationDbContext context,
            ILogger<DeleteAllChampionsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAllChampionsCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM Champions;", cancellationToken);

            _logger.LogInformation("{ChampionAmount} champions deleted", deleted);

            return Unit.Value;
        }
    }
}