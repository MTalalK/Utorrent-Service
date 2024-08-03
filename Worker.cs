using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkerServiceNew.Models;

namespace WorkerServiceNew
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ApiClient _apiClient;
        private TorrentContext _dbContext;

        public Worker(ILogger<Worker> logger, IConfiguration config, TorrentContext dbContext)
        {
            _logger = logger;
            _apiClient = new ApiClient(logger, config);
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpResponseMessage response = await GetTorrents();
                int serviceDelay = 60000;
                if (response is not null)
                {
                    var torrentsObject = await response.Content.ReadFromJsonAsync<Response>();

                    if (torrentsObject is not null)
                    {
                        foreach (List<object> torrentData in torrentsObject.Torrents)
                        {
                            if (torrentData is not null && torrentData.Count > 0)
                            {
                                Torrent torrent = new Torrent()
                                {
                                    Hash = Convert.ToString(torrentData[0]) ?? string.Empty,
                                    Name = Convert.ToString(torrentData[2]) ?? string.Empty,
                                    OriginalBytes = Convert.ToInt64(torrentData[3].ToString()),
                                    DownloadedBytes = Convert.ToInt64(torrentData[5].ToString()),
                                    CompletionStatus = Convert.ToInt32(torrentData[4].ToString()),
                                    DownloadingStatus = Convert.ToString(torrentData[21]) ?? string.Empty,
                                    FilePath = Convert.ToString(torrentData[26]) ?? string.Empty,
                                };
                                if (torrent.CompletionStatus == 1000 && torrent.DownloadingStatus.Contains("Seeding"))
                                {
                                    _dbContext.Torrents.Add(torrent);
                                    _dbContext.SaveChanges();
                                    await DeleteTorrent(torrent.Hash);
                                    break;
                                }
                            }
                            else
                            {
                                serviceDelay = 36000000;
                            }
                        }
                    }
                }
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(serviceDelay, stoppingToken);
            }
        }

        protected async Task<HttpResponseMessage> GetTorrents()
        {
            HttpResponseMessage response = await _apiClient.FetchData("&list=1");
            return response;
        }

        protected async Task<HttpResponseMessage> DeleteTorrent(string hash)
        {
            HttpResponseMessage response = await _apiClient.DeleteTorrent("&action=remove&hash=" + hash);
            return response;
        }
    }
}