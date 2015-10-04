using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGProject
{
    public class BattleCharacterDrawPositionSorter : IComparer<BattleCharacter>
    {//This is a comparison class for calculating the order of draw calls to a list of battle characters.
     //This is so that the characters are drawn correctly in terms of layer depth. (i.e. the lowest Y position character last)
        #region Methods
        public int Compare(BattleCharacter character1, BattleCharacter character2)
        {
            if (character1 == character2)
            {
                return 0;
            }

            if ((character1.Position.Y + character1.YDrawOffset) < (character2.Position.Y + character2.YDrawOffset))
            {
                return -1;
            }
            else if ((character1.Position.Y + character1.YDrawOffset) > (character2.Position.Y + character2.YDrawOffset))
            {
                return 1;
            }

            return 0;
        }
        #endregion
    }
}
