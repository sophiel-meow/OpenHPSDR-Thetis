//=================================================================
// frmFilterManager.cs - MW0LGE 2021
//=================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thetis
{
    public partial class frmFilterManager : Form
    {
        public frmFilterManager()
        {
            InitializeComponent();
            this.Width = 930;
            LanguageManager.RegisterAndTranslateForm(this);
            LanguageManager.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => LanguageManager.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
                this.BeginInvoke((Action)(() => LanguageManager.TranslateForm(this)));
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            if (this.Width < 1272)
            {
                this.Width = 1272;
                btnMore.Text = "Less <<";
            }
            else
            {
                this.Width = 930;
                btnMore.Text = "More >>";
            }
                
        }
    }
}
