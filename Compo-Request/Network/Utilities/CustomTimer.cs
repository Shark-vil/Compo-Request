using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Compo_Request.Network.Utilities
{
    public class CustomTimer
    {
        public delegate void TimerDelegate(object sender, EventArgs e);

        public static DispatcherTimer Create(TimerDelegate TimerActionEvent, TimeSpan TickTime, bool SingleTick = true)
        {
            var DTimer = new DispatcherTimer();
            DTimer.Tick += new EventHandler(TimerActionEvent);
            DTimer.Interval = TickTime;

            if (SingleTick)
            {
                DTimer.Tick += new EventHandler(delegate(object sender, EventArgs e)
                {
                    DTimer.Stop();
                });
            }
               
            DTimer.Start();

            return DTimer;
        }

        public static DispatcherTimer Create(DispatcherTimer DTimer, TimeSpan TickTime, TimerDelegate TimerActionEvent)
        {
            DTimer = new DispatcherTimer();
            DTimer.Tick += new EventHandler(TimerActionEvent);
            DTimer.Interval = TickTime;
            DTimer.Start();

            return DTimer;
        }
    }
}
