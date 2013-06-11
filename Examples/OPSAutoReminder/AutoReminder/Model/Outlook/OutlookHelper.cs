using System;

namespace AutoReminder.Model.Outlook
{
    public class OutlookHelper
    {
        public static bool IsOutlookInstalled()
        {
            try
            {
                var type = Type.GetTypeFromCLSID(new Guid("0006F03A-0000-0000-C000-000000000046")); //Outlook.Application
                if (type == null) return false;
                var obj = Activator.CreateInstance(type);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
