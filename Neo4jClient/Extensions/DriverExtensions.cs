﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4jClient.Extensions
{
    using System.Linq;
    using Neo4j.Driver;

    public static class DriverExtensions
    {

        public static IAsyncSession AsyncSession(this IDriver driver, Version databaseVersion, string database, bool isWrite, IEnumerable<string> bookmarks)
        {
            IEnumerable<Bookmarks> properBookmarks = null;
            if (bookmarks != null)
                properBookmarks = bookmarks.Select(b => Bookmarks.From(b));

            return driver.AsyncSession(databaseVersion, database, isWrite, properBookmarks);
        }

        public static IAsyncSession AsyncSession(this IDriver driver, Version databaseVersion, string database, bool isWrite, IEnumerable<Bookmarks> bookmarks) 
            => driver.AsyncSession(builder =>
                                                                                                                                                                           {
                                                                                                                                                                               if (databaseVersion.Major >= 4)
                                                                                                                                                                                   builder.WithDatabase(database);
                                                                                                                                                                               builder.WithDefaultAccessMode(isWrite ? AccessMode.Write : AccessMode.Read);
                                                                                                                                                                               if (bookmarks != null)
                                                                                                                                                                                   builder.WithBookmarks(bookmarks.ToArray());
                                                                                                                                                                           });
    }
}
