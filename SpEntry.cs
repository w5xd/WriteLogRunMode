using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WriteLogRunMode
{
    /* class SpEntry
     * This radio does not call CQ. It is in Search and Pounce mode.
     */
    internal class SpEntry : Entry
    {
        public enum States
        {
            SENDING_MSG, SENDING_VOX, // SENDING states
            RECEIVING_CALL, RECEIVING_EXCHANGE, RECEIVING_WITH_QUEUEDMSG   // RECEIVING states
        };

        public SpEntry(WriteLogClrTypes.ISingleEntry wlEntry, WriteLogClrTypes.IWriteL wl)
            : base(wlEntry, wl)
        {
        }

        #region object state

        private States m_state = States.RECEIVING_CALL;
        private short m_QueuedMessage = NO_MEMORY;

        #endregion

        #region Event handlers
        
        // a program F-key message is starting
        public override void OnStartMessage(
            int id,     // which message 1 is F1, 2 is F2, etc.
            int stillActive // non-zero means the "red box" is on--operator still typing
            )
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.SpEntry.OnStartMessage " +
                m_EntryId.ToString() + " " +
                m_state.ToString() + " msg: " +
                id.ToString());
#endif
            switch ((short)id)
            {
                default:
                    SetState(States.SENDING_MSG);
                    break;

            }
            if (stillActive == 0 || !m_Settings.EnableRedBox)
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

        // A programmed message has finished
        public override void OnProgramMessageCompleted()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.SpEntry.OnProgramMessageCompleted " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
            switch (m_state)
            {
                default:
                    SetState(States.RECEIVING_EXCHANGE);
                    break;
            }
            HeadphonesAsReceive();
        }

        // the operator logged a QSO
        public override void OnLoggedQso()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.SpEntry.OnLoggedQso " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
            SetState(States.RECEIVING_CALL);
            if (!m_other.Sending)
                m_other.GrabFocusAndPhones();
        }

        // The timed-CQ interval has elapsed since the end of a transmission
        public override void OnListenIntervalComplete()
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.SpEntry.OnListenIntervalComplete " +
                m_EntryId.ToString() + " " +
                m_state.ToString());
#endif
        }

        public override void OnWipeQSO()
        {
            if (!Sending)
                SetState(States.RECEIVING_CALL);
        }

        public override void OperatorMadeEntry(bool QsoIsBlank, WriteLogClrTypes.ISingleEntry r)
        {
                HeadphonesAsTyping();
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
            SetState(States.RECEIVING_CALL);
            m_wlEntry.SetXmitPtt(0);
        }

        #endregion

        public States State { get { return m_state; } }
        public Entry other { set { m_other = value; } }

        #region between Entry objects
        // The "other" entry window / radio changed its state
        public override void OtherStateChanged()
        {
            if (!m_other.Sending)
            {
                switch (m_state)
                {
                    case States.RECEIVING_WITH_QUEUEDMSG:
                        if (m_QueuedMessage > NO_MEMORY)
                            m_wlEntry.SendProgramMsg(m_QueuedMessage);
                        m_QueuedMessage = NO_MEMORY; // only use it once
                        break;
                    default:
                        break;
                }
            }
            else if (Sending)
            {   // if other moved to sending, and I was sending
                // then I am not sending anymore
                SetState(States.RECEIVING_CALL);
            }
        }

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
            get { 
                return Sending ? Sending_t.SENDING_DONT_STOPME : Sending_t.NOT_SENDING; 
            }
        }

        public override bool CanRelinquishFocus
        {
            /* We want to keep the focus if we are copying an exchange */
            get
            {
                return (m_state != States.SENDING_VOX) && 
                    (m_state != States.RECEIVING_EXCHANGE) && 
                    (String.IsNullOrEmpty(m_wlEntry.Callsign) ||
                    (m_wlEntry.CurrentFieldNumber != m_CallFieldNumber));
            }
        }

        public override short DelayStartMessage(int id)
        {
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

        #endregion

        private bool StateToSending(States value)
        {
            return (value == States.SENDING_MSG) ||
                    (value == States.SENDING_VOX);
        }

        private void SetState(States value)
        {
#if DEBUG
            Debug.WriteLine("WriteLogRunMode.SpEntry.SetState " +
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
                        case States.SENDING_VOX:
                        case States.RECEIVING_EXCHANGE:
                            GrabFocusAndPhones();
                            break;
                    }
                }
                else if (value == States.SENDING_VOX)
                {
                    m_other.GrabPhones();
                }
                else
                {
                    m_other.GrabFocusAndPhonesIfAppropriate();
                }

                m_other.OtherStateChanged();
            }
        }
    }
}
