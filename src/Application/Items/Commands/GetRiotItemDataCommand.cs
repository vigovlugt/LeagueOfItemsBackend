using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Domain.Models;
using LeagueOfItems.Domain.Models.Riot;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Items.Commands
{
    public record GetRiotItemDataCommand : IRequest
    {
        public GetRiotItemDataCommand(string version)
        {
            Version = version;
        }
        
        public string Version { get; init; }
    }

    public class GetRiotItemDataCommandHandler : IRequestHandler<GetRiotItemDataCommand>
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetRiotItemDataCommandHandler> _logger;

        public GetRiotItemDataCommandHandler(IMediator mediator, IApplicationDbContext context,
            ILogger<GetRiotItemDataCommandHandler> logger)
        {
            _mediator = mediator;
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(GetRiotItemDataCommand request, CancellationToken cancellationToken)
        {
            var itemResponse = await GetRiotItemResponse(request.Version);

            var items = ParseRiotItems(itemResponse);

            _logger.LogInformation("{ItemAmount} items found", items.Count);

            await SaveItems(items);

            return Unit.Value;
        }

        private async Task<RiotItemResponse> GetRiotItemResponse(string version)
        {
            var responseStream = await _mediator.Send(new GetRiotApiResponse
            {
                Url = $"cdn/{version}/data/en_US/item.json"
            });

            var itemResponse = await JsonSerializer.DeserializeAsync<RiotItemResponse>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            if (itemResponse == null) throw new ArgumentException("LOL items endpoint returned null");

            return itemResponse;
        }

        private async Task SaveItems(List<Item> items)
        {
            await _mediator.Send(new DeleteAllItemsCommand());

            _context.Items.AddRange(items);

            await _context.SaveChangesAsync();

            _logger.LogInformation("{ItemAmount} items saved", items.Count);
        }
        
        private static List<Item> ParseRiotItems(RiotItemResponse itemResponse)
        {
            foreach (var (id, item) in itemResponse.Data) item.Id = id;

            return itemResponse.Data.Values.Select(Item.FromRiotItem).ToList();
        }
    }
}