using System;
using System.Windows.Threading;

namespace CodeBase
{
    public class Heart
    {
        private ApplicationData Data;
        private Action onTick;
        //
        private DispatcherTimer timer;
        private long lastTime;

        public Heart(ApplicationData Data, Action onTick)
        {
            this.Data = Data;
            this.onTick = onTick;

            lastTime = UnixTime.Now();

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Tick;
            timer.Start();
        }

        private void Tick(object sender, EventArgs args)
        {
            long now = UnixTime.Now();

            if (now - lastTime > Data.UpdateInterval * 60)
            {
                onTick?.Invoke();
                lastTime = now;
            }
        }
    }
}
