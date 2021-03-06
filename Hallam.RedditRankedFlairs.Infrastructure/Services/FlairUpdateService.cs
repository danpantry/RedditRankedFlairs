﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hallam.RedditRankedFlairs.Data;

namespace Hallam.RedditRankedFlairs.Services
{
    public class FlairService : IFlairService
    {
        private readonly IUnitOfWork _context;

        public FlairService(IUnitOfWork context)
        {
            _context = context;
        }

        public async Task<ICollection<User>> GetUsersForUpdateAsync(int max)
        {
            var results = new List<User>();
            var query = from user in _context.Users
                        where user.FlairUpdateRequiredTime.HasValue
                              || !user.FlairUpdatedTime.HasValue
                        orderby user.FlairUpdateRequiredTime
                        select user;

            return await query.Take(max).ToListAsync();
        }

        public async Task<bool> SetUpdateFlagAsync(IEnumerable<User> users, bool requiresUpdate = true)
        {
            foreach (var user in users)
            {
                user.FlairUpdateRequiredTime = requiresUpdate ? DateTimeOffset.Now : (DateTimeOffset?)null;
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetUpdatedAsync(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                user.FlairUpdateRequiredTime = null;
                user.FlairUpdatedTime = DateTimeOffset.Now;
            }
            return await _context.SaveChangesAsync() > 0;
        }
    }
}