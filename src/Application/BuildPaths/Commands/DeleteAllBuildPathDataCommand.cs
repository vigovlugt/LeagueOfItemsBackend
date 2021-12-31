using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.BuildPaths.Commands
{
    public record DeleteAllBuildPathDataCommand: IRequest;
    
    public class DeleteAllBuildPathDataCommandHandler : IRequestHandler<DeleteAllBuildPathDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteAllBuildPathDataCommandHandler> _logger;

        public DeleteAllBuildPathDataCommandHandler(IApplicationDbContext context,
            ILogger<DeleteAllBuildPathDataCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAllBuildPathDataCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM BuildPathData;", cancellationToken);

            _logger.LogInformation("{BuildPathDataAmount} BuildPathData rows deleted", deleted);

            return Unit.Value;
        }
    }
}