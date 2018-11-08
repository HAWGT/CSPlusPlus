using CSGOPlusPlus.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSGOPlusPlus
{
    public partial class MainForm : Form
    {
        private CSGOCore csgoCore;
        public MainForm()
        {
            InitializeComponent();
            csgoCore = new CSGOCore(this);
        }

        public bool Kill()
        {
            return chk_kill.Checked;
        }

        public bool KillHS()
        {
            return chk_killhs.Checked;
        }

        public bool Money()
        {
            return chk_money.Checked;
        }

        public bool Reload()
        {
            return chk_reload.Checked;
        }

        public bool Flash()
        {
            return chk_flash.Checked;
        }

        public bool Smoke()
        {
            return chk_smoke.Checked;
        }

        public bool Burn()
        {
            return chk_burn.Checked;
        }

        public bool MVP()
        {
            return chk_mvp.Checked;
        }

        public bool Knife()
        {
            return chk_knife.Checked;
        }

        public bool Assist()
        {
            return chk_assist.Checked;
        }

        public bool Death()
        {
            return chk_death.Checked;
        }

        public bool SpecialKill()
        {
            return chk_spkill.Checked;
        }

        public bool Taser()
        {
            return chk_taser.Checked;
        }

        public bool Ace()
        {
            return chk_ace.Checked;
        }

        public bool SpecialAce()
        {
            return chk_space.Checked;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
