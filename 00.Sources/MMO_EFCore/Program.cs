using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace MMO_EFCore
{
    class Program
    {
        // 초기화는 시간 소요 됨
        static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                Console.WriteLine("DB was Initialized");
            };
        }

        static void Main(string[] args)
        {
            InitializeDB(forceReset: true);
        }
    }
}
