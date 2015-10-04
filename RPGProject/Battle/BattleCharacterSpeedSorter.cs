using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGProject
{
    public class BattleCharacterSpeedSorter : IComparer<BattleCharacter>
    {//This is a comparison class for ordering a list of battle characters with respect to their speed stat.
     //This is used by the BattleScreen to calculate the turn order each turn of the battle.
        #region Methods
        public int Compare(BattleCharacter character1, BattleCharacter character2)
        {
            if(character1.CurrentSPD > character2.CurrentSPD)
            {
                return -1;
            }
            else if (character1.CurrentSPD < character2.CurrentSPD)
            {
                return 1;
            }
            else
            {
                if (character1.BattleOrder < character2.BattleOrder)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
        #endregion
    }
}
