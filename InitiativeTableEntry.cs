using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HeavyDuck.Dnd.InitiativeBuddy
{
    internal abstract class InitiativeTableEntry
    {

        public InitiativeTableEntry(string description, string html)
        {
            this.Description = description;
            this.Html = html;
        }

        public int Initiative { get; set; }
        public string Description { get; set; }
        public string Html { get; set; }

        public abstract bool IsCombatant { get; }

        public virtual Image Image
        {
            get
            {
                return null;
            }
        }
    }

    internal class MonsterInitiativeTableEntry : InitiativeTableEntry
    {
        public MonsterInitiativeTableEntry(string description, string html)
            : base(description, html) { }

        public override bool IsCombatant
        {
            get { return true; }
        }

        public override Image Image
        {
            get { return Properties.Resources.bug; }
        }
    }
}
