namespace DeviceTracker.Services
{
    public interface ISettingService
    {
        bool IsInitialRun { get; set; }

        bool DeviceBackupEnabled { get; set; }

        string MediaBackupPath { get; set; }

        bool DefaultMediaBackupPathEnabled { get; set; }
    }
}