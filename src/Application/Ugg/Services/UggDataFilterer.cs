using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Application.Ugg.Services
{
    public static class UggDataFilterer
    {
        public static List<T> Filter<T>(IEnumerable<T> data) where T : IData
        {
            return data.Where(itemData => itemData.Region == Region.World && itemData.Rank == Rank.PlatinumPlus)
                .ToList();
        }
    }
}