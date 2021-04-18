using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LeagueOfItems.Application.Riot.Queries;
using LeagueOfItems.Domain.Models.Riot;
using MediatR;

namespace LeagueOfItems.Application.Runes.Commands
{
    public record GetRiotItemDataCommand : IRequest
    {
    }
    
    public class GetRiotItemDataCommandHandler : IRequestHandler<GetRiotItemDataCommand>
    {
        private readonly IMediator _mediator;

        public GetRiotItemDataCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Unit> Handle(GetRiotItemDataCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
        
        public async Task<List<RiotRunePath>> GetRuneResponse()
        {
            var version = await _mediator.Send(new GetRiotVersionQuery());

            var responseStream = await _mediator.Send(new GetRiotApiResponse
            {
                Url = $"cdn/{version}/data/en_US/runesReforged.json"
            });

            var runeResponse = await JsonSerializer.DeserializeAsync<List<RiotRunePath>>(responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return runeResponse;
        }
    }
}