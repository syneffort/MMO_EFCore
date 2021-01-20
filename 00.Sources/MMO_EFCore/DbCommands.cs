using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace MMO_EFCore
{
    // Entity State
    // 0) Detached : Notracking 상태, 추적되지 않아 Savechange 해도 영향 X
    // 1) Unchanged : DB에는 있으나 수정 사항은 없음, Savechange 해도 영향 X
    // 2) Deleted : DB에는 있으나 삭제되어야 함, Savechange 시 적용
    // 3) Modified : DB에 있고 수정이 발생함, Savechange 시 적용
    // 4) Added : DB에는 아직 없음, Savechange 시 적용

    public class DbCommands
    {
        // 초기화는 시간 소요 됨
        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                // db.Database.Migrate();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                string command =
                    @"  CREATE FUNCTION GetAverageReviewScore (@itemId INT) RETURNS FLOAT
                        AS
                        BEGIN

                        DECLARE @result AS FLOAT

                        SELECT @result = AVG(CAST([Score] AS FLOAT))
                        FROM ItemReview AS r
                        WHERE @itemId = r.ItemId

                        RETURN @result

                        END";

                db.Database.ExecuteSqlRaw(command);

                CreateTestData(db);
                Console.WriteLine("DB was Initialized");
            };
        }
        
        public static void CreateTestData(AppDbContext db)
        {
            Player synk = new Player() { Name = "SynK" };
            Player faker = new Player() { Name = "Faker" };
            Player deft = new Player() { Name = "Deft" };

            //Console.WriteLine(db.Entry(synk).State); // Detached

            List<Item> Items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    Owner = synk
                },
                //new Item()
                //{
                //    TemplateId = 102,
                //    Owner = faker
                //},
                //new Item()
                //{
                //    TemplateId = 103,
                //    Owner = deft
                //},
            };
            Guild guild = new Guild()
            {
                GuildName = "T1",
                Members = new List<Player>() { synk, faker, deft }
            };

            db.Items.AddRange(Items);
            db.Guilds.Add(guild);

            // Added
            Console.WriteLine($"Sate1 : {db.Entry(synk).State}"); // Added

            db.SaveChanges();
        }

        public static void ShowItems()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.Items.Include(i => i.Owner).IgnoreQueryFilters().ToList())
                {
                   if (item.SoftDeleted)
                    {
                        Console.WriteLine($"\tDELETED > ItemId({item.ItemId}) TemplateId({item.TemplateId})");
                    }
                   else
                    {
                        if (item.Owner == null)
                            Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                        else
                            Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner({item.Owner.PlayerId}, {item.Owner.Name})");
                    }
                }
            }
        }

        public static void ShowGuild()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.Include(g => g.Members).ToList())
                {
                    Console.WriteLine($"GuildId({guild.GuildId}) GuildName({guild.GuildName}) MemberCount({guild.Members.Count})");
                }
            }
        }

        public static void Test()
        {
            using (AppDbContext db = new AppDbContext())
            {
                // State 조작
                {
                    Player p = new Player() { Name = "StateTest" };
                    db.Entry(p).State = EntityState.Added; // Tracked로 변환
                    db.SaveChanges();
                }

                // TrackGraph
                {
                    // Disconnect 상태에서 플레이어 이름만 갱신하려고 함
                    Player p = new Player()
                    {
                        PlayerId = 2,
                        Name = "Track_Faker"
                    };

                    p.OwnedItem = new Item() { ItemId = 777 };
                    p.Guild = new Guild() { GuildName = "Track_Guild" };

                    db.ChangeTracker.TrackGraph(p, e =>
                    {
                        if (e.Entry.Entity is Player)
                        {
                            e.Entry.State = EntityState.Unchanged;
                            e.Entry.Property("Name").IsModified = true;
                        }
                        else if (e.Entry.Entity is Guild)
                        {
                            e.Entry.State = EntityState.Unchanged;
                        }
                        else if (e.Entry.Entity is Item)
                        {
                            e.Entry.State = EntityState.Unchanged;
                        }
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}
