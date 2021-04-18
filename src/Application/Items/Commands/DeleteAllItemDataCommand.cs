using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Items.Commands
{
    public record DeleteAllItemDataCommand : IRequest
    {
        
    }
    
    public class DeleteAllItemDataCommandHandler : IRequestHandler<DeleteAllItemDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteAllItemDataCommandHandler> _logger;

        public DeleteAllItemDataCommandHandler(IApplicationDbContext context, ILogger<DeleteAllItemDataCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAllItemDataCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM ItemData;", cancellationToken);

            _logger.LogInformation("{ItemDataAmount} ItemData rows deleted", deleted);
            
            return Unit.Value;
        }
    }
}