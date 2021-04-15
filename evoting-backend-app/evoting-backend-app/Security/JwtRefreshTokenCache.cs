using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace evoting_backend_app.Security
{
    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly IJwtAuthManager jwtAuthManager;

        public JwtRefreshTokenCache(IJwtAuthManager jwtAuthManager)
        {
            this.jwtAuthManager = jwtAuthManager;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            // remove expired refresh tokens from cache every minute
            this.timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            this.jwtAuthManager.RemoveExpiredRefreshTokens(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            this.timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.timer?.Dispose();
        }
    }
}
