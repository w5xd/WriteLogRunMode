﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WriteLogRunMode
{
    /* 
     * class Entry is the base class in common with S&P and CQ.
     * This class has only the basic plumbing to receive events
     * and plug in to the RunModeProcessor. It is abstract and
     * is subclassed (presently) for either CQ mode or S&P
     * 
     * It is designed to work with exactly two radios.
     * 
     * Each implements a state machine that moves among the various
     * states of a contest QSO-- copying a CALL, sending
     * the exchange and copying the response, and, maybe, call CQ
     * 
     */

    internal abstract class Entry: IDisposable
    {
        // The operator must setup his memories like this:
        public const short CQ_MEMORY = 11; // F11 or F1 sends CQ
        protected const short CALLEXCHANGE_MEMORY = 10;   // F10 sends call and exchange
        protected const short EXCHANGE_MEMORY = 2; // F2 sends exchange, without call
        protected const short QSL_QRZ_MEMORY = 3; // F3 sends "QSL QRZ?"
        protected const short MYCALL_MEMORY = 4;  // F4 sends my call
        protected const short HISCALL_MEMORY = 5; // F5 sends his call
        protected const short AGN_MEMORY = 6;     // F4 sends AGN?
        protected const short NO_MEMORY = 1;     // flag that means no message at all

        protected Entry(WriteLogClrTypes.ISingleEntry wlEntry, WriteLogClrTypes.IWriteL wl)
        {
            m_wlEntry = wlEntry;
            m_wl = wl;
            m_EntryId = wlEntry.GetEntryId(); // Only for debug displays
        }

        #region events
        public abstract void OnStartMessage(
            int id,     // which message 1 is F1, 2 is F2, etc.
            int stillActive // non-zero means the "red box" is on--operator still typing
            );
        public abstract short DelayStartMessage(int id);
        public abstract void OnProgramMessageCompleted();
        public abstract void OnMessageCALLComplete();
        public abstract void OnListenIntervalComplete();
        public abstract void OnLoggedQso();
        #endregion

        #region between Entry objects

        public abstract bool Sending { get; }
        public enum Sending_t {SENDING_DONT_STOPME, SENDING_CAN_STOPME, NOT_SENDING};
        public abstract Sending_t SendingPriority { get; }
        public abstract void OtherStateChanged();

        #endregion
        
        #region object state

        protected Entry m_other; // the other radio
        protected WriteLogClrTypes.ISingleEntry m_wlEntry;
        protected WriteLogClrTypes.IWriteL m_wl;
        protected short m_lastHeadphonesSet = -1;
        protected bool m_AmOperatingCW = false;
        protected bool m_AmOperatingVoice = false;
        protected bool m_VoiceCQmemoryInterruptible = true;

        // We have some logic based on what the operator has done with
        // his Entry window insertion cursor and need to know which
        // is the CALL field.
        protected short m_CallFieldNumber;
        protected short m_EntryId;    // debuging output only.
        protected RunModeSettings m_Settings;

        #endregion

        public short CallFieldNumber { set { m_CallFieldNumber = value; } }
        public virtual RunModeSettings RunModeSettings { set { m_Settings = value; } }

        // this is both headphones and keyboard
        public void GrabFocusAndPhonesIfAppropriate()
        {
#if DEBUG
            Debug.WriteLine(m_EntryId.ToString() + " " +
                DateTime.Now.TimeOfDay + " WriteLogRunMode.Entry.GrabFocusAndPhonesIfAppropriate " );
#endif            
            if (m_other.CanRelinquishFocus)
                GrabFocusAndPhones();
            else if (m_other.CanRelinquishHeadphoneFocus)
                GrabPhones();
        }

        // this is both headphones and keyboard
        public void GrabFocusAndPhones()
        {
#if DEBUG
            Debug.WriteLine(m_EntryId.ToString() + " " + DateTime.Now.TimeOfDay + " WriteLogRunMode.Entry.GrabFocusAndPhones " );
#endif
            m_wlEntry.SetFocusWithPhones(1  /* headphones too, if not split */ );
        }

        // headphones only
        public void GrabPhones()
        {
            m_wlEntry.SetHeadphones(1  /* do nothing if headphones are split */);
        }

        protected void CheckWhetherCW()
        {
            short mode=0; double rx=0; double tx=0; short split=0;
            m_wlEntry.GetLogFrequency(ref mode, ref rx, ref tx, ref split);
            m_AmOperatingCW = mode == 3;
            m_AmOperatingVoice = (mode != 3) && (mode != 6);
            var t = m_wl.GetFKeyMsgVoice(CQ_MEMORY-2).ToUpper();
            m_VoiceCQmemoryInterruptible = 
                    !m_AmOperatingVoice ||
                    !m_wl.GetFKeyMsgVoice(CQ_MEMORY-2).ToUpper().StartsWith("%A");
        }


        #region dynamic heaphone split support

        // subclasses call here when they switch to receive
        protected void HeadphonesAsReceive()
        {
            if (!m_Settings.DynamicHeadphoneSplit)
                return;
            short next = (short)(m_other.Sending ? 0 : 1);
            m_wl.HeadphonesSplit = m_lastHeadphonesSet = next;
        }

        // subclasses call here when they switch to transmit
        protected void HeadphonesAsTransmit()
        {
            if (!m_Settings.DynamicHeadphoneSplit)
                return;
            m_wl.HeadphonesSplit = m_lastHeadphonesSet = 0;
        }

        protected void HeadphonesAsTyping()
        {
            if (!m_Settings.DynamicHeadphoneSplit)
                return;
            short next = 0;
            if (next != m_lastHeadphonesSet)
            {
                m_wl.HeadphonesSplit = m_lastHeadphonesSet = next;
                m_wlEntry.SetHeadphones(0);
            }
        }

        #endregion

        public abstract bool CanRelinquishFocus   {    get;  }
        public virtual bool CanRelinquishHeadphoneFocus { get { return false; } }
        public abstract void OnWipeQSO();
        public abstract void HoldTransmitHere(bool pttControl);
        public abstract void EndHoldTransmitHere();
        public abstract void OperatorMadeEntry(bool QsoIsBlank, WriteLogClrTypes.ISingleEntry rentry);

        #region IDisposable Support
        // This is very ugly, but required to avoid a crash at WriteLog exit.
        // This object retains references to COM objects that it must call 
        // Marshal.ReleaseComObject() to free up before WL exits:
        // https://devblogs.microsoft.com/cppblog/mixing-deterministic-and-non-deterministic-cleanup/
        private bool disposedValue = false; // To detect redundant calls

        // the TODO's below come from Visual Studio's implementation using the dispose pattern.
        // The comments are retained to explain where future changes belong.
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                // unmanaged disposals
                if (null != m_wl)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wl);
                m_wl = null;
                if (null != m_wlEntry)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(m_wlEntry);
                m_wlEntry = null;

                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
         ~Entry() {
           // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           Dispose(false);
         }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }

 
 }
