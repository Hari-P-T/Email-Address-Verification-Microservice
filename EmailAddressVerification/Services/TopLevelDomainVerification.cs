﻿using EmailAddressVerificationAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EmailAddressVerificationAPI.Services
{
    public class TopLevelDomainVerification
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "TopLevelDomains";
        private const string FilePath = "tlds.txt";
        private static readonly object CacheLock = new();

        public TopLevelDomainVerification(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            LoadTLDs();
        }

        private void LoadTLDs()
        {
            var topLevelDomains = new HashSet<string>();

            if (File.Exists(FilePath))
            {
                foreach (var line in File.ReadLines(FilePath))
                {
                    var domain = line.Trim().ToLower();
                    if (!string.IsNullOrEmpty(domain))
                    {
                        topLevelDomains.Add(domain);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Warning: {FilePath} not found. No domains loaded.");
            }

            _cache.Set(CacheKey, topLevelDomains, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(60)
            });
        }

        public Task<EmailStatusCode> IsRegisteredTLD(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return Task.FromResult(EmailStatusCode.Invalid);

            if (!_cache.TryGetValue(CacheKey, out HashSet<string>? topLevelDomains))
            {
                lock (CacheLock)
                {
                    if (!_cache.TryGetValue(CacheKey, out topLevelDomains))
                    {
                        LoadTLDs();
                        _cache.TryGetValue(CacheKey, out topLevelDomains);
                    }
                }
            }

            EmailStatusCode result = EmailStatusCode.Invalid;

            if (topLevelDomains.Contains(domain.ToLower()))
            {
                result = EmailStatusCode.Valid;
            }
            return Task.FromResult(result);
        }
    }
}
