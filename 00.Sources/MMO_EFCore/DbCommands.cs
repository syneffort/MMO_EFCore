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
    // 0) Detached : Notracking 상태, 추적되지 않아 Savechange 해도 영향 djqtdma
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

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

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
                    CreateDate = DateTime.Now,
                    Owner = synk
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = faker
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
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

            //Console.WriteLine(db.Entry(synk).State); // Added

            db.SaveChanges();
        }

        // Q) Dependent 데이터가 Principal 데이터 없이 존재 할 수 있을까?
        //  1> 주인이 없는 아이템은 불가능 : FK로 참조하고 있는 데이터도 삭제됨
        //  2> 주인이 없는 아이템이 가능 : FK로 참조하고 있는 데이터 유지됨
        // -> 2가지 케이스를 어떻게 구분해서 설정? => Nullable
        // FK 를 그냥 int로 설정시 1> 케이스, Nullable 설정 시 2> 케이스

        public static void ShowItems()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (var item in db.Items.Include(i => i.Owner).ToList())
                {
                    if (item.Owner == null)
                        Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner(0)");
                    else
                        Console.WriteLine($"ItemId({item.ItemId}) TemplateId({item.TemplateId}) Owner({item.Owner.PlayerId}, {item.Owner.Name})");
                }
            }
        }

        public static void Test()
        {
            ShowItems();

            Console.WriteLine("Input delete player id");
            Console.Write(" > ");
            int id = int.Parse(Console.ReadLine());

            using (AppDbContext db = new AppDbContext())
            {
                Player player = db.Players
                    .Include(p => p.Item)
                    .Single(p => p.PlayerId == id);

                db.Players.Remove(player);
                db.SaveChanges();
            }

            Console.WriteLine("--- Test complete ---");

            ShowItems();
        }
    }
}
