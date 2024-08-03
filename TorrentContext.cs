using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WorkerServiceNew
{
    public class TorrentContext : DbContext
    {
        public DbSet<Torrent> Torrents { get; set; }
        public TorrentContext(DbContextOptions<TorrentContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Torrent>();
        }
    }
}
