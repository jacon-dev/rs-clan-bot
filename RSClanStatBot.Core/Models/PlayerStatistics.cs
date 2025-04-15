using System.Collections.Generic;

namespace RSClanStatBot.Core.Models
{
    public class PlayerStatistics
    {
        public long Magic { get; set; }
        
        public int QuestsStarted { get; set; }
        
        public int TotalKills { get; set; }
        
        public int QuestsComplete { get; set; }
        
        public int QuestsNotStarted { get; set; }
        
        public long TotalExp { get; set; }
        
        public long Ranged { get; set; }
        
        public List<PlayerActivity> Activities { get; set; }
        
        public List<PlayerSkill> SkillValues { get; set; }
        
        public string Name { get; set; }
        
        public string Rank { get; set; }
        
        public long Melee { get; set; }
        
        public int CombatLevel { get; set; }
        
        public bool LoggedIn { get; set; }
    }
}