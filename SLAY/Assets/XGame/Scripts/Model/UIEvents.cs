﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    //通用
    public class UpdateGoldEvent
    {
        public float AnimDur = 0;
        public int BeginValue;
    }


    public class NewDayEvent { }
    public class ChangeFragmentsEvent { }



    public class GuideEndEvent
    {
        public GuideType GuideType;
        public bool IsClickGuideArea;
    }

    public class SetFlyTargetEvent { }


    public class SentUpdateGoldText
    {
        public bool isShowPalu;

    }

    // 玩家交互方式发生改变
    public class UpdateInteractEvent
    {
        public InteractType type;
    }

    public class PlayerInteractEvent
    {
        public InteractType type;
    }

    public class ShowHudEvent { }

    public class HungerUpdatedEvent
    {
        public int min;
        public int max;
        public int value;
    }
}
