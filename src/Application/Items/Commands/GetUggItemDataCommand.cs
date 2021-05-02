using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Items.Services;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Champions;
using LeagueOfItems.Domain.Models.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Items.Commands
{
    public record GetUggItemDataCommand : IRequest
    {
    }

    public class GetUggItemDataCommandHandler : IRequestHandler<GetUggItemDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUggItemDataCommandHandler> _logger;
        private readonly IMediator _mediator;

        public GetUggItemDataCommandHandler(ILogger<GetUggItemDataCommandHandler> logger, IApplicationDbContext context,
            IMediator mediator)
        {
            _logger = logger;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(GetUggItemDataCommand request, CancellationToken cancellationToken)
        {
            var champions = await _context.Champions.ToListAsync(cancellationToken);
            var version = await _mediator.Send(new GetUggVersionQuery(), cancellationToken);

            var itemDataLists = champions.Select(champion => GetItemDataForChampion(version, champion)).ToList();

            await Task.WhenAll(itemDataLists);

            var itemData = itemDataLists.SelectMany(x => x.Result).ToList();

            await SaveItemData(itemData);

            return Unit.Value;
        }

        private async Task<List<ItemData>> GetItemDataForChampion(string version, Champion champion)
        {
            _logger.LogInformation("Downloading Item data for {Champion}", champion.Name);

            await using var responseStream = await _mediator.Send(new GetUggApiResponse
            {
                ChampionId = champion.Id,
                Type = "items",
                Version = version
            });

            var parsedItemData = await UggItemDataParser.Parse(champion.Id, responseStream);

            var filteredItemData = UggDataFilterer.Filter(parsedItemData);

            return filteredItemData;
        }

        private async Task SaveItemData(List<ItemData> itemData)
        {
            await _mediator.Send(new DeleteAllItemDataCommand());

            _context.ItemData.AddRange(itemData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ItemDataAmount} ItemData rows saved", itemData.Count);
        }
    }
}