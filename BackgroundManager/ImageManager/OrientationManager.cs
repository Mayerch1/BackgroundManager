using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundManager.ImageManager
{
    public class OrientationManager : ImageManager
    {
        private OrientationWatcher orientationWatcher = new OrientationWatcher();

        public void init()
        {
            Handle.data.IsFlipChanged += changeOrientationFlip;

            //enable/disable Watcher
            changeOrientationFlip(Handle.data.IsFlipEnabled);

            //sets new image, according to orientation
            if (Handle.data.IsFlipEnabled)
                orientationChanged(orientationWatcher.getOrientation());
        }

        private void changeOrientationFlip(bool flipOnChange)
        {
            //subscribe/unscubscribe from delegate
            if (flipOnChange)
            {
                orientationWatcher.OrientationChanged += orientationChanged;
            }
            else
            {
                orientationWatcher.OrientationChanged -= orientationChanged;
            }
        }

        private void orientationChanged(Orientation orientation)
        {
            string path;

            if (orientation == Orientation.Landscape)
            {
                path = Handle.data.LandscapePath;
            }
            else
            {
                path = Handle.data.PortraitPath;
            }

            setImage(path);
        }
    }
}