using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}