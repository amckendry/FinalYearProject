using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGProjectLibrary;

namespace RPGProject
{
    public class PartyCharacter
    {
        CharacterIdentity characterID;
        public CharacterIdentity CharacterID
        {
            get
            {
                return characterID;
            }
        }

        int level;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {

            }
        }
        int expToNext;
        public int ExpToNext
        {
            get
            {
                return expToNext;
            }
            set
            {
                expToNext = value;
            }
        }
        int pointsToSpend;
        public int PointsToSpend
        {
            get
            {
                return pointsToSpend;
            }
            set
            {
                pointsToSpend = value;
            }
        }

        protected Texture2D portraitTexture;
        public Texture2D PortraitTexture
        {
            get
            {
                return portraitTexture;
            }
        }

        protected int hp;
        public int HP
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
            }
        }
        protected int mp;
        public int MP
        {
            get
            {
                return mp;
            }
            set
            {
                mp = value;
            }
        }
        protected int atk;
        public int ATK
        {
            get
            {
                return atk;
            }
            set
            {
                atk = value;
            }

        }
        protected int def;
        public int DEF
        {
            get
            {
                return def;
            }
            set
            {
                def = value;
            }

        }
        protected int mag_atk;
        public int MAG_ATK
        {
            get
            {
                return mag_atk;
            }
            set
            {
                mag_atk = value;
            }

        }
        protected int mag_def;
        public int MAG_DEF
        {
            get
            {
                return mag_def;
            }
            set
            {
                mag_def = value;
            }
        }
        protected int spd;
        public int SPD
        {
            get
            {
                return spd;
            }
            set
            {
                spd = value;
            }
        }

        protected int hpGrowth;
        public int HPGrowth
        {
            get
            {
                return hpGrowth;
            }
        }
        protected int mpGrowth;
        public int MPGrowth
        {
            get
            {
                return mpGrowth;
            }
        }
        protected int atkGrowth;
        public int ATKGrowth
        {
            get
            {
                return atkGrowth;
            }
        }
        protected int defGrowth;
        public int DEFGrowth
        {
            get
            {
                return defGrowth;
            }
        }
        protected int mag_atkGrowth;
        public int MAG_ATKGrowth
        {
            get
            {
                return mag_atkGrowth;
            }
        }
        protected int mag_defGrowth;
        public int MAG_DEFGrowth
        {
            get
            {
                return mag_defGrowth;
            }
        }
        protected int spdGrowth;
        public int SPDGrowth
        {
            get
            {
                return spdGrowth;
            }
        }

        protected List<BattleTechniqueLevelData> techniqueLevelData;
        public List<BattleTechniqueLevelData> TechniqueLevelData
        {
            get
            {
                return techniqueLevelData;
            }
        }

        public PartyCharacter()
        {
        } 
        public void LoadContent(PlayerStatsData growthStats, int level, int expToNext, int pointsToSpend, int HP, int MP, int ATK, int DEF, int MAG_ATK, int MAG_DEF, int SPD)
        {
            this.characterID = growthStats.characterID;
            portraitTexture = GameClass.LoadTextureData(growthStats.portraitTextureFileName);
            this.level = level;
            this.expToNext = expToNext;
            this.pointsToSpend = pointsToSpend;
            hp = HP;
            mp = MP;
            atk = ATK;
            def = DEF;
            mag_atk = MAG_ATK;
            mag_def = MAG_DEF;
            spd = SPD;
            hpGrowth = growthStats.HPGrowth;
            mpGrowth = growthStats.MPGrowth;
            atkGrowth = growthStats.ATKGrowth;
            defGrowth = growthStats.DEFGrowth;
            mag_atkGrowth = growthStats.MAG_ATKGrowth;
            mag_defGrowth = growthStats.MAG_DEFGrowth;
            spdGrowth = growthStats.SPDGrowth;

            techniqueLevelData = growthStats.techniqueLevelData;
        }
        public void LoadContent(PlayerStatsData initialStats)
        {
            this.characterID = initialStats.characterID;
            portraitTexture = GameClass.LoadTextureData(initialStats.portraitTextureFileName);
            this.level = initialStats.initialLevel;
            this.expToNext = level * 5;
            this.pointsToSpend = 0;

            hp = initialStats.initialHP;
            mp = initialStats.initialMP;
            atk = initialStats.initialATK;
            def = initialStats.initialDEF;
            mag_atk = initialStats.initialMAG_ATK;
            mag_def = initialStats.initialMAG_DEF;
            spd = initialStats.initialSPD;

            hpGrowth = initialStats.HPGrowth;
            mpGrowth = initialStats.MPGrowth;
            atkGrowth = initialStats.ATKGrowth;
            defGrowth = initialStats.DEFGrowth;
            mag_atkGrowth = initialStats.MAG_ATKGrowth;
            mag_defGrowth = initialStats.MAG_DEFGrowth;
            spdGrowth = initialStats.SPDGrowth;

            techniqueLevelData = initialStats.techniqueLevelData;
        }
        public void LevelUp()
        {
            if (level < GlobalGameInfo.LevelLimit)
            {
                level += 1;
                pointsToSpend += 3;
                hp += hpGrowth;
                mp += mpGrowth;
                atk += atkGrowth;
                def += defGrowth;
                mag_atk += mag_atkGrowth;
                mag_def += mag_defGrowth;
                spd += spdGrowth;
                expToNext = 5 * level;
            }

            if (hp > GlobalGameInfo.StatLimit)
            {
                hp = GlobalGameInfo.StatLimit;
            }
            if (mp > GlobalGameInfo.StatLimit)
            {
                mp = GlobalGameInfo.StatLimit;
            }
            if (atk > GlobalGameInfo.StatLimit)
            {
                atk = GlobalGameInfo.StatLimit;
            }
            if (def > GlobalGameInfo.StatLimit)
            {
                def = GlobalGameInfo.StatLimit;
            }
            if (mag_atk > GlobalGameInfo.StatLimit)
            {
                mag_atk = GlobalGameInfo.StatLimit;
            }
            if (mag_def > GlobalGameInfo.StatLimit)
            {
                mag_def = GlobalGameInfo.StatLimit;
            }
            if (spd > GlobalGameInfo.StatLimit)
            {
                spd = GlobalGameInfo.StatLimit;
            }
        }
    }
}
