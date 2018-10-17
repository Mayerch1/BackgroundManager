using System.Device.Location;

namespace BackgroundManager
{
    public static class LocationManager
    {
        public static void setCoordinates()
        {
            var watcher = new GeoCoordinateWatcher();
            var coord = watcher.Position.Location;
            if (!watcher.Position.Location.IsUnknown)
            {
                Handle.data.Latitude = coord.Latitude;
                Handle.data.Longitude = coord.Longitude;
            }
            else
            {
                setCoordinatesAlt();
            }
        }

        private static void setCoordinatesAlt()
        {
        }
    }
}