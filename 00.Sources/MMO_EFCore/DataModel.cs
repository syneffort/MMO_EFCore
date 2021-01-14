using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // Configuration
    // A) Convention (관례)
    //   - 규칙에 맞게 작성하면 EF에서 알아서 처리. 쉽고 빠르지만 모든 경우를 처리할 수 없음
    // B) Data Annotation (데이터 주석)
    //   - 클래스나 프로퍼티에 Attribute 작성하여 추가정보 입력
    // C) Fluent Api (직접정의)
    //   - OnModelCreating에서 직접 설정을 정의하는 '귀찮은' 방식. 활용 범위가 가장 넓음

    // -------- CONVENTION -----------
    // 1) Entity class 관련
    //   - public & non-static
    //   - property는 public getter
    //   - property명 = table column명
    // 2) 이름, 형식, 크기 관련
    //   - .NET 형식 <-> SQL 형식
    //   - .NET 형식의 nullable 여부를 따라감
    // 3) PK 관련
    //   - Id 혹은 클래스명Id 형태의 property는 PK로 간주 (클래스명Id 권장)
    //   - 복합키(Composite key)는 convention으로 처리 불가

    // Q1) DB column type, size, nullable
    // Nullable [Required]          .IsRequired()
    // size     [MaxLength(20)]     .HasMaxLenth(20)
    // 문자형식                       .IsUnicod(true)

    // Q2) PK
    // [Key]
    // 복합키의 경우
    // [Key]
    // [Column(Order = 0...)]
    // .HasKey(x => new { x.Prop1, x.Prop2 })

    // Q3) Index
    // 인덱스 추가   .HasIndex(p => p.Prop1)
    // 복합인덱스    .HasIndex(p => new { p.Prop1, p.Prop2 })
    // 인덱스 이름을 정해서 추가   .HasIndex(p => p.Prop1).HasName("Index_MyProp")
    // 유니크 인덱스  .HasIndex(p => p.Prop1).IsUnique()

    // Q4) 테이블 이름 지정
    // DBSet<T> property명 or calss명
    // [Table("MyTable")]   .ToTable("MyTable)

    // Q5) 컬럼명
    // property 이름
    // [Column("MyCol")]    .HasColumnName("MyCol")

    // Q6) 코드 모델링에서는 사용하나 DB모델링에서는 제외하고 싶다면 (class, property 모두)
    // [NotMapped]  .Ignore()

    // Q7) Soft delete
    // .HasQueryFilter()

    // Configuration 어떻게 처리?
    // 1) Convention 처리
    // 2) Validation 관련 부분은 Data annotation (직관적)
    // 3) 그 외 Fluent Api 처리
    // ※ convention, data annotation, fluent api 후자가 전자의 방식을 덮어씀

    // Entity 클래스 이름 = 테이블 이름 = Item
    // 별도 테이블 이름 지정 시 attribute 지정
    [Table("Item")]
    public class Item
    {
        public bool SoftDeleted { get; set; }

        // PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; }
        public DateTime CreateDate { get; set; }

        // 다른 클래스를 참조 -> FK (Navigational Property)
        //public int OwnerId { get; set; } // Convention 방식으로 플레이어 객체와 연동
        //[ForeignKey("OnerId")]
        public int? OwnerId { get; set; }
        public Player Owner { get; set; }
    }

    // Entity 클래스 이름 = 테이블 이름 = Player
    [Table("Player")]
    public class Player
    {
        // 클래스명Id -> PK
        public int PlayerId { get; set; } 
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public Item Item { get; set; }
        public Guild Guild { get; set; }
    }

    [Table("Guild")]
    public class Guild
    {
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Members { get; set; }
    }

    // DTO : Data Transfer Object
    public class GuildDto
    {
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
}
