using System.Collections.Generic;
using System.Linq;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Ugg.Services
{
    public static class UggDataFilterer
    {
        public static List<T> Filter<T>(IEnumerable<T> data) where T : IUggData
        {
            return data.Where(itemData => itemData.Region == UggRegion.World && itemData.Rank == UggRank.PlatinumPlus)
                .ToList();
        }
    }
}