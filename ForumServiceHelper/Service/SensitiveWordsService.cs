using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Response;
using Microsoft.Extensions.Caching.Memory;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Service
{
    public class SensitiveWordsService : ISensitiveWordsService
    {
        private readonly IGenericSalterRepository<ForumSensitiveWord> _dbWords;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "SensitiveWords_List";
        public SensitiveWordsService(IGenericSalterRepository<ForumSensitiveWord> dbWords, IMemoryCache cache)
        {
            _dbWords = dbWords;
            _cache = cache;
        }

        public async Task<SensitiveWordsResponseModel> ValidateContentAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return new SensitiveWordsResponseModel { IsValid = true };

            // --- 快取邏輯開始 ---
            if (!_cache.TryGetValue(CacheKey, out List<string> sensitiveWordList))
            {
                var words = await _dbWords.GetAllAsync();
                sensitiveWordList = words.Select(x => x.Word).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // 1小時絕對過期
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20)); // 20分鐘沒人動就過期

                _cache.Set(CacheKey, sensitiveWordList, cacheOptions);
            }
            // --- 快取邏輯結束 ---

            // 進行比對 (使用 Contains 簡單有效，若字數極大可改用 Regex)
            var foundWords = sensitiveWordList.Where(w => content.Contains(w)).ToList();

            return new SensitiveWordsResponseModel
            {
                IsValid = !foundWords.Any(),
                ViolatedWords = foundWords
            };
        }
    }
    }

