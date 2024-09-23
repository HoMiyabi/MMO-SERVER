﻿using FreeSql.DataAnnotations;

namespace GameServer.Database
{
    /// <summary>
    /// 玩家信息
    /// </summary>
    [Table(Name = "player")]
    public class DbPlayer
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Coin { get; set; }
    }

    /// <summary>
    /// 玩家的角色
    /// </summary>
    [Table(Name = "character")]
    public class DbCharacter
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; } = 100;
        public int Mp { get; set; } = 100;
        public int Level { get; set; } = 1;
        public int Exp { get; set; } = 0;
        public int SpaceId { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public long Gold { get; set; } = 0;
        public int PlayerId { get; set; }
    }
}