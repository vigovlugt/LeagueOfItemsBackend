using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Items.Services
{
    public static class UggItemDataParser
    {
        public static List<UggItemData> Parse(
            int championId,
            Dictionary<int, Dictionary<int, Dictionary<int, List<List<JsonElement>>>>> data)
        {
            var itemDataList = new List<UggItemData>();

            foreach (var (regionIndex, rankRoleData) in data)
            {
                var region = (UggRegion) regionIndex;

                foreach (var (rankIndex, roleData) in rankRoleData)
                {
                    var rank = (UggRank) rankIndex;

                    foreach (var (roleIndex, itemData) in roleData)
                    {
                        var role = (UggRole) roleIndex;

                        var uggItemDataByOrder = ParseItem(itemData);

                        for (var i = 0; i < uggItemDataByOrder.Count; i++)
                        {
                            if (i == 1)
                                // TODO handle boots
                                continue;

                            var uggItemData = uggItemDataByOrder[i];

                            var order = i == 0 ? i : i - 1;

                            itemDataList.AddRange(uggItemData.Select(uggItem => new UggItemData
                            {
                                ChampionId = championId,
                                ItemId = uggItem.ItemId,
                                Matches = uggItem.Matches,
                                Wins = uggItem.Wins,
                                Order = order,
                                Rank = rank,
                                Region = region,
                                Role = role
                            }));
                        }
                    }
                }
            }

            return itemDataList;
        }

        private static List<List<UggSimpleItemData>> ParseItem(IEnumerable<List<JsonElement>> itemData)
        {
            var itemDataByOrder = itemData.Skip(1).ToList();

            var newItemDataByOrder = itemDataByOrder.Select(itemsByOrder => itemsByOrder.Select(itemInfo =>
                new UggSimpleItemData
                {
                    ItemId = itemInfo[0].GetInt32(),
                    Wins = itemInfo[1].GetInt32(),
                    Matches = itemInfo[2].GetInt32()
                }).ToList()).ToList();

            return newItemDataByOrder;
        }
    }
}