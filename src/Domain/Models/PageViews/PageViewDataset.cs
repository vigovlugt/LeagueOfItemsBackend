using System.Collections.Generic;
using System.Linq;

namespace LeagueOfItems.Domain.Models.PageViews;

public class PageViewDataset
{
    public List<PageViewData> Items { get; set; }
    public List<PageViewData> Runes { get; set; }
    public List<PageViewData> Champions { get; set; }

    public PageViewDataset(List<PageViewData> pageViewData)
    {
        foreach (var grouping in pageViewData.GroupBy(v => v.Type))
        {
            switch (grouping.Key)
            {
                case "ITEM":
                    Items = grouping.Take(10).ToList();
                    break;
                case "RUNE":
                    Runes = grouping.Take(10).ToList();
                    break;
                case "CHAMPION":
                    Champions = grouping.Take(10).ToList();
                    break;
            }
        }
    }
}