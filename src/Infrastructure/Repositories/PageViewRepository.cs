using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeagueOfItems.Application.Common.Interfaces;
using LeagueOfItems.Domain.Models.PageViews;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace LeagueOfItems.Infrastructure.Repositories;

public class PageViewRepository : IPageViewRepository
{
    private readonly IMongoCollection<PageView> _pageViewsCollection;

    public PageViewRepository(
        IConfiguration config)
    {
        var mongoClient = new MongoClient(
            config["MongoDb:ConnectionString"]);

        var mongoDatabase = mongoClient.GetDatabase(config["MongoDb:DatabaseName"]);

        _pageViewsCollection = mongoDatabase.GetCollection<PageView>(config["MongoDb:PageViewsCollection"]);
    }

    public async Task<PageViewDataset> GetDatasetAsync()
    {
        var result = await _pageViewsCollection.Aggregate().Group(v => new {v.Type, v.Id},
            v => new {v.Key, Count = v.Count()}).ToListAsync();

        var pageViewResults = result.Select(v => new PageViewData
        {
            Id = v.Key.Id,
            Type = v.Key.Type,
            Count = v.Count
        }).OrderByDescending(p => p.Count).ToList();

        return new PageViewDataset(pageViewResults);
    }

    public async Task<int> DeleteOldAsync()
    {
        var result =  await _pageViewsCollection.DeleteManyAsync(v => v.CreatedAt < DateTime.Now.AddDays(-7));

        return (int)result.DeletedCount;
    }
}