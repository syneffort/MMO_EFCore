using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // DB 관계 모델링
    // 1 : 1 => Foreign Key를 만들 쪽에 명시적으로 인덱스 프로퍼티 구현 또는 어트리뷰트 선언 필요함
    // 1 : 다
    // 다 : 다

    // Entity 클래스 이름 = 테이블 이름 = Item
    // 별도 테이블 이름 지정 시 attribute 지정
    [Table("Item")]
    public class Item
    {
        // PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; }
        public DateTime CreateDate { get; set; }

        // 다른 클래스를 참조 -> FK (Navigational Property)
        //public int OwnerId { get; set; } // Convention 방식으로 플레이어 객체와 연동
        [ForeignKey("OnerId")]
        public Player Owner { get; set; }
    }

    // Entity 클래스 이름 = 테이블 이름 = Player
    [Table("Player")]
    public class Player
    {
        // 클래스명Id -> PK
        public int PlayerId { get; set; }
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
