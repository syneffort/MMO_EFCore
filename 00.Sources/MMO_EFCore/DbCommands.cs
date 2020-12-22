using System;
using System.Collections.Generic;
using System.Linq;
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

        // Entity State
        // 0) Detached : Notracking 상태, 추적되지 않아 Savechange 해도 영향 djqtdma
        // 1) Unchanged : DB에는 있으나 수정 사항은 없음, Savechange 해도 영향 X
        // 2) Deleted : DB에는 있으나 삭제되어야 함, Savechange 시 적용
        // 3) Modified : DB에 있고 수정이 발생함, Savechange 시 적용
        // 4) Added : DB에는 아직 없음, Savechange 시 적용

        // SavaChanges 호출하면?
        // 1) 추가된 객체들의 상태가 Unchanged로 바뀜
        // 2) SQL Identity로 PK 관리
        // - 데이터 추가 후 ID 받아와서 객체의 ID 프로퍼티를 채워줌 (SaveChange 이 전에는 ID를 알 수 없음)
        // - Relationship 참고해서, FK 및 객체 참조 연결

        // 이미 존재하는 사용자를 연동하려면?
        // 1) Tracked Instance(추적되고 있는 객체)를 얻어옴
        // 2) 객체 연결

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

            {
                Player owner = db.Players.Where(p => p.Name == "SynK").First();
                Item item = new Item()
                {
                    TemplateId = 300,
                    CreateDate = DateTime.Now,
                    Owner = owner
                };
                db.Items.Add(item);

                db.SaveChanges();
            }

            //Console.WriteLine(db.Entry(synk).State); // Unchanged
        }
    }
}
