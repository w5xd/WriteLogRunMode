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
            checkBoxCallVox.Checked = m_settings.FirstCallLetterStartsVOX;
            checkBoxStopWithFocus.Checked = m_settings.StopAlignsTransmitAndKeyboardFocus;
            cbRedBox.Checked = m_settings.EnableRedBox;
        }

        private void RunModeSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_settings.MaxSecondsBetweenCALL = (int)maxSecondsBetweenCALL.Value;
            m_settings.DynamicHeadphoneSplit = controlHpSplit.Checked;
            m_settings.FirstCallLetterStartsVOX = checkBoxCallVox.Checked;
            m_settings.StopAlignsTransmitAndKeyboardFocus = checkBoxStopWithFocus.Checked;
            m_settings.EnableRedBox = cbRedBox.Checked;
            m_settings.Save();
        }

     }
}
