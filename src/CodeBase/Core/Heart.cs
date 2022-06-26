using System;
using System.Windows.Threading;

namespace CodeBase.Core
{
    public class Heart
    {
        private readonly ApplicationData _data;
        private readonly Action _onTick;
        private readonly DispatcherTimer _timer;
        private long _lastTime;

        public Heart(ApplicationData data, Action onTick)
        {
            _data = data;
            _onTick = onTick;

            _lastTime = UnixTime.Now();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object sender, EventArgs args)
        {
            var now = UnixTime.Now();

            if (now - _lastTime > _data.UpdateInterval * 60)
            {
                _onTick?.Invoke();
                _lastTime = now;
            }
        }
    }
}
