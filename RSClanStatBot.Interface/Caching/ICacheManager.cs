namespace RSClanStatBot.Interface.Caching
{
    public interface ICacheManager
    {
        void BackupCache();
        bool LoadBackup();
    }
}