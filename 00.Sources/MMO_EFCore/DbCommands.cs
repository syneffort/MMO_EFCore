using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MMO_EFCore
{
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
            Player player = new Player()
            {
                Name = "SynK"
            };

            List<Item> Items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 102,
                    CreateDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                    TemplateId = 103,
                    CreateDate = DateTime.Now,
                    Owner = new Player() { Name = "Faker" }
                },
            };

            db.Items.AddRange(Items);
            db.SaveChanges();
        }

        public static void ReadAll()
        {
            using (AppDbContext db = new AppDbContext())
            {
                // AsNoTracking : ReadOnly => Tracking Snapshot 이라는 데이터 변경 탐지기능 때문
                // Include : Eager Loading (즉시 로딩)
                foreach (Item item in db.Items.AsNoTracking().Include(i => i.Owner))
                {
                    Console.WriteLine($"TempalteId({item.TemplateId}) Owner({item.Owner.Name}) Created({item.CreateDate})");
                }
            };
        }

        // 특정 플레이어가 소지한 아이템들의 CreatedDate 수정
        public static void UpdateDate()
        {
            Console.WriteLine("Input Player Name");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                            .Where(i => i.Owner.Name == name);

                foreach (Item item in items)
                {
                    item.CreateDate = DateTime.Now;
                }

                db.SaveChanges();
            }

            ReadAll();
        }

        public static void DeleteItem()
        {
            Console.WriteLine("Input Player Name");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                            .Where(i => i.Owner.Name == name);

                db.Items.RemoveRange(items);

                db.SaveChanges();
            }

            ReadAll();
        }
    }
}
