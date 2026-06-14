using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Lolalytics.Models;
using LeagueOfItems.Application.Lolalytics.Queries;
using LeagueOfItems.Application.Lolalytics.Services;
using LeagueOfItems.Domain.Models.Champions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Lolalytics.Commands;

public record GetLolalyticsDataCommand(string Version) : IRequest<LolalyticsParsedData>;

public class GetLolalyticsDataCommandHandler : IRequestHandler<GetLolalyticsDataCommand, LolalyticsParsedData>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetLolalyticsDataCommandHandler> _logger;
    private readonly IMediator _mediator;

    public GetLolalyticsDataCommandHandler(
        IApplicationDbContext context,
        ILogger<GetLolalyticsDataCommandHandler> logger,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<LolalyticsParsedData> Handle(GetLolalyticsDataCommand request,
        CancellationToken cancellationToken)
    {
        var champions = await _context.Champions
            .OrderBy(champion => champion.Name)
            .ToListAsync(cancellationToken);
        var itemIds = (await _context.Items.Select(item => item.Id).ToListAsync(cancellationToken)).ToHashSet();
        var runeIds = (await _context.Runes.Select(rune => rune.Id).ToListAsync(cancellationToken)).ToHashSet();

        var data = new LolalyticsParsedData();
        foreach (var champion in champions)
        {
            data.Add(await GetDataForChampion(request.Version, champion, itemIds, runeIds, cancellationToken));
        }

        await SaveData(data);

        return data;
    }

    private async Task<LolalyticsParsedData> GetDataForChampion(
        string version,
        Champion champion,
        HashSet<int> itemIds,
        HashSet<int> runeIds,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading Lolalytics data for {Champion} Version: {Version}", champion.Name,
            version);

        var parsedData = new LolalyticsParsedData();
        var defaultData = await GetChampionRoleData(version, champion, null, cancellationToken);
        if (defaultData?.Skill6 == null || defaultData.Header == null)
        {
            _logger.LogWarning("No Lolalytics response for Champion {Champion}", champion.Name);
            return parsedData;
        }

        var defaultRole = defaultData.Header.Lane;
        var roles = LolalyticsRoleMapper.GetRolesWithData(defaultData.Nav?.Lanes);
        if (!string.IsNullOrEmpty(defaultRole) && !roles.Contains(defaultRole))
        {
            roles.Insert(0, defaultRole);
        }

        foreach (var role in roles)
        {
            var roleData = role == defaultRole
                ? defaultData
                : await GetChampionRoleData(version, champion, role, cancellationToken);

            if (roleData?.Header == null || roleData.Header.N <= 0)
            {
                _logger.LogInformation("No Lolalytics data for {Champion} Role: {Role}", champion.Name, role);
                continue;
            }

            parsedData.Add(LolalyticsDataParser.Parse(champion.Id, version, role, roleData, itemIds, runeIds));
        }

        return parsedData;
    }

    private Task<LolalyticsQwikChampionData> GetChampionRoleData(
        string version,
        Champion champion,
        string role,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new GetLolalyticsQwikChampionQuery(version, champion.RiotId, role), cancellationToken);
    }

    private async Task SaveData(LolalyticsParsedData data)
    {
        _context.ChampionData.AddRange(data.ChampionData);
        _context.ItemData.AddRange(data.ItemData);
        _context.RuneData.AddRange(data.RuneData);
        _context.BuildPathData.AddRange(data.BuildPathData);

        await _context.SaveChangesAsync();

        _logger.LogInformation("{ChampionDataAmount} ChampionData rows saved", data.ChampionData.Count);
        _logger.LogInformation("{ItemDataAmount} ItemData rows saved", data.ItemData.Count);
        _logger.LogInformation("{RuneDataAmount} RuneData rows saved", data.RuneData.Count);
        _logger.LogInformation("{BuildPathDataAmount} BuildPathData rows saved", data.BuildPathData.Count);
    }
}
