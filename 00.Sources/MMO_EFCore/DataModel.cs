using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MMO_EFCore
{
    public struct ItemOption
    {
        public int str;
        public int dex;
        public int hp;
    }

    // Entity 클래스 이름 = 테이블 이름 = Item
    // 별도 테이블 이름 지정 시 attribute 지정
    [Table("Item")]
    public class Item
    {
        private string _jsonData;
        public string JsonData
        { 
            get { return _jsonData; }
            //set { _jsonData = value; } 
        }

        public void SetOption(ItemOption option)
        {
            _jsonData = JsonConvert.SerializeObject(option);
        }

        public ItemOption GetOption()
        {
            return JsonConvert.DeserializeObject<ItemOption>(_jsonData);
        }

        public bool SoftDeleted { get; set; }

        // PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; }
        public DateTime CreateDate { get; set; }

        // 다른 클래스를 참조 -> FK (Navigational Property)
        //public int OwnerId { get; set; } // Convention 방식으로 플레이어 객체와 연동
        public int OwnerId { get; set; }
        
        public Player Owner { get; set; }

        public int? CreatorId { get; set; }
        public Player Creator { get; set; }
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

        //[InverseProperty("Owner")]
        public Item OwnedItem { get; set; }
        //[InverseProperty("Creator")]
        public ICollection<Item> CreatedItems { get; set; }

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
