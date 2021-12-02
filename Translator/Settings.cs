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

        public bool weapon = true;
        public bool armor = true;
        public bool ingredient = true;
        public bool npc = true;

        public bool worldSpace = true;

        public bool magicEffect = true;
        public bool objectEffect = true;
        public bool spell = true;
        public bool perk = true;

        public bool quest = true;
        

        
        public bool hazard = true;
        public bool message = true;
        public bool container = true;

        public bool cell = true;

        public bool activator = true;
        public bool ammunition = true;
        public bool projectile = true;

        public bool race = true;

        public bool scroll = true;
    }
}
