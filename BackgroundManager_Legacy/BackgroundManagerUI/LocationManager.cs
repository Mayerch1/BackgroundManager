using System;
using System.Device.Location;

namespace BackgroundManagerUI
{
    public static class LocationManager
    {
        public static Tuple<double?, double?> setCoordinates()
        {
            double? latitude, longitude;

            var watcher = new GeoCoordinateWatcher();
            var coords = watcher.Position.Location;
            if (!watcher.Position.Location.IsUnknown)
            {
                latitude = coords.Latitude;
                longitude = coords.Longitude;
            }
            else
            {
                latitude = longitude = null;
                setCoordinatesAlt();
            }

            return Tuple.Create(latitude, longitude);
        }

        private static void setCoordinatesAlt()
        {
        }
    }
}