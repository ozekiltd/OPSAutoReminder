using OzCommon.Model;

namespace AutoReminder.Model.Settings
{
    public class SettingsRepository : GenericSettingsRepository<AppPreferences>
    {
        public SettingsRepository()
            : base("OPSAutoReminder")
        { }
    }
}
