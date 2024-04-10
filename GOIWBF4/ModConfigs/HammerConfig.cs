using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace GOIWBF4.ModConfigs
{
    public class HammerConfig:ModConfig
    {
        
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [Header("[i:7]GettingOverItInTerraria'sModConfig")]
        [Range(.1f, 3f)]
        [DefaultValue(1f)]
        [Increment(.1f)]
        public float GivenPlayerVelocityXBase;
        [Range(.1f, 3f)]
        [DefaultValue(1f)]
        [Increment(.1f)]
        public float GivenPlayerVelocityYBase;
        [Range(1, 20)]
        [DefaultValue(1)]
        [Slider]
        [DrawTicks]
        public int PastPositionRecordCooldown;
        [Range(4, 32)]
        [DefaultValue(16)]
        [Slider]
        [DrawTicks]
        [Increment(4)]
        public int PastPositionRecordMaxTimes;
    }
}
