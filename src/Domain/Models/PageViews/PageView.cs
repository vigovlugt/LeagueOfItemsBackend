using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LeagueOfItems.Domain.Models.PageViews;

public class PageView
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string PageViewId { get; set; }
    
    [BsonElement("id")]
    public int Id { get; set; }
    
    [BsonElement("type")]
    public string Type { get; set; }
    
    [BsonElement("user")]
    public string User { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [BsonElement("__v")]
    public int Version { get; set; }
}