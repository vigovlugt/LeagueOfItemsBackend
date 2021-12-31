using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Items.Commands
{
    public record DeleteAllItemsCommand : IRequest;

    public class DeleteAllItemsCommandHandler : IRequestHandler<DeleteAllItemsCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteAllItemsCommandHandler> _logger;

        public DeleteAllItemsCommandHandler(IApplicationDbContext context, ILogger<DeleteAllItemsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAllItemsCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _context.Database.ExecuteSqlRawAsync("DELETE FROM Items;", cancellationToken);

            _logger.LogInformation("{ItemAmount} Items deleted", deleted);
            
            return Unit.Value;
        }
    }
}