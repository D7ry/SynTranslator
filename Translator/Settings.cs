using Mutagen.Bethesda;
using System;
using System.Collections.Generic;
using System.Text;

namespace Translator
{
    public record Settings
    {
        public bool verboseLog = false;
        public List<ModKey> LOPlugins = new List<ModKey>();
        public List<String> outsidePlugins = new List<string>();
        public string translationDir = "";
        

        public bool forwardSelectedLOMods = false;
        public bool forwardOutsideMods = false;

        public bool weapon = true;
        public bool armor = true;
        public bool ingredient = true;
        public bool ingestible = true;
        public bool npc = true;

        public bool worldSpace = true;

        public bool magicEffect = true;
        public bool objectEffect = true;
        public bool spell = true;
        public bool perk = true;

        public bool quest = true;
        
        public bool hazard = true;
        public bool message = true;
        public bool ActorValueInformation = true;
        public bool container = true;

        public bool cell = true;

        public bool activator = true;
        public bool ammunition = true;
        public bool projectile = true;
        public bool book = true;

        public bool dialogueTopic = true; 
        public bool door = true; 
        public bool flora = true; 
        public bool furniture = true; 
        public bool key = true; 
        public bool location = true; 
        public bool miscItem = true; 
        public bool shout = true; 
        public bool soulGem = true; 
        public bool tree = true;


        public bool race = true;

        public bool scroll = true;
    }
}
