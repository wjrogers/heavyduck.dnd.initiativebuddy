using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace HeavyDuck.Dnd.InitiativeBuddy
{
    internal abstract class InitiativeTableEntry
    {
        private static readonly Regex m_number_regex = new Regex(@"[\+\-]?\d+");

        public InitiativeTableEntry(string description, string html)
        {
            this.Description = description;
            this.Html = html;
        }

        public int Initiative { get; set; }
        public int InitiativeBonus { get; set; }
        public string Description { get; set; }
        public string Html { get; set; }
        public int AC { get; set; }
        public int Fortitude { get; set; }
        public int Reflex { get; set; }
        public int Will { get; set; }

        public abstract bool IsCombatant { get; }

        public virtual Image Image
        {
            get
            {
                return null;
            }
        }

        protected static int TolerantParse(string value)
        {
            Match match = m_number_regex.Match(value);
            int result;

            if (match.Success && int.TryParse(match.Value, out result))
                return result;
            else
                return 0;
        }
    }

    internal class PCInitiativeTableEntry : InitiativeTableEntry
    {
        public PCInitiativeTableEntry(string path)
            : base("PC", null)
        {
            XPathDocument doc;
            XPathNavigator nav;

            // read the file
            using (FileStream fs = File.OpenRead(path))
            {
                doc = new XPathDocument(fs);
                nav = doc.CreateNavigator();
            }

            // pull stuff from it
            this.Description = nav.SelectSingleNode("/D20Character/CharacterSheet/Details/name").Value.Trim();
            this.InitiativeBonus = GetStat(nav, "initiative");
            this.AC = GetStat(nav, "AC");
            this.Fortitude = GetStat(nav, "Fortitude Defense");
            this.Reflex = GetStat(nav, "Reflex Defense");
            this.Will = GetStat(nav, "Will Defense");
        }

        public override bool IsCombatant
        {
            get { return true; }
        }

        public override Image Image
        {
            get { return Properties.Resources.user; }
        }

        private static int GetStat(XPathNavigator nav, string name)
        {
            XPathNavigator node = nav.SelectSingleNode("/D20Character/CharacterSheet/StatBlock/Stat[@name = '" + name + "']/@value");

            if (node != null)
                return TolerantParse(node.Value);
            else
                return 0;
        }
    }

    internal class MonsterInitiativeTableEntry : InitiativeTableEntry
    {
        public MonsterInitiativeTableEntry(string description, string html)
            : base(description, html)
        {
            // attempt to wade through the statblock tag soup and pull out the stats we need
            try
            {
                HtmlDocument doc;
                XPathNavigator nav;
                XPathNodeIterator iter;

                // load document and select all the bold thingies, they mark beginnings of things
                doc = new HtmlDocument();
                doc.LoadHtml(html);
                nav = doc.CreateNavigator();
                iter = nav.Select("//div[@id = 'detail']/p/b");

                // we have some chance of finding what we want here...
                while (iter.MoveNext())
                {
                    switch (iter.Current.Value.Trim().ToLowerInvariant())
                    {
                        case "initiative":
                            if (iter.Current.MoveToNext(XPathNodeType.Text))
                                this.InitiativeBonus = TolerantParse(iter.Current.Value);
                            break;
                        case "ac":
                            if (iter.Current.MoveToNext(XPathNodeType.Text))
                                this.AC = TolerantParse(iter.Current.Value);
                            break;
                        case "fortitude":
                            if (iter.Current.MoveToNext(XPathNodeType.Text))
                                this.Fortitude = TolerantParse(iter.Current.Value);
                            break;
                        case "reflex":
                            if (iter.Current.MoveToNext(XPathNodeType.Text))
                                this.Reflex = TolerantParse(iter.Current.Value);
                            break;
                        case "will":
                            if (iter.Current.MoveToNext(XPathNodeType.Text))
                                this.Will = TolerantParse(iter.Current.Value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // we really ought to use a logger and track this properly
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

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
