using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeagueOfItems.Application.Ugg.Services;
using LeagueOfItems.Domain.Models.Items;
using LeagueOfItems.Domain.Models.Ugg;

namespace LeagueOfItems.Application.Items.Services;

public static class UggItemDataParser
{
    private const int BootsIndex = 1;

    public static async Task<List<ItemData>> Parse(
        int championId,
        Stream stream,
        string version)
    {
        var parsed = await UggResponseParser.Parse<List<List<JsonElement>>, List<ItemData>>(stream,
            (region, rank, role, data) =>
            {
                var uggItemDataByOrder = ParseItem(data);

                var itemDataList = new List<ItemData>();

                for (var i = 0; i < uggItemDataByOrder.Count; i++)
                {
                    var uggItemData = uggItemDataByOrder[i];
                    var slot = GetSlot(i);
                    var order = GetOrder(i, slot);

                    itemDataList.AddRange(uggItemData.Select(uggItem => new ItemData
                    {
                        ChampionId = championId,
                        ItemId = uggItem.ItemId,
                        Matches = uggItem.Matches,
                        Wins = uggItem.Wins,
                        Order = order,
                        Slot = slot,
                        Rank = rank,
                        Region = region,
                        Role = role,
                        Patch = version
                    }));
                }

                return itemDataList;
            });

        return parsed.SelectMany(x => x).ToList();
    }

    private static ItemSlot GetSlot(int groupIndex)
    {
        return groupIndex == BootsIndex ? ItemSlot.Boots : ItemSlot.Core;
    }

    private static int GetOrder(int groupIndex, ItemSlot slot)
    {
        if (slot == ItemSlot.Boots)
        {
            return 0;
        }

        return groupIndex < BootsIndex ? groupIndex : groupIndex - 1;
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