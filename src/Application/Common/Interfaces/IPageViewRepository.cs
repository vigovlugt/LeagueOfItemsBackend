using System.Collections.Generic;
using System.Threading.Tasks;
using LeagueOfItems.Domain.Models.PageViews;

namespace LeagueOfItems.Application.Common.Interfaces;

public interface IPageViewRepository
{
    Task<PageViewDataset> GetDatasetAsync();
    Task<int> DeleteOldAsync();
}