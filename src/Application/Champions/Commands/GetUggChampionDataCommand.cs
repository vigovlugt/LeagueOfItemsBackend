using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Champions.Services;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Champions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Champions.Commands
{
    public record GetUggChampionDataCommand : IRequest
    {
    }

    public class GetUggChampionDataCommandHandler : IRequestHandler<GetUggChampionDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetUggChampionDataCommandHandler> _logger;
        private readonly IMediator _mediator;

        public GetUggChampionDataCommandHandler(ILogger<GetUggChampionDataCommandHandler> logger,
            IApplicationDbContext context,
            IMediator mediator)
        {
            _logger = logger;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(GetUggChampionDataCommand request, CancellationToken cancellationToken)
        {
            var champions = await _context.Champions.ToListAsync(cancellationToken);
            var version = await _mediator.Send(new GetUggVersionQuery(), cancellationToken);

            var championDataTasks = champions.Select(champion => GetDataForChampion(version, champion)).ToList();

            await Task.WhenAll(championDataTasks);

            var championData = championDataTasks.SelectMany(x => x.Result).ToList();

            await SaveChampionData(championData);

            return Unit.Value;
        }

        private async Task<List<ChampionData>> GetDataForChampion(string version, Champion champion)
        {
            _logger.LogInformation("Downloading Champion data for {Champion}", champion.Name);

            await using var responseStream = await _mediator.Send(new GetUggApiResponse
            {
                ChampionId = champion.Id,
                Type = "overview",
                Version = version,
                Table = false
            });

            var parsedChampionData = await UggChampionDataParser.Parse(champion.Id, responseStream);

            var filteredChampionData = UggDataFilterer.Filter(parsedChampionData);

            return filteredChampionData;
        }


        private async Task SaveChampionData(List<ChampionData> championData)
        {
            await _mediator.Send(new DeleteAllChampionDataCommand());

            _context.ChampionData.AddRange(championData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ChampionDataAmount} ChampionData rows saved", championData.Count);
        }
    }
}