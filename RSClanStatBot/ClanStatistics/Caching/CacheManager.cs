using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RSClanStatBot.Interface.Caching;

namespace RSClanStatBot.ClanStatistics.Caching
{
    public class CacheManager(IMemoryCache cache) : ICacheManager
    {
        private const string CacheBackupFilePath = "CacheBackup.txt";
        
        public void BackupCache()
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition?.GetValue(cache) as dynamic;

            var cacheCollection = new Dictionary<string, string>();

            if (cacheEntriesCollection != null)
            {
                foreach (var cacheItem in cacheEntriesCollection)
                {
                    ICacheEntry cacheEntry = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                    var key = cacheEntry.Key.ToString();
                    var value = cacheEntry.Value.ToString();

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        cacheCollection.Add(key, value);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(cacheCollection);
            if (File.Exists(CacheBackupFilePath))
            {
                File.Delete(CacheBackupFilePath);
            }
            File.WriteAllText(CacheBackupFilePath, json);
        }

        public bool LoadBackup()
        {
            if (!File.Exists(CacheBackupFilePath)) return false;
            
            var restoredDictionary = JsonConvert.DeserializeObject<Dictionary<string,string>>(File.ReadAllText(CacheBackupFilePath));

            foreach (var (key, value) in restoredDictionary)
            {
                if (cache.TryGetValue(key, out _))
                    cache.Remove(key);

                if(bool.TryParse(value, out var parsedBool))
                {
                    cache.GetOrCreate(key, entry =>
                    {
                        entry.SetValue(parsedBool);
                        return entry.Value;
                    });
                }
                else
                {
                    cache.GetOrCreate(key, entry =>
                    {
                        entry.SetValue(value);
                        return entry.Value;
                    });
                }
            }

            return true;
        }
    }
}