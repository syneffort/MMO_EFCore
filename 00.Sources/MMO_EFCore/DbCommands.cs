using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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

        // Update 3단계
        // 1) Tracked entity 가져오기
        // 2) Entity 클래스의 프로퍼티 변경 (set)
        // 3) Savechanges 호출 --> DetectChang에서 최초 Sanpshot / 현재 Snapshot 비교해서 변경사항만 저장

        public static void UpdateTest()
        {
            using (AppDbContext db = new AppDbContext())
            {
                Guild guild = db.Guilds.Single(g => g.GuildName == "T1");

                guild.GuildName = "DWG";

                db.SaveChanges();
            }
        }
    }
}
