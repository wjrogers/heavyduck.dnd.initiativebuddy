﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeavyDuck.Dnd.Forms;
using HeavyDuck.Utilities.Collections;
using HeavyDuck.Utilities.Forms;

namespace HeavyDuck.Dnd.InitiativeBuddy.Forms
{
    public partial class Main : Form
    {
        private CompendiumHelper m_compendium = new CompendiumHelper();

        public Main()
        {
            InitializeComponent();

            this.Load += new EventHandler(Main_Load);
            this.FormClosed += new FormClosedEventHandler(Main_FormClosed);
        }

        #region Event Handlers

        private void Main_Load(object sender, EventArgs e)
        {
            // configure toolstrip
            toolstrip.GripStyle = ToolStripGripStyle.Hidden;
            toolstrip.Padding = new Padding(4, 0, 4, 0);
            toolstrip.Items.Add("New Encounter", Properties.Resources.application_add, ToolStrip_NewEncounter);
            toolstrip.Items.Add(new ToolStripButton("Remove Encounter", Properties.Resources.application_delete, ToolStrip_RemoveEncounter, "remove_encounter"));
            toolstrip.Items.Add("-");
            toolstrip.Items.Add(new ToolStripButton("Add PC", Properties.Resources.user_add, ToolStrip_AddPC, "add_pc"));
            toolstrip.Items.Add(new ToolStripButton("Add Monster", Properties.Resources.bug_add, ToolStrip_AddMonster, "add_monster"));

            // load cookies
            m_compendium.LoadCookies();

            // initialize button state
            UpdateButtons();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            // persist cookies
            m_compendium.SaveCookies();
        }

        private void ToolStrip_NewEncounter(object sender, EventArgs e)
        {
            TabPage page;
            DataGridView grid;
            ObjectBindingList<InitiativeTableEntry> entries;
            string name = "";

            // get a name for the new encounter
            if (InputDialog.ShowDialog(this, "New Encounter", "Encounter name?", ref name) != DialogResult.OK)
                return;

            // check the name is a name
            if (string.IsNullOrEmpty(name) || name.Trim().Length < 2)
            {
                MessageBox.Show(this, "Encounter names must be at least 2 characters long", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // create entry list
            entries = new ObjectBindingList<InitiativeTableEntry>();

            // create the grid
            grid = new DataGridView();
            grid.Name = "grid";
            GridHelper.Initialize(grid, true);
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridHelper.AddColumn(grid, "Initiative", "Init");
            GridHelper.AddColumn(grid, "InitiativeBonus", " ");
            GridHelper.AddColumn(grid, new DataGridViewImageColumn(), "Image", " ");
            GridHelper.AddColumn(grid, "Description", "Description");
            GridHelper.AddColumn(grid, "AC", "AC");
            GridHelper.AddColumn(grid, "Fortitude", "Fort");
            GridHelper.AddColumn(grid, "Reflex", "Ref");
            GridHelper.AddColumn(grid, "Will", "Will");
            grid.Columns["Initiative"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["InitiativeBonus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["InitiativeBonus"].DefaultCellStyle.Format = "(+0)";
            grid.Columns["AC"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["Fortitude"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["Reflex"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["Will"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridHelper.DisableClickToSort(grid, false);
            grid.CurrentCellChanged += new EventHandler(grid_CurrentCellChanged);
            grid.DataSource = entries;

            // create the tab page
            page = new TabPage(name);
            page.Controls.Add(grid);

            // add it to the tab control
            encounter_tabs.TabPages.Add(page);

            // refresh some stuff
            UpdateButtons();
        }

        private void ToolStrip_RemoveEncounter(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this, "Really delete encounter '" + encounter_tabs.SelectedTab.Text + "'?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.OK)
            {
                encounter_tabs.TabPages.Remove(encounter_tabs.SelectedTab);
                UpdateButtons();
            }
        }

        private void ToolStrip_AddPC(object sender, EventArgs e)
        {
            ObjectBindingList<InitiativeTableEntry> list = GetCurrentList();
            string path;

            // let the user browse for a file
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.CheckFileExists = true;
                d.Filter = "Character Builder Files (*.dnd4e)|*.dnd4e";

                if (d.ShowDialog(this) != DialogResult.OK)
                    return;

                path = d.FileName;
            }

            // read the file
            try
            {
                list.Add(new PCInitiativeTableEntry(path));
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ToolStrip_AddMonster(object sender, EventArgs e)
        {
            ObjectBindingList<InitiativeTableEntry> list = GetCurrentList();
            string monster_html;

            // sanity check
            if (list == null) return;

            using (MonsterSearchDialog dialog = new MonsterSearchDialog(m_compendium))
            {
                // let the user choose a monster from the compendium
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                // if that worked, get the monster info
                try
                {
                    using (StreamReader r = new StreamReader(m_compendium.GetMonster(dialog.SelectedMonsterID)))
                        monster_html = r.ReadToEnd();

                    monster_html = CompendiumHelper.FixStyles(monster_html);
                    list.Add(new MonsterInitiativeTableEntry(dialog.SelectedMonsterName, monster_html));
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
        }

        private void grid_CurrentCellChanged(object sender, EventArgs e)
        {
            InitiativeTableEntry entry = GetCurrentEntry((DataGridView)sender);

            if (entry == null || string.IsNullOrEmpty(entry.Html))
                browser.DocumentText = null;
            else
                browser.DocumentText = entry.Html;
        }

        #endregion

        #region Private Methods

        private DataGridView GetCurrentGrid()
        {
            if (encounter_tabs.SelectedTab == null)
                return null;
            else
                return (DataGridView)encounter_tabs.SelectedTab.Controls["grid"];
        }

        private ObjectBindingList<InitiativeTableEntry> GetCurrentList()
        {
            DataGridView grid = GetCurrentGrid();

            if (grid == null)
                return null;
            else
                return (ObjectBindingList<InitiativeTableEntry>)grid.DataSource;
        }

        private InitiativeTableEntry GetCurrentEntry(DataGridView grid)
        {
            if (grid.CurrentRow == null)
                return null;
            else
                return (InitiativeTableEntry)grid.CurrentRow.DataBoundItem;
        }

        private void UpdateButtons()
        {
            toolstrip.Items["remove_encounter"].Enabled = encounter_tabs.SelectedTab != null;
            toolstrip.Items["add_pc"].Enabled = encounter_tabs.SelectedTab != null;
            toolstrip.Items["add_monster"].Enabled = encounter_tabs.SelectedTab != null;
        }

        private void ShowException(Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
