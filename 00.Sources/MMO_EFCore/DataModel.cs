using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // DB 관계 모델링
    // 1 : 1
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
        public Player Owner { get; set; }
    }

    // Entity 클래스 이름 = 테이블 이름 = Player
    [Table("Player")]
    public class Player
    {
        // 클래스명Id -> PK
        public int PlayerId { get; set; }
        public string Name { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
