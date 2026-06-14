using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace LeagueOfItems.Application.Lolalytics.Services;

public static class LolalyticsQwikParser
{
    private static readonly Regex QwikJsonRegex = new(
        "<script\\s+type=[\"']qwik/json[\"'][^>]*>([\\s\\S]*?)</script>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Parse<T>(string html)
    {
        var data = ExtractData(html);

        return data.Deserialize<T>(SerializerOptions);
    }

    private static JsonNode ExtractData(string html)
    {
        var match = QwikJsonRegex.Match(html);
        if (!match.Success)
        {
            throw new InvalidOperationException("Lolalytics Qwik JSON payload was not found.");
        }

        using var document = JsonDocument.Parse(match.Groups[1].Value);
        var objs = document.RootElement.GetProperty("objs").EnumerateArray()
            .Select(element => element.Clone())
            .ToList();

        var dataIndex = GetDataIndex(objs);
        if (dataIndex == null)
        {
            throw new InvalidOperationException("Lolalytics champion data object was not found.");
        }

        return ParseReference(dataIndex.Value, objs, new HashSet<int>());
    }

    private static int? GetDataIndex(IReadOnlyList<JsonElement> objs)
    {
        for (var i = 0; i < objs.Count; i++)
        {
            if (objs[i].ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            if (objs[i].TryGetProperty("analysed", out _) &&
                objs[i].TryGetProperty("avgWr", out _) &&
                objs[i].TryGetProperty("enemy", out _))
            {
                return i;
            }
        }

        return null;
    }

    private static JsonNode ParseElement(
        JsonElement element,
        IReadOnlyList<JsonElement> objs,
        HashSet<int> referenceStack)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var obj = new JsonObject();
                foreach (var property in element.EnumerateObject())
                {
                    obj[property.Name] = ParseElement(property.Value, objs, referenceStack);
                }

                return obj;
            case JsonValueKind.Array:
                var array = new JsonArray();
                foreach (var item in element.EnumerateArray())
                {
                    array.Add(ParseElement(item, objs, referenceStack));
                }

                return array;
            case JsonValueKind.String:
                var value = element.GetString();
                var referenceIndex = TryParseBase36(value);
                if (referenceIndex != null && referenceIndex.Value >= 0 && referenceIndex.Value < objs.Count)
                {
                    return ParseReference(referenceIndex.Value, objs, referenceStack);
                }

                return JsonValue.Create(value);
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return JsonNode.Parse(element.GetRawText());
        }
    }

    private static JsonNode ParseReference(
        int referenceIndex,
        IReadOnlyList<JsonElement> objs,
        HashSet<int> referenceStack)
    {
        if (!referenceStack.Add(referenceIndex))
        {
            return null;
        }

        var node = ParseElement(objs[referenceIndex], objs, referenceStack);
        referenceStack.Remove(referenceIndex);

        return node;
    }

    private static int? TryParseBase36(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var result = 0;
        foreach (var character in value.ToLowerInvariant())
        {
            int digit;
            if (character is >= '0' and <= '9')
            {
                digit = character - '0';
            }
            else if (character is >= 'a' and <= 'z')
            {
                digit = character - 'a' + 10;
            }
            else
            {
                return null;
            }

            try
            {
                result = checked(result * 36 + digit);
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        return result;
    }
}
