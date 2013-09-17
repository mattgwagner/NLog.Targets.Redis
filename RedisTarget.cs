namespace NLog.Targets.Redis
{
    using ServiceStack.Redis;
    using System;
    using System.ComponentModel.DataAnnotations;

    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        public String Host { get; set; }

        public int Port { get; set; }

        private volatile RedisClient redisClient;

        [Required]
        public string Key { get; set; }

        public RedisTarget()
        {
            this.Host = "localhost";
            this.Port = 6379;
        }

        protected RedisClient RedisClient
        {
            get
            {
                if (redisClient == null)
                {
                    lock (SyncRoot)
                    {
                        if (redisClient == null)
                        {
                            redisClient = new RedisClient(this.Host, this.Port);
                        }
                    }
                }

                return redisClient;
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            this.RedisClient.PushItemToList(this.Key, Layout.Render(logEvent));
        }
    }
}