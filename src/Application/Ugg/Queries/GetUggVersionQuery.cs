using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LeagueOfItems.Application.Ugg.Queries
{
    public record GetUggVersionQuery : IRequest<string>
    {
    }

    public class GetUggVersionQueryHandler : IRequestHandler<GetUggVersionQuery, string>
    {
        private readonly string _uggPageUrl;
        private readonly HttpClient _client;
        private readonly ILogger<GetUggVersionQueryHandler> _logger;

        public GetUggVersionQueryHandler(IHttpClientFactory clientFactory, IConfiguration configuration,
            ILogger<GetUggVersionQueryHandler> logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _uggPageUrl = configuration["Ugg:PatchPageUrl"];
        }

        public async Task<string> Handle(GetUggVersionQuery request, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync(_uggPageUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            var match = new Regex("Patch (\\d+\\.\\d+)").Match(html);

            if (!match.Success)
            {
                _logger.LogCritical("Couldn't find Ugg Version in Tierlist page");
                return null;
            }

            var version = match.Groups[1].Value;
            
            _logger.LogInformation("Got Ugg version {Version}", version);

            return version;
        }
    }
}