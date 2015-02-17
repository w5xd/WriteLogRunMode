using System;
using System.Collections.Generic;
using System.Text;

/*
 * WriteLog run mode processor
 * 
 * This assembly "plugs in" to WriteLog's external keyboard interface 
 * and implements a two radio run mode. It checks that you have at least
 * two Entry windows configured when you invoke its "start2RadioRunMode"
 * keyboard shortcut else it won't work.
 * 
 * It has a "stop2RadioRunMode" shortcut to turn off the run mode.
 * 
 */

namespace WriteLogRunMode
{
    //class RunModeProcessor is the connection from WriteLog 
    public class RunModeProcessor : 
        WriteLogShortcutHelper.EntryStateHelper /* shortcut helper is distributed with WriteLog */
    {
        /* These are the names of the commands as they will appear
           in WriteLog's Keyboard Shortcuts "Command to run" list: */
        string[] CommandNames = new string[] { 
                "stop2RadioRunMode", // this is which == 0
                  
                // ... and you can add as many as you like.
                // They will appear in WriteLog with "External:" prepended.
                // Look in the E's in that menu.
                "start2RadioRunMode", 
                "start1RadioRunMode",
                "holdTransmitOn",
                "endHoldTransmitOn",
                "EntryClear",
                "Setup"
        };

        // add "case" in the "switch" below
        public override void InvokeShortcut(
            int which, // corresponds to values in CommandNames
            WriteLogClrTypes.IWriteL wl // the root of the WriteLog object hierarchy
            )
        {
 
            // you can call methods on the wl object here. 
            switch (which)
            {
                case 0: // "stop2RadioRunMode"
                    m_Entries.Clear();
                    wl.InvokeKeyboardCommand("MessageAbortTransmission");
                    break;

                case 1: // "start2RadioRunMode"
                    if (!m_am2RadioCq)
                        m_Entries.Clear();
                    if (m_Entries.Count == 0)
                    {
                        WriteLogClrTypes.ISingleEntry r1 = wl.GetEntry(0) as WriteLogClrTypes.ISingleEntry;
                        WriteLogClrTypes.ISingleEntry r2 = wl.GetEntry(1) as WriteLogClrTypes.ISingleEntry;
                        if ((r1 != null) && (r2 != null))
                        {
                            short CallFieldNumber = wl.CALLFieldNumber;
                            CqEntry e1 = new CqEntry(r1, wl);
                            CqEntry e2 = new CqEntry(r2, wl);
                            e1.CallFieldNumber = CallFieldNumber;
                            e2.CallFieldNumber = CallFieldNumber;
                            e1.other = e2;
                            e2.other = e1;
                            m_Entries[r1.GetEntryId()] = e1;
                            m_Entries[r2.GetEntryId()] = e2;
                            m_am2RadioCq = true;
                            e1.RunModeSettings = m_Settings;
                            e2.RunModeSettings = m_Settings;
                        }
                        else
                            throw new System.NotSupportedException("Need two Entry Windows");
                    }
                    if (m_Entries.Count > 0)
                    {
                        WriteLogClrTypes.ISingleEntry en = (WriteLogClrTypes.ISingleEntry)
                            wl.GetCurrentEntry();
                        wl.TimedCqMessageNumber = 0; // cancel WL's built-in auto-cq, if its active
                        en.SendProgramMsg(Entry.CQ_MEMORY);
                    }
                    break;

                case 2:// "start1RadioRunMode"
                    {
                        m_Entries.Clear();
                        m_am2RadioCq = false;
                        WriteLogClrTypes.ISingleEntry rcq = wl.GetCurrentEntry() as WriteLogClrTypes.ISingleEntry;
                        short curId = rcq.GetEntryId();
                        short otherId = (short)((((int)curId - 1) + 1) % 2);
                        WriteLogClrTypes.ISingleEntry rsp = wl.GetEntry(otherId) as WriteLogClrTypes.ISingleEntry;
                        if ((rsp != null) && (rcq != null))
                        {
                            short CallFieldNumber = wl.CALLFieldNumber;
                            CqEntry ecq = new CqEntry(rcq, wl);
                            SpEntry esp = new SpEntry(rsp, wl);
                            ecq.CallFieldNumber = CallFieldNumber;
                            esp.CallFieldNumber = CallFieldNumber;
                            ecq.other = esp;
                            esp.other = ecq;
                            m_Entries[rcq.GetEntryId()] = ecq;
                            m_Entries[rsp.GetEntryId()] = esp;
                            ecq.RunModeSettings = m_Settings;
                            esp.RunModeSettings = m_Settings;
                         }
                         else
                            throw new System.NotSupportedException("Need two Entry Windows");
                        wl.TimedCqMessageNumber = 0; // cancel WL's built-in auto-cq, if its active
                        rcq.SendProgramMsg(Entry.CQ_MEMORY);
                    }
                    break;

                case 3: // "holdTransmitHere
                    {
                        WriteLogClrTypes.ISingleEntry wle = 
                            wl.GetCurrentEntry() as WriteLogClrTypes.ISingleEntry;
                        short curId = wle.GetEntryId();
                        Entry ent;
                        if (m_Entries.TryGetValue(curId, out ent))
                            ent.HoldTransmitHere();
                    }
                    break;

                case 4:
                    {
                        WriteLogClrTypes.ISingleEntry wle =
                        wl.GetCurrentEntry() as WriteLogClrTypes.ISingleEntry;
                        short curId = wle.GetEntryId();
                        Entry ent;
                        if (m_Entries.TryGetValue(curId, out ent))
                            ent.EndHoldTransmitHere();
                    }
                    break;

                case 5: // EntryClear
                    {
                        wl.InvokeKeyboardCommand("EntryClear");
                        WriteLogClrTypes.ISingleEntry wle =
                            wl.GetCurrentEntry() as WriteLogClrTypes.ISingleEntry;
                        short curId = wle.GetEntryId();
                        Entry ent;
                        if (m_Entries.TryGetValue(curId, out ent))
                                ent.OnWipeQSO();
                    }
                    break;

                case 6: // Setup
                    {
                        RunModeSettingsForm f = new RunModeSettingsForm(m_Settings);
                        f.ShowDialog();
                        f.Dispose();
                    }
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        // you must have this property. The code here should be OK without change.
        public override int ShortcutCount
        { get { return CommandNames.Length; } }


        // and you must have this one. The code here should be OK without change.
        public override string this[int which]
        {
            get
            {
                if ((which < 0) || (which >= CommandNames.Length))
                    throw new IndexOutOfRangeException();
                return CommandNames[which];
            }
        }

        #region object state
        private Dictionary<short, Entry> m_Entries = new Dictionary<short, Entry>();
        private bool m_am2RadioCq = false;
        private RunModeSettings m_Settings = new RunModeSettings();
        #endregion

        #region IEntryNotification Members

        // in all methods, simply route the notification to the appropriate Entry

        public override void OnProgramMessageCompleted(WriteLogClrTypes.ISingleEntry rEntry, WriteLogClrTypes.IWriteL rWl)
        {
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                ent.OnProgramMessageCompleted();
        }

        public override void OnListenIntervalComplete(WriteLogClrTypes.ISingleEntry rEntry, WriteLogClrTypes.IWriteL rWl)
        {
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                ent.OnListenIntervalComplete();
        }

        public override void OnStartMessage(WriteLogClrTypes.ISingleEntry rEntry, 
            WriteLogClrTypes.IWriteL rWl, int msg,  int stillActive)
        {
            rWl.TimedCqMessageNumber = 0; // cancel WL's built-in auto-cq, if its active
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                ent.OnStartMessage(msg, stillActive);
        }

        public override short DelayStartMessage(WriteLogClrTypes.ISingleEntry rEntry, WriteLogClrTypes.IWriteL rWl, int msg)
        {
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                return ent.DelayStartMessage(msg);
            return 0;
        }

        public override void OnMessageCALLComplete(WriteLogClrTypes.ISingleEntry rEntry, WriteLogClrTypes.IWriteL rWl)
        {
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                ent.OnMessageCALLComplete();
        }

        public override void OnLoggedQso(WriteLogClrTypes.ISingleEntry rEntry, WriteLogClrTypes.IWriteL rWl)
        {
            short id = rEntry.GetEntryId();
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
                ent.OnLoggedQso();
        }

        public override void OnEntryWindowUpdated(WriteLogClrTypes.ISingleEntry rentry, short isblank, WriteLogClrTypes.IWriteL wl)
        {
            short id = rentry.GetEntryId();
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                "WriteLogRunMode.RunModeProcessor.OnEntryWindowUpdated " +
                id.ToString() );
#endif            
            Entry ent;
            if (m_Entries.TryGetValue(id, out ent))
            {
                if (isblank == 0)
                    ent.OperatorMadeEntry();
            }
        }
        #endregion

    }
}
