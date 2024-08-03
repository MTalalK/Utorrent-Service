using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorkerServiceNew
{
    public record Torrent
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Hash { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public long OriginalBytes { get; set; } = 0;
        public long DownloadedBytes { get; set; } = 0;
        public int CompletionStatus { get; set; } = 0;
        [DeniedValues("")]
        public string DownloadingStatus { get; set; } = string.Empty;
        [DeniedValues("")]
        public string FilePath { get; set; } = string.Empty;
        public DateTime DeletionDate { get; set; } = DateTime.Now;
    }
}
