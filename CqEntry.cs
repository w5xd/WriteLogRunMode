using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WriteLogRunMode
{
    /* class CqEntry
      * 
      * This processor is a "run mode" processor--that is
      * you are always calling CQ, and you are doing so on both radios.
      * 
      * With the exception of causing a CQ to be sent when it thinks
      * nothing else is going on, this class doesn't invoke any
      * transmissions. Instead, it makes assumptions about what the
      * operator is sending based on which F key the operator pressed
      * to send a message.
      */

    internal class CqEntry : Entry
    {
        private const int MAX_CLEARED_IDLE_MSEC = 5000;
        public enum States
        {
            IDLE, // would like to send CQ, but were blocked for some reason
            DELAYED_TU, // would like to acq full QSO, but were delayed
            SENDING_CQ, SENDING_EXCHANGE, SENDING_AGN, SENDING_VOX, SENDING_TU, SENDING_OTHER, // SENDING states
            RECEIVING_CALL, RECEIVING_EXCHANGE, RECEIVING_WITH_QUEUEDMSG  // RECEIVING states
        };

        public CqEntry(WriteLogClrTypes.ISingleEntry wlEntry, WriteLogClrTypes.IWriteL wl)
            : base(wlEntry, wl)
        {
        }

        #region object state

        private States m_state = States.IDLE;
        private short m_QueuedMessage = NO_MEMORY;
        private int m_SentCALLat = System.Environment.TickCount;

        #endregion

        #region Event handlers

        // A programmed message has finished
        public override void OnProgramMessageCompleted()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.CqEntry.OnProgramMessageCompleted " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
            switch (m_state)
            {
                case States.SENDING_EXCHANGE:
                case States.SENDING_AGN:
                case States.SENDING_OTHER: // any MessageNN we don't understand
                case States.SENDING_VOX:
                    SetState(States.RECEIVING_EXCHANGE);
                    break;

                case States.SENDING_CQ:
                case States.SENDING_TU:
                    SetState(States.RECEIVING_CALL);
                    break;                
            }
            HeadphonesAsReceive();
        }

        public override short DelayStartMessage(int id) 
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.CqEntry.DelayStartMessage " +
                m_EntryId.ToString() + " " +
                m_state.ToString() + " " +
                id.ToString());
#endif
            // If we return non-zero, WriteLog won't send this message.
            if (m_other.SendingPriority == Sending_t.SENDING_DONT_STOPME)
            {
                if (id > NO_MEMORY)
                {
                    SetState(States.RECEIVING_WITH_QUEUEDMSG);
                    m_QueuedMessage = (short)id;
                }
                return 1;
            }
            return 0; 
        }

        // a program F-key message is starting
        public override void OnStartMessage(
            int id,     // which message 1 is F1, 2 is F2, etc.
            int stillActive // non-zero means the "red box" is on--operator still typing
            )
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.CqEntry.OnStartMessage " +
                m_EntryId.ToString() + " " +
                m_state.ToString() + " msg: " +
                id.ToString());
#endif
            switch ((short)id)
            {
                case CQ_MEMORY:
                    SetState(States.SENDING_CQ);
                    m_SentCALLat = System.Environment.TickCount;
                    break;

                case QSL_QRZ_MEMORY:
                    SetState(States.SENDING_TU);
                    break;

                case CALLEXCHANGE_MEMORY:
                case EXCHANGE_MEMORY:
                case HISCALL_MEMORY:
                    SetState(States.SENDING_EXCHANGE);
                    break;

                case AGN_MEMORY:
                    SetState(States.SENDING_AGN);
                    break;

                case MYCALL_MEMORY:
                    m_SentCALLat = System.Environment.TickCount;
                    break;

                default:
                    SetState(States.SENDING_OTHER);
                    break;

            }
            if ((stillActive == 0) || !m_Settings.EnableRedBox)
            {
                HeadphonesAsTransmit();
                m_other.GrabFocusAndPhones();
            }
        }

        // the red box in WriteLog erased.
        public override void OnMessageCALLComplete()
        {
            HeadphonesAsTransmit();
            m_other.GrabFocusAndPhones();
        }

        // the operator logged a QSO
        public override void OnLoggedQso()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.CqEntry.OnLoggedQso " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
            SendTU();
        }

        // The timed-CQ interval has elapsed since the end of a transmission
        public override void OnListenIntervalComplete()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.CqEntry.OnListenIntervalComplete " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
            switch (m_state)
            {
                case States.RECEIVING_CALL:
                    if (String.IsNullOrEmpty(m_wlEntry.Callsign))
                    {
                        if (!m_other.Sending)
                            SendCQ();
                        else
                            SetState(States.IDLE);
                    }
                    break;
            }
        }

        private int NonBlankOpEntryId = 0;
        private int BlankOpEntryId = -1;

        public override void OnWipeQSO()
        {
            NonBlankOpEntryId += 1; // disable timer
            if (!Sending)
            {
                switch (m_state)
                {
                    case States.DELAYED_TU:
                        break;

                    default:
                        if (!m_other.Sending)
                            SendCQ();
                        else
                            SetState(States.IDLE);
                        break;
                }
            }
        }

        private void OpBlankEntryTimer(int oldId)
        {
            if (oldId == NonBlankOpEntryId)
            {   // there has been no operator entry during the timer
                OnWipeQSO();
#if DEBUG
                Debug.WriteLine("Invoked Setting Blank Operator Entry timer");
#endif
            }
            else
            {
#if DEBUG
                Debug.WriteLine("Ignored Setting Blank Operator Entry timer");
#endif            
            }
        }

        public override void OperatorMadeEntry(bool QsoIsBlank, WriteLogClrTypes.ISingleEntry rentry)
        {
            if (QsoIsBlank)
            {
                if (BlankOpEntryId != NonBlankOpEntryId)
                {
                    int last = NonBlankOpEntryId;
                    TimerOnThisThread timerOnThread = new TimerOnThisThread(() => OpBlankEntryTimer(last));
                    System.Threading.Timer tm = new System.Threading.Timer(
                        timerOnThread.OnTimer,
                       timerOnThread,
                       MAX_CLEARED_IDLE_MSEC, // allow Entry Window to remain blank only 10 seconds
                       System.Threading.Timeout.Infinite);
                    timerOnThread.tm = tm;
                    BlankOpEntryId = NonBlankOpEntryId; // only start once
#if DEBUG
                    Debug.WriteLine("Setting Blank Operator Entry timer");
#endif
                }
            }
            else
                NonBlankOpEntryId += 1;
            HeadphonesAsTyping();
            if (m_Settings.FirstCallLetterStartsVOX &&
                State != States.SENDING_VOX &&
                m_CallFieldNumber == rentry.CurrentFieldNumber &&
                rentry.Callsign.Length == 1 )
            {
                HoldTransmitHere(false);
            }
        }

        public override void HoldTransmitHere(bool pttControl)
        {
            if (State == States.SENDING_VOX)
            {
                EndHoldTransmitHere();
                return;
            }
            SetState(States.SENDING_VOX);
            m_wlEntry.SetTransmitFocus();
            if (pttControl)
                m_wlEntry.SetXmitPtt(1);
            else
                m_wl.NotifyXmitStart();
            m_other.GrabPhones();
        }

        public override void EndHoldTransmitHere()
        {
            SetState(States.IDLE);
            m_wlEntry.SetXmitPtt(0);
        }

        public override RunModeSettings RunModeSettings
        {
            set
            {
                base.RunModeSettings = value;
                m_SentCALLat -= 1000 * m_Settings.MaxSecondsBetweenCALL;
            }
        }

        #endregion

        public States State { get { return m_state; } }
        public Entry other { set { m_other = value; } }

        #region between Entry objects
        public override bool Sending
        {
            get
            {
                return
                    StateToSending(m_state);
            }
        }

        public override Sending_t SendingPriority
        {
            get { return Sending ? 
                    (
                        (m_state == States.SENDING_CQ) ?
                            Sending_t.SENDING_CAN_STOPME 
                        : Sending_t.SENDING_DONT_STOPME)
                    : Sending_t.NOT_SENDING; }
        }

        public override bool CanRelinquishFocus
        {
            /* We want to keep the focus if we have entered 
               anything in the CALL field, or if we've moved
               the insert cursor out of the CALL field */
            get
            {
                return (m_state != States.SENDING_VOX) &&
                        String.IsNullOrEmpty(m_wlEntry.Callsign) &&
                        (m_wlEntry.CurrentFieldNumber == m_CallFieldNumber);
            }
        }

        // The "other" entry window / radio changed its state
        public override void OtherStateChanged()
        {
            if (!m_other.Sending)
            {
                switch (m_state)
                {
                    case States.DELAYED_TU:
                        SendTU();
                        break;
                    case States.IDLE:
                        if (String.IsNullOrEmpty(m_wlEntry.Callsign) &&
                            (m_wlEntry.CurrentFieldNumber == m_CallFieldNumber))
                        {
                            SendCQ();
                        }
                        else
                            SetState(States.RECEIVING_CALL);
                        break;

                    case States.RECEIVING_WITH_QUEUEDMSG:
                        if (m_QueuedMessage > NO_MEMORY)
                            m_wlEntry.SendProgramMsg(m_QueuedMessage);
                        m_QueuedMessage = NO_MEMORY; // only use it once
                        break;
                }
            }
            else if (Sending)
            {   // if other moved to sending, and I was sending
                // then I am not sending anymore--got aborted
                SetState(States.IDLE);
            }
        }
        
        
        #endregion

        private void SetState(States value)
        {
#if DEBUG
                Debug.WriteLine("WriteLogRunMode.CqEntry.SetState " +
                    m_EntryId.ToString() + " " +
                    value.ToString() + " from: " +
                    m_state.ToString());
#endif
            if (m_state != value)
            {
                m_state = value;
                if (!StateToSending(value))
                {
                    switch (value)
                    {
                        default:
                            GrabFocusAndPhonesIfAppropriate();
                        break;
                    }
                }
                else switch (value)
                {
                    case  States.SENDING_VOX:
                        break;
                    case States.SENDING_CQ:
                        m_other.GrabFocusAndPhones();
                        break;
                    default:
                        if (CanRelinquishFocus)
                            m_other.GrabFocusAndPhones();
                        break;
                }

               m_other.OtherStateChanged();
            }
        }

        private bool StateToSending(States value)
        {
            return (value == States.SENDING_AGN) ||
                    (value == States.SENDING_OTHER) ||
                    (value == States.SENDING_TU) ||
                    (value == States.SENDING_CQ) ||
                    (value == States.SENDING_EXCHANGE) ||
                    (value == States.SENDING_VOX);
        }

        private void SendCQ()
        {
            if (!m_other.Sending)
            {
                SetState(States.SENDING_CQ);
                m_wlEntry.SendProgramMsg(CQ_MEMORY);
            }
            else   //other radio was sending, so wait
                SetState(States.IDLE);
        }

        private void SendTU()
        {
            if (m_other.SendingPriority != Sending_t.SENDING_DONT_STOPME)
            {
                if (m_state != States.SENDING_TU)
                {
                    SetState(States.SENDING_TU);
                    m_wlEntry.SendProgramMsg(QSL_QRZ_MEMORY);
                }
                int maxSeconds = m_Settings.MaxSecondsBetweenCALL;
                if ((maxSeconds > 0) && 
                    (((System.Environment.TickCount - m_SentCALLat) / 1000) >
                    maxSeconds))
                    m_wlEntry.SendProgramMsg(MYCALL_MEMORY);
            }
            else   //other radio was sending, so wait
                SetState(States.DELAYED_TU);
        }
    }

    delegate void StaTimer();

    internal class TimerOnThisThread
    {
        private System.Windows.Threading.Dispatcher m_disp;
        private StaTimer m_ot;
        public TimerOnThisThread(StaTimer ot)
        {
            m_disp = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            m_ot = ot;
        }
        public System.Threading.Timer tm;
        public void OnTimer(Object o)
        {
            m_disp.Invoke(m_ot);
            if (tm != null) // there is a race...but we should never lose...
                tm.Dispose(); // ..but we check anyway
        }
    }
}
