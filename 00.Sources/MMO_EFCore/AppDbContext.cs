using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMO_EFCore
{
    // EF Core 작동 스텝
    // 1) DbContext 만들 때
    // 2) DbSet<T> 찾는다
    // 3) 모델링 클래스 분석하여 컬럼 찾는다
    // 4) 모델링 클래스 내 다른 참조하는 클래스가 있다면 분석한다
    // 5) OnModelCreating 함수 호출 (추가 설정 = override)
    // 6) 데이터베이스의 전체 모델링 구조를 내부 메모리에 로딩

    public class AppDbContext : DbContext
    {
        // DbSet<T> -> EF Core 에게 테이블 정보 알림
        // 여기에 선언된 프로퍼티 이름이 우선하여 테이블 이름으로 지정됨
        public DbSet<Item> Items { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Guild> Guilds { get; set; }

        public const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EfCoreDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // 앞으로 Item entity에 접근할 때 항상 사용되는 모델 레벨의 필터링 구현
            // 필터를 무시하고 싶으면 IgnoreQueryFilter 사용하면 됨
            builder.Entity<Item>().HasQueryFilter(i => i.SoftDeleted == false);

            builder.Entity<Player>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("Index_Person_Name")
                .IsUnique();

            builder.Entity<Player>()
                .HasMany(p => p.CreatedItems)
                .WithOne(i => i.Creator)
                .HasForeignKey(i => i.CreatorId);

            builder.Entity<Player>()
                .HasOne(p => p.OwnedItem)
                .WithOne(i => i.Owner)
                .HasForeignKey<Item>(i => i.OwnerId);
        }
    }
}
