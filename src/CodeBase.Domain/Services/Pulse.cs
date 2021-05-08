using System;
using System.Windows.Threading;

namespace CodeBase.Domain.Services
{
    public class Pulse
    {
        private readonly Context _context;
        private readonly Action _onTick;
        private DateTime _lastTime;
        private readonly DispatcherTimer _timer;

        public Pulse(Context context, Action onTick)
        {
            _context = context;
            _onTick = onTick;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Tick;
        }

        public void Start() 
        {
            _lastTime = DateTime.Now;
            _timer.Start();
        }

        public void Pause() => _timer.Stop();

        private void Tick(object sender, EventArgs args)
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromMinutes(_context.AppData.AutoUpdateInterval);

            if (now - _lastTime > span)
            {
                _onTick();
                _lastTime = now;
            }
        }
        
    }
}
