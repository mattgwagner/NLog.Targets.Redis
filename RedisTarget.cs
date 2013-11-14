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
        private string dataType;

        [Required]
        public string Key { get; set; }

        [Required]
        public string DataType 
        {   
            get { return this.dataType; }
            set { this.dataType = value.ToLower(); } 
        }

        public RedisTarget()
        {
            this.Host = "localhost";
            this.Port = 6379;
            this.DataType = "list";
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
            if (dataType.Equals("list"))
                this.RedisClient.PushItemToList(this.Key, Layout.Render(logEvent));
            else if (dataType.Equals("channel"))
                this.redisClient.PublishMessage(this.Key, Layout.Render(logEvent));
            else
                throw new InvalidOperationException("Invalid type for redis operation: " + this.dataType);
        }
    }
}