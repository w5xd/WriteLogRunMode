using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WriteLogRunMode
{
    /* DuelingCQEntry
     * This class supports a much simpler model for CQing on 2 radios.
     * Start it with External: startDuelingCQtop (or bottom)
     * CQ's duel (alternating between radios) until you type anything in CALL
     * That stops the dueling until you do another startDuelingCQ.
     * 
     * stop2RadioRunMode cancels without typing
     */
    internal class DuelingCQEntry : Entry
    {
        public enum States
        {
            IDLE,
            STOPPED,
            SENDING_CQ,
            SENDING_OTHER,
        }

        public DuelingCQEntry(WriteLogClrTypes.ISingleEntry wlEntry,
                 WriteLogClrTypes.IWriteL wl)
            : base(wlEntry, wl)
        {
        }

        public void Init()
        {
#if DEBUG
            SetState(States.IDLE); // this is a BUG
#else
            m_state = States.IDLE;
#endif
        }

        public override void OnStartMessage(int id, int stillActive)
        {
            switch ((short)id)
            {
                case CQ_MEMORY:
                    SetState(States.SENDING_CQ);
                    break;
                default:
                    SetState(States.STOPPED);
                    break;
            }
        }

        public override short DelayStartMessage(int id)
        {
            // If we return non-zero, WriteLog won't send this message.
            return 0;
        }

        public override void OnProgramMessageCompleted()
        {
            switch (m_state)
            {
                case States.STOPPED:
                    break;
                default:
                    SetState(States.IDLE);
                    break;
            }
        }

        public override void OnMessageCALLComplete()
        {
        }

        public override void OnListenIntervalComplete()
        {
        }

        public override void OnLoggedQso()
        {
        }

        public override bool Sending
        {
            get
            {
                return
                  StateToSending(m_state);
            }
        }

        public override Entry.Sending_t SendingPriority
        {
            get
            {
                switch (m_state)
                {
                    case States.SENDING_CQ:
                    case States.SENDING_OTHER:
                        return Sending_t.SENDING_CAN_STOPME;
                    default:
                        break;
                }
                return Sending_t.NOT_SENDING;
           }
        }

        public override void OtherStateChanged()
        {
            if (!m_other.Sending)
            {
                {
                    switch (m_state)
                    {
                        case States.IDLE:
                            if (String.IsNullOrEmpty(m_wlEntry.Callsign) &&
                                (m_wlEntry.CurrentFieldNumber == m_CallFieldNumber))
                            {
                                m_wlEntry.SendProgramMsg(CQ_MEMORY);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else switch (m_state)
                {
                    case States.IDLE:
                        GrabFocusAndPhones();
                        break;
                }

        }
        
        public override bool CanRelinquishFocus
        {
            get { return true; }
        }

        public override void OnWipeQSO()
        {
        }

        public override void HoldTransmitHere(bool pttControl)
        {
        }

        public override void EndHoldTransmitHere()
        {
        }

        public override void OperatorMadeEntry(bool QsoIsBlank,
            WriteLogClrTypes.ISingleEntry rentry)
        {
            if (!QsoIsBlank)
            {
                switch (m_state)
                {
                    case States.STOPPED:
                        break;
                    case States.SENDING_OTHER:
                    case States.SENDING_CQ:
                        if (rentry != null)
                        {
                            m_wl.InvokeKeyboardCommand("MessageAbortTransmission");
                            GrabFocusAndPhones();
                        }
                        SetState(States.STOPPED);
                        break;
                    default:
                        if (rentry != null)
                            rentry.Activate();
                        SetState(States.STOPPED);
                        break;
                }
            }
        }

        public Entry other { set { m_other = value; } }

        #region object state
        private States m_state = States.STOPPED;

        private void SetState(States value)
        {
            if (m_state != value)
            {
                m_state = value;
                switch (value)
                {
                    case States.STOPPED:
                       // stop the OTHER guy, too
                        m_other.OperatorMadeEntry(false, null);
                        break;
               }
                
                m_other.OtherStateChanged();
            }
        }

        private bool StateToSending(States value)
        {
            return (value == States.SENDING_CQ) ||
                    (value == States.SENDING_OTHER);
        }
        #endregion
    }
}
