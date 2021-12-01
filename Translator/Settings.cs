using Mutagen.Bethesda;
using System;
using System.Collections.Generic;
using System.Text;

namespace Translator
{
    public record Settings
    {
        //public List<ModKey> TargetMods = new List<ModKey>();
        public bool verboseLog = false;
        public string translationDir = "";
    }
}
