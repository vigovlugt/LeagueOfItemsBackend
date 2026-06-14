using System.Collections.Generic;
using LeagueOfItems.Application.Lolalytics.Models;
using LeagueOfItems.Domain.Models.Common;

namespace LeagueOfItems.Application.Lolalytics.Services;

public static class LolalyticsRoleMapper
{
    public const string Top = "top";
    public const string Jungle = "jungle";
    public const string Middle = "middle";
    public const string Bottom = "bottom";
    public const string Support = "support";

    public static readonly List<string> Roles = new()
    {
        Top,
        Jungle,
        Middle,
        Bottom,
        Support
    };

    public static Role ToDomainRole(string role)
    {
        return role switch
        {
            Top => Role.Top,
            Jungle => Role.Jungle,
            Middle => Role.Mid,
            Bottom => Role.Adc,
            Support => Role.Supp,
            _ => Role.None
        };
    }

    public static List<string> GetRolesWithData(LolalyticsLanes lanes)
    {
        if (lanes == null)
        {
            return Roles;
        }

        var roles = new List<string>();
        AddRoleIfPresent(roles, Top, lanes.Top);
        AddRoleIfPresent(roles, Jungle, lanes.Jungle);
        AddRoleIfPresent(roles, Middle, lanes.Middle);
        AddRoleIfPresent(roles, Bottom, lanes.Bottom);
        AddRoleIfPresent(roles, Support, lanes.Support);

        return roles.Count == 0 ? Roles : roles;
    }

    private static void AddRoleIfPresent(List<string> roles, string role, double presence)
    {
        if (presence > 0)
        {
            roles.Add(role);
        }
    }
}
