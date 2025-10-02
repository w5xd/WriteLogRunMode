using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace WriteLogRunMode
{
    sealed internal class RunModeSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValueAttribute("20")]
        public int MaxSecondsBetweenCALL
        {
            get { return (int)this["MaxSecondsBetweenCALL"]; }
            set { this["MaxSecondsBetweenCALL"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("False")]
        public bool DynamicHeadphoneSplit
        {
            get { return (bool)this["DynamicHeadphoneSplit"]; }
            set { this["DynamicHeadphoneSplit"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("False")]
        public bool FirstCallLetterStartsVOX
        {
            get { return (bool)this["FirstCallLetterStartsVOX"]; }
            set { this["FirstCallLetterStartsVOX"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("False")]
        public bool StopAlignsTransmitAndKeyboardFocus
        {
            get { return (bool)this["StopAlignsTransmitAndKeyboardFocus"]; }
            set { this["StopAlignsTransmitAndKeyboardFocus"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("True")]
        public bool EnableRedBox
        {
            get { return (bool)this["EnableRedBox"]; }
            set { this["EnableRedBox"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("False")]
        public bool Radio2ShiftF3
        {
            get { return (bool)this["Radio2ShiftF3"]; }
            set { this["Radio2ShiftF3"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValueAttribute("False")]
        public bool Radio2ShiftF1
        {
            get { return (bool)this["Radio2ShiftF1"]; }
            set { this["Radio2ShiftF1"] = value; }
        }
    }
}
