using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Application.Ugg.Services
{
    public static class UggResponseParser
    {
        public static async Task<List<U>> Parse<T, U>(Stream responseStream, Func<Region, Rank, Role, T, U> fn)
        {
            var dataObj = await JsonSerializer.DeserializeAsync<Dictionary<int, Dictionary<int, Dictionary<int, T>>>>(
                responseStream);

            var dataList = new List<U>();

            foreach (var (region, rankRoleData) in dataObj)
            {
                foreach (var (rank, roleData) in rankRoleData)
                {
                    foreach (var (role, data) in roleData)
                    {
                        dataList.Add(fn((Region) region, (Rank) rank, (Role) role, data));
                    }
                }
            }

            return dataList;
        }
    }
}