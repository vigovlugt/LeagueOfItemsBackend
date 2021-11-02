using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        private readonly string _uggPatchUrl;
        private readonly HttpClient _client;
        private readonly ILogger<GetUggVersionQueryHandler> _logger;

        public GetUggVersionQueryHandler(IHttpClientFactory clientFactory, IConfiguration configuration,
            ILogger<GetUggVersionQueryHandler> logger)
        {
            _logger = logger;
            _client = clientFactory.CreateClient();
            _uggPatchUrl = configuration["Ugg:PatchUpdateUrl"];
        }

        public async Task<string> Handle(GetUggVersionQuery request, CancellationToken cancellationToken)
        {
            var response = await _client.GetAsync(_uggPatchUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            var text = await response.Content.ReadAsStringAsync(cancellationToken);

            var jsonDictionary = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(text);
            if (jsonDictionary == null)
            {
                return null;
            }

            var version = jsonDictionary.Keys.Select(v => v.Split("_")
                    .Select(int.Parse).ToList())
                .Aggregate((max, v) => VersionIsGreater(max, v) ? max : v);
            
            var versionString = string.Join(".", version);

            _logger.LogInformation("Got Ugg version {Version}", versionString);

            return versionString;
        }

        private static bool VersionIsGreater(IReadOnlyList<int> a, IReadOnlyList<int> b)
        {
            var aMajor = a[0];
            var aMinor = a[1];

            var bMajor = b[0];
            var bMinor = b[1];

            if (aMajor > bMajor)
            {
                return true;
            }
            else if (bMajor < aMajor)
            {
                return false;
            }

            return bMinor < aMinor;
        }
    }
}