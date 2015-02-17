using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WriteLogRunMode
{
    public partial class RunModeSettingsForm : Form
    {
        private RunModeSettings m_settings;

        internal RunModeSettingsForm(RunModeSettings s)
        {
            m_settings = s;
            InitializeComponent();
        }

        private void RunModeSettingsForm_Load(object sender, EventArgs e)
        {
            maxSecondsBetweenCALL.Value = m_settings.MaxSecondsBetweenCALL;
            controlHpSplit.Checked = m_settings.DynamicHeadphoneSplit;
        }

        private void RunModeSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_settings.MaxSecondsBetweenCALL = (int)maxSecondsBetweenCALL.Value;
            m_settings.DynamicHeadphoneSplit = controlHpSplit.Checked;
            m_settings.Save();
        }

     }
}
