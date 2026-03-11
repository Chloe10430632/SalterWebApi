using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpRepositoryHelper;
using ExpServiceHelper.IService;

namespace ExpServiceHelper.Service
{
    internal class SCoachIndex : ISCoachIndex
    {
        public Task<(List<RCoachIndex> Items, int TotalCount)> DistrictAsync(string? dist, string? sortBy, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<(List<RCoachIndex> Items, int TotalCount)> HOTAsync(string? Hot, string? sortBy, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<RCoachIndex?> MyFavCoach()
        {
            throw new NotImplementedException();
        }
    }
}
