using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WBMT.Models;

namespace WBMT.Data
{
    public class WBMTContext : DbContext
    {
        public WBMTContext (DbContextOptions<WBMTContext> options)
            : base(options)
        {
        }

        public DbSet<WBMT.Models.UserModel> UserModel { get; set; } = default!;
        public DbSet<WBMT.Models.ReUsersModel> reUsersModels { get; set; } = default!;
    }
}
