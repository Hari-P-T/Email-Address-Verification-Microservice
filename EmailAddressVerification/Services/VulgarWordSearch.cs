using EmailAddressVerification.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EmailAddressVerification.Services
{
    public class VulgarWordSearch
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "ProfanityWords";
        private const string FilePath = "final_profanity_v1.txt";
        private static readonly object CacheLock = new();

        public VulgarWordSearch(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            LoadVulgarWords();
        }

        private void LoadVulgarWords()
        {
            var vulgarWords = new HashSet<string>();

            if (File.Exists(FilePath))
            {
                foreach (var line in File.ReadLines(FilePath))
                {
                    var words = line.Trim().ToLower();
                    if (!string.IsNullOrEmpty(words))
                    {
                        vulgarWords.Add(words);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Warning: {FilePath} not found. No domains loaded.");
            }

            _cache.Set(CacheKey, vulgarWords, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(60)
            });
        }

        //public Task<bool> HasVulgarWords(string domain)
        //{
        //    if (string.IsNullOrWhiteSpace(domain)) return false;

        //    if (!_cache.TryGetValue(CacheKey, out HashSet<string>? vulgarWords))
        //    {
        //        lock (CacheLock)
        //        {
        //            if (!_cache.TryGetValue(CacheKey, out vulgarWords))
        //            {
        //                LoadVulgarWords();
        //                _cache.TryGetValue(CacheKey, out vulgarWords);
        //            }
        //        }
        //    }
        //    return await vulgarWords?.Contains(domain.ToLower()) ?? false;
        //}

        public Task<EmailStatusCode> HasVulgarWordsAsync(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return Task.FromResult(EmailStatusCode.Invalid);

            if (!_cache.TryGetValue(CacheKey, out HashSet<string>? vulgarWords))
            {
                lock (CacheLock)
                {
                    if (!_cache.TryGetValue(CacheKey, out vulgarWords))
                    {
                        LoadVulgarWords();
                        _cache.TryGetValue(CacheKey, out vulgarWords);
                    }
                }
            }

            EmailStatusCode result=EmailStatusCode.Invalid;
            if (vulgarWords.Contains(domain.ToLower()))
            {
                result = EmailStatusCode.Valid;
            }
            return Task.FromResult(result);
        }

    }
}