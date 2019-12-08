using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundManager
{
    internal class AppDomainWrapper : MarshalByRefObject
    {
        public void openUI()
        {
            var application = new System.Windows.Application();
            var ui = new BackgroundManagerUI.MainWindow();

            ui.Data = Handle.data;
            ui.DataContext = ui.Data;
            ui.loadNonBindings();

            application.Run(ui);
            application.Shutdown();
        }
    }
}