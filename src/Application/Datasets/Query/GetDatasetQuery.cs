using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Builds;
using LeagueOfItems.Application.Champions.Queries;
using LeagueOfItems.Application.Champions.Services;
using LeagueOfItems.Application.Items.Queries;
using LeagueOfItems.Application.Items.Services;
using LeagueOfItems.Application.PageViews.Commands;
using LeagueOfItems.Application.PageViews.Queries;
using LeagueOfItems.Application.Patches.Queries;
using LeagueOfItems.Application.Patches.Services;
using LeagueOfItems.Application.Runes.Queries;
using LeagueOfItems.Application.Runes.Services;
using LeagueOfItems.Application.Ugg.Helpers;
using LeagueOfItems.Application.Ugg.Queries;
using LeagueOfItems.Domain.Models.Datasets;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Datasets.Query;

public record GetDatasetCommand : IRequest<Dataset>;

public class GetDatasetCommandHandler : IRequestHandler<GetDatasetCommand, Dataset>
{
    private readonly ILogger<GetDatasetCommandHandler> _logger;
    private readonly IMediator _mediator;

    public GetDatasetCommandHandler(IMediator mediator, ILogger<GetDatasetCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Dataset> Handle(GetDatasetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting Dataset as JSON");
        var patch = await _mediator.Send(new GetUggVersionQuery(), cancellationToken);
        var previousPatch = LolVersionHelper.GetPreviousVersion(patch);

        _logger.LogInformation("Getting all Items");
        var itemStats = await _mediator.Send(new GetAllItemsQuery(patch), cancellationToken);
        var previousItemStats = await _mediator.Send(new GetAllItemsQuery(previousPatch), cancellationToken);
        PreviousItemStatsService.SetPreviousItemStats(itemStats, previousItemStats);

        _logger.LogInformation("Getting all Runes");
        var runeStats = await _mediator.Send(new GetAllRunesQuery(patch), cancellationToken);
        var previousRuneStats = await _mediator.Send(new GetAllRunesQuery(previousPatch), cancellationToken);
        PreviousRuneStatsService.SetPreviousRuneStats(runeStats, previousRuneStats);

        _logger.LogInformation("Getting all Champions");
        var championStats = await _mediator.Send(new GetAllChampionsQuery(patch), cancellationToken);
        var previousChampionStats = await _mediator.Send(new GetAllChampionsQuery(previousPatch), cancellationToken);
        PreviousChampionStatsService.SetPreviousChampionStats(championStats, previousChampionStats);

        await _mediator.Send(new DeleteOldPageViewCommand(), cancellationToken);
        var pageViewDataset = await _mediator.Send(new GetPageViewsQuery(), cancellationToken);

        var patchSchedule = await _mediator.Send(new GetPatchScheduleQuery(), cancellationToken);
        var patchNotes = await _mediator.Send(new GetPatchNotesQuery(patch), cancellationToken);
        PatchNotesParserService.PostProcess(patchNotes, championStats, runeStats, itemStats);

        var dataset = new Dataset
        {
            Items = itemStats,
            Runes = runeStats,
            Champions = championStats,
            Version = patch,
            ChampionMatches = championStats.Sum(s => s.Matches),
            PreviousChampionMatches = previousChampionStats.Sum(s => s.Matches),
            PatchSchedule = patchSchedule,
            PageView = pageViewDataset,
            PatchNotes = patchNotes
        };

        return dataset;
    }
}