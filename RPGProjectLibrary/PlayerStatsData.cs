using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace RPGProjectLibrary
{

    public struct PlayerStatsData
    {
        public CharacterIdentity characterID;
        public string portraitTextureFileName;
        public int initialLevel;
        public int initialHP;
        public int initialMP;
        public int initialATK;
        public int initialDEF;
        public int initialMAG_ATK;
        public int initialMAG_DEF;
        public int initialSPD;
        public int HPGrowth;
        public int MPGrowth;
        public int ATKGrowth;
        public int DEFGrowth;
        public int MAG_ATKGrowth;
        public int MAG_DEFGrowth;
        public int SPDGrowth;
        public List<BattleTechniqueLevelData> techniqueLevelData;
    }

    public class PlayerStatsDataReader : ContentTypeReader<PlayerStatsData>
    {
        protected override PlayerStatsData Read(ContentReader input, PlayerStatsData existingInstance)
        {
            PlayerStatsData data = new PlayerStatsData();
            data.characterID = input.ReadObject<CharacterIdentity>();
            data.portraitTextureFileName = input.ReadString();
            data.initialLevel = input.ReadInt32();
            data.initialHP = input.ReadInt32();
            data.initialMP = input.ReadInt32();
            data.initialATK = input.ReadInt32();
            data.initialDEF = input.ReadInt32();
            data.initialMAG_ATK = input.ReadInt32();
            data.initialMAG_DEF = input.ReadInt32();
            data.initialSPD = input.ReadInt32();
            data.HPGrowth = input.ReadInt32();
            data.MPGrowth = input.ReadInt32();
            data.ATKGrowth = input.ReadInt32();
            data.DEFGrowth = input.ReadInt32();
            data.MAG_ATKGrowth = input.ReadInt32();
            data.MAG_DEFGrowth = input.ReadInt32();
            data.SPDGrowth = input.ReadInt32();
            data.techniqueLevelData = input.ReadObject<List<BattleTechniqueLevelData>>();
            return data;
        }
    }

    public struct BattleTechniqueLevelData
    {
        public string techniqueName;
        public int levelLearnt;
    }
    public class BattleTechniqueLevelDataReader : ContentTypeReader<BattleTechniqueLevelData>
    {
        protected override BattleTechniqueLevelData Read(ContentReader input, BattleTechniqueLevelData existingInstance)
        {
            BattleTechniqueLevelData data = new BattleTechniqueLevelData();
            data.techniqueName = input.ReadString();
            data.levelLearnt = input.ReadInt32();
            return data;
        }
    }
}
