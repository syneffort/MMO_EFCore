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

        // Connected / Disconnected Update
        // Disconnected : 업데이트 단계가 끊기면서 실행됨 (Rest API 등)
        // 처리하는 2가지 방법
        // 1) Reload 방식,필요한 정보만 보내서 단계별 처리
        // 2) Full Update 방식, 모든 정보를 다 보내고 받아서 전체를 업데이트

        public static void ShowGuilds()
        {
            using (AppDbContext db = new AppDbContext())
            {
                foreach (GuildDto guild in db.Guilds.MapGuildToDto())
                {
                    Console.WriteLine($"GuildId({guild.GuildId}) GuildName({guild.Name}) MemberCount({guild.MemberCount})");
                }
            };
        }

        // 장점 : 최소한의 정보로 업데이트
        // 단점 : 불러오기를 2번 수행해야 함
        public static void UpdateByReload()
        {
            ShowGuilds();

            // 외부에서 수정을 원하는 정보를 넘겨줌
            Console.WriteLine("Input GuildId");
            Console.Write("> ");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("Input GuildName");
            Console.Write("> ");
            string name = Console.ReadLine();

            using (AppDbContext db = new AppDbContext())
            {
                Guild guild = db.Find<Guild>(id);
                guild.GuildName = name;
                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ---");
            ShowGuilds();
        }

        // 장점 : 다시 불러오지 않고 바로 업데이트 가능
        // 단점 : 모든 정보가 필요하고 보안 이슈 발생 가능
        public static string MakeUpdateJsonStr()
        {
            var jsonStr = "{\"GuildId\":1, \"GuildName\":\"Hello\", \"Members\":null}";
            return jsonStr;
        }

        public static void UpdateByFull()
        {
            ShowGuilds();

            //string jsonStr = MakeUpdateJsonStr();
            //Guild guild = JsonConvert.DeserializeObject<Guild>(jsonStr);

            Guild guild = new Guild()
            {
                GuildId = 1,
                GuildName = "TestGuild",
                Members = null
            };

            using (AppDbContext db = new AppDbContext())
            {
                db.Guilds.Update(guild);
                db.SaveChanges();
            }

            Console.WriteLine("--- Update Complete ---");
            ShowGuilds();
        }
    }
}
