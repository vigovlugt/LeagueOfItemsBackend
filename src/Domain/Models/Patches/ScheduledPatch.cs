using System;

namespace LeagueOfItems.Domain.Models.Patches;

public record ScheduledPatch(string Patch, DateTime ScheduledDate);