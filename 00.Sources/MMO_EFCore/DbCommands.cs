using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                new Item()
                {
                    TemplateId = 102,
                    Owner = faker
                },
                new Item()
                {
                    TemplateId = 103,
                    Owner = deft
                },
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

            // Add Test
            {
                Item item = new Item()
                {
                    TemplateId = 500,
                    Owner = synk
                };

                db.Items.Add(item); // 간접적으로 Player에 영향을 줌
                // Player는 Tracking 상태이고, FK는 없으므로 현 상태 유지 (Unchanged)
                Console.WriteLine($"Sate2 : {db.Entry(synk).State}"); // Unchanged
            }

            // Delete Test
            {
                Player p = db.Players.First();

                // 아직 DB에 등록되지 않은 정보 (DB 키 없음)
                p.Guild = new Guild() { GuildName = "곧 사라질 길드" };
                // 이미 Item이 등록되어 DB 발급키 존재함
                p.OwnedItem = Items[0];

                db.Players.Remove(p);
                Console.WriteLine($"State3 : {db.Entry(p).State}"); // Deleted
                Console.WriteLine($"State4 : {db.Entry(p.Guild).State}"); // Added
                Console.WriteLine($"State5 : {db.Entry(p.OwnedItem).State}"); // Deleted (Player가 삭제되기 때문)
            }
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

        public static void CalcAverage()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (double? average in db.Items.Select(i => Program.GetAverageReviewScore(i.ItemId)))
                {
                    if (average == null)
                        Console.WriteLine("No review");
                    else
                        Console.WriteLine($"Average({average.Value})");
                }
            }
        }
    }
}
