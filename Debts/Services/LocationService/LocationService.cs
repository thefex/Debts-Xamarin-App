using System;
using System.Reactive.Subjects;
using System.Security;
using System.Threading.Tasks;
using Debts.Data;
using MvvmCross.Plugin.Location;

namespace Debts.Services.LocationService
{
    public class LocationService
    { 
        private Subject<MvxGeoLocation> _locationSubject = new Subject<MvxGeoLocation>();
        private readonly IMvxLocationWatcher _locationWatcher;

        public LocationService(IMvxLocationWatcher locationWatcher)
        {
            _locationWatcher = locationWatcher;
        }
        
        public bool IsObservingLocation { get; private set; }

        public void StartObserving()
        {
            if (_locationWatcher.Started)
                return;
            
            _locationWatcher
                .Start(new MvxLocationOptions()
                {
                    TrackingMode = MvxLocationTrackingMode.Foreground,
                    TimeBetweenUpdates = TimeSpan.FromSeconds(10),
                    MovementThresholdInM = 200,
                    Accuracy = MvxLocationAccuracy.Fine
                }, onSuccess => { _locationSubject.OnNext(onSuccess); }, onError =>
                {
                    _locationSubject.OnError(new InvalidOperationException(onError.ToString()));
                });

            IsObservingLocation = true;
            
            if (_locationWatcher.CurrentLocation != null)
                _locationSubject.OnNext(_locationWatcher.CurrentLocation);
            else if (_locationWatcher.LastSeenLocation != null)
                _locationSubject.OnNext(_locationWatcher.LastSeenLocation);
        }


        public MvxGeoLocation MostRecentLocation =>
            _locationWatcher?.CurrentLocation ?? _locationWatcher.LastSeenLocation;

        public void StopObserving()
        {
            if (!_locationWatcher.Started)
                return;
            _locationWatcher.Stop();
            IsObservingLocation = false;
            _locationSubject.Dispose();
            _locationSubject = new Subject<MvxGeoLocation>();
        }

        public IObservable<MvxGeoLocation> GeolocationsObservable => _locationSubject;
        
    }
}