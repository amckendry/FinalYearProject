using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RPGProjectLibrary;

namespace RPGProject
{
    public class BattleTechniqueAction
    {
        int actionTime;
        public int ActionTime
        {
            get
            {
                return actionTime;
            }
        }
        BattleTechniqueActionType actionType;
        public BattleTechniqueActionType ActionType
        {
            get
            {
                return actionType;
            }
        }
        ActionStatType actionStatType;
        public ActionStatType ActionStatType
        {
            get
            {
                return actionStatType;
            }
        }
        int strength;
        public int Strength
        {
            get
            {
                return strength;
            }
        }

        public BattleTechniqueAction()
        {

        }
        public void LoadContent(BattleTechniqueActionData actionData)
        {
            actionTime = actionData.actionTime;
            actionType = actionData.actionType;
            actionStatType = actionData.actionStatType;
            strength = actionData.strength;
        }
    }
}
