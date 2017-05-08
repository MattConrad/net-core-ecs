using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntropyEcsCore;

namespace SampleGame.Sys
{
    internal static class EntityName
    {
        /// <summary>
        /// Overwrite an entity name with the values from a new EntityName part. Null values in the new part are ignored.
        /// The original part is kept, and the new part is discarded after overwriting.
        /// </summary>
        internal static void Overwrite(EcsRegistrar rgs, long entityId, Parts.EntityName newNames)
        {
            var currentNames = rgs.GetPartsOfType<Parts.EntityName>(entityId).Single();
            currentNames.LongDescription = newNames.LongDescription ?? currentNames.LongDescription;
            currentNames.ObscuredName = newNames.ObscuredName ?? currentNames.ObscuredName;
            currentNames.Pronoun = newNames.Pronoun ?? currentNames.Pronoun;
            currentNames.ProperName = newNames.ProperName ?? currentNames.ProperName;
            currentNames.ShortDescription = newNames.ShortDescription ?? currentNames.ShortDescription;
        }
    }
}
