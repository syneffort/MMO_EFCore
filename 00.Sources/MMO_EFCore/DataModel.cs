using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    // 클래스 이름 = 테이블 이름 = Item
    // 별도 테이블 이름 지정 시 attribute 지정
    [Table("Item")]
    public class Item
    {
        // PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; }
        public DateTime CreateDate { get; set; }

        // 다른 클래스를 참조 -> FK (Navigational Property)
        public int OwnerId { get; set; }
        public Player Owner { get; set; }
    }

    // 클래스 이름 = 테이블 이름 = Player
    public class Player
    {
        // 클래스명Id -> PK
        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}
