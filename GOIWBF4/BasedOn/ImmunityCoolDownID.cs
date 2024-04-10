using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOIWBF4.BasedOn
{
    //此文件来自lace-wing的无敌帧github教学文档
    public static class ImmunityCooldownID
    {
        // 默认的, 没啥特别的,就是 Player.immuneTime
        public const int General = -1;

        // 物块的接触伤害, 像尖刺和饥荒世界中的仙人掌
        public const int TileContactDamage = 0;

        // 像月总和光女一样的Boss(以及它们的仆从和射弹)
        // 防止玩家用其它低额伤害来骗伤
        public const int Bosses = 1;

        // 塔防兽人的攻击, 除了特殊的击退效果以外和 -1 一样
        public const int DD2OgreKnockback = 2;

        // 尝试用普通捕虫网捕捉岩浆生物(得用防熔岩虫网或金虫网)
        public const int WrongBugNet = 3;

        // 岩浆伤害
        public const int Lava = 4;
    }
}
