using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Items.Services
{
    public static class UggItemDataParser
    {
        public static async Task<List<ItemData>> Parse(
            int championId,
            Stream stream)
        {
            var parsed = await UggResponseParser.Parse<List<List<JsonElement>>, List<ItemData>>(stream,
                (region, rank, role, data) =>
                {
                    var uggItemDataByOrder = ParseItem(data);

                    var itemDataList = new List<ItemData>();

                    for (var i = 0; i < uggItemDataByOrder.Count; i++)
                    {
                        if (i == 1)
                            // TODO handle boots
                            continue;

                        var uggItemData = uggItemDataByOrder[i];

                        var order = i == 0 ? i : i - 1;

                        itemDataList.AddRange(uggItemData.Select(uggItem => new ItemData
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

                    return itemDataList;
                });

            return parsed.SelectMany(x => x).ToList();
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