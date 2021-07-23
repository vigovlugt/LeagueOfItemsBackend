using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace LeagueOfItems.Application.Ugg.Queries
{
    public record GetUggApiResponse : IRequest<Stream>
    {
        public string Version { get; init; }
        public int ChampionId { get; init; }
        public string Type { get; init; }
        public bool Table { get; init; } = true;
    }

    public class GetUggApiResponseHandler : IRequestHandler<GetUggApiResponse, Stream>
    {
        private readonly HttpClient _client;

        public GetUggApiResponseHandler(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri(configuration["Ugg:ApiUrl"]);
        }

        public async Task<Stream> Handle(GetUggApiResponse request, CancellationToken cancellationToken)
        {
            var uggVersion = string.Join('_', request.Version.Split(".").Take(2));

            var prefix = request.Table ? "table/" : "";
            var requestUri =
                $"lol/1.1/{prefix}{request.Type}/{uggVersion}/ranked_solo_5x5/{request.ChampionId}/1.4.0.json";

            var response = await _client.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
    }
}