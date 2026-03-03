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
    public partial class ShutdownForm : Form
    {
        public ShutdownForm()
        {
            InitializeComponent();
            LanguageManager.RegisterAndTranslateForm(this);
            LanguageManager.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => LanguageManager.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated)
                this.BeginInvoke((Action)(() => LanguageManager.TranslateForm(this)));
        }
    }
}
