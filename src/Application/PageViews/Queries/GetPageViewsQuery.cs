using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.PageViews;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.PageViews.Queries;

public record GetPageViewsQuery : IRequest<PageViewDataset>;

public class GetPageViewsQueryHandler : IRequestHandler<GetPageViewsQuery, PageViewDataset>
{
    private readonly IPageViewRepository _repository;
    private readonly ILogger<GetPageViewsQueryHandler> _logger;

    public GetPageViewsQueryHandler(ILogger<GetPageViewsQueryHandler> logger, IPageViewRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    public async Task<PageViewDataset> Handle(GetPageViewsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting PageView data");
        var pageViewDataset = await _repository.GetDatasetAsync();
        
        return pageViewDataset;
    }
}