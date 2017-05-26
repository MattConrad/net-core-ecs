using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame.Parts
{
    /// <summary>
    /// An entity might have several of these. For now, the only anatomy modification is having 
    /// a slot disabled, effectively permanently.
    /// I expect eventually this will morph into different sorts of modifications, expirations, etc.
    /// 
    /// This has to do with equippability only--any debilities from missing parts are/will be handled separately.
    /// </summary>
    public class AnatomyModifier : EntropyEcsCore.EcsEntityPart
    {
        public string SlotDisabled { get; set; }
    }
}
