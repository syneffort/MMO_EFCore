﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMO_EFCore
{
    public static class Extensions
    {
        // IEnumerable : LINQ to Object / LINQ to XML
        // IQueryable : LINQ to SQL

        public static IQueryable<GuildDto> MapGuildToDto(this IQueryable<Guild> guild)
        {
            return guild.Select(g => new GuildDto()
            {
                GuildId = g.GuildId,
                Name = g.GuildName,
                MemberCount = g.Members.Count
            });
        }
    }
}
