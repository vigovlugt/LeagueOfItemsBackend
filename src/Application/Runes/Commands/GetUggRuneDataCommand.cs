using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Runes.Services;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Ugg;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Runes.Commands
{
    public record GetUggRuneDataCommand : IRequest
    {
    }

    public class GetUggRuneDataCommandHandler : IRequestHandler<GetUggRuneDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUggRuneDataCommandHandler> _logger;
        private readonly IMediator _mediator;

        public GetUggRuneDataCommandHandler(IApplicationDbContext context, ILogger<GetUggRuneDataCommandHandler> logger,
            IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(GetUggRuneDataCommand request, CancellationToken cancellationToken)
        {
            var champions = await _context.Champions.ToListAsync(cancellationToken);
            var version = await _mediator.Send(new GetUggVersionQuery(), cancellationToken);

            var runeDataLists = champions.Select(champion => GetRuneDataForChampion(version, champion)).ToList();

            await Task.WhenAll(runeDataLists);

            var runeData = runeDataLists.SelectMany(x => x.Result).ToList();

            await SaveRuneData(runeData);

            return Unit.Value;
        }

        private async Task<List<UggRuneData>> GetRuneDataForChampion(string version, Champion champion)
        {
            _logger.LogInformation("Downloading Rune data for {Champion}", champion.Name);

            var rawRuneData = await DownloadUggRuneData(version, champion.Id);

            var parsedItemData = UggRuneDataParser.Parse(champion.Id, rawRuneData);

            var filteredItemData = UggDataFilterer.Filter(parsedItemData);

            return filteredItemData;
        }

        private async Task<Dictionary<int, Dictionary<int, Dictionary<int, List<Dictionary<int, List<int>>>>>>>
            DownloadUggRuneData(string version, int championId)
        {
            await using var responseStream = await _mediator.Send(new GetUggApiResponse
            {
                ChampionId = championId,
                Type = "runes",
                Version = version
            });

            var runeData = await JsonSerializer
                .DeserializeAsync<Dictionary<int, Dictionary<int, Dictionary<int, List<Dictionary<int, List<int>>>>>>>(
                    responseStream);

            return runeData;
        }

        private async Task SaveRuneData(List<UggRuneData> runeData)
        {
            await _mediator.Send(new DeleteAllRuneDataCommand());

            _context.RuneData.AddRange(runeData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{RuneDataAmount} RuneData rows saved", runeData.Count);
        }
    }
}