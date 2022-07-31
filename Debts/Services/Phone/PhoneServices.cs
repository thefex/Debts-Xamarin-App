using System;
using Xamarin.Essentials;

namespace Debts.Services.Phone
{
    public class PhoneCallServices
    {
        private DateTime? _startTime;
        private TimeSpan _recentCallDuration;
        
        public PhoneCallServices()
        {
        }

        public void StartPhoneCall(string number)
        {
            IsCalling = true;
            PhoneDialer.Open(number);
        }

        public void StopPhoneCall()
        {
            IsCalling = false;
            if (_startTime.HasValue) 
                _recentCallDuration = DateTime.UtcNow - _startTime.Value;
        }
        
        public bool IsCalling { get; private set; }

        public TimeSpan RecentCallDuration => _recentCallDuration;

        public void OnConnected()
        {
            _startTime = DateTime.UtcNow;
        }
        
        public void OnDisconnected()
        {
            if (_startTime.HasValue)
                _recentCallDuration = DateTime.UtcNow - _startTime.Value;
            
            if (IsCalling)
                Disconnected?.Invoke();
        }

        public event Action Disconnected; 
    }
}