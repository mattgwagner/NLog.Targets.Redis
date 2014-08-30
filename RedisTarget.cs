using StackExchange.Redis;
using System;
using System.ComponentModel.DataAnnotations;

namespace NLog.Targets.Redis
{
    [Target("Redis")]
    public class RedisTarget : TargetWithLayout
    {
        private readonly Lazy<RedisConnection> _redis;

        public String Host { get; set; }

        public int Port { get; set; }

        public int Db { get; set; }

        public String Password { get; set; }

        [Required]
        public String Key { get; set; }

        private string _dataType;

        public String DataType
        {
            get { return this._dataType; }
            set { this._dataType = value.ToLower(); }
        }

        public RedisTarget()
        {
            this.Host = "localhost";
            this.Port = 6379;
            this.Db = 0;
            this.Password = String.Empty;

            this.DataType = "list";

            this._redis = new Lazy<RedisConnection>(() =>
            {
                return new RedisConnection(this.Host, this.Port, this.Db, this.Password);
            });
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if ("list" == this._dataType)
            {
                this._redis.Value.GetClient().ListLeftPushAsync(this.Key, Layout.Render(logEvent));
            }
            else if ("channel" == this._dataType)
            {
                this._redis.Value.GetClient().PublishAsync(this.Key, Layout.Render(logEvent));
            }
            else
            {
                throw new InvalidOperationException("Invalid DataType for Redis NLog Target");
            }
        }
    }

    public class RedisConnection
    {
        // Note: I'm writing this for redis without clustering

        private readonly ConnectionMultiplexer _connectionManager;

        private readonly String _host;
        private readonly int _port;
        private readonly int _db;

        public RedisConnection(string host = "localhost", int port = 6379, int db = 0, string password = null)
        {
            this._host = host;
            this._port = port;
            this._db = db;

            var options = new ConfigurationOptions
            {
                Password = password,
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ConnectTimeout = 1000,
                KeepAlive = 10
            };

            options.EndPoints.Add(this._host, this._port);

            this._connectionManager = ConnectionMultiplexer.Connect(options);
        }

        public RedisConnection(ConnectionMultiplexer connectionMultiplexer)
        {
            this._connectionManager = connectionMultiplexer;
        }

        public IDatabase GetClient()
        {
            return this._connectionManager.GetDatabase(this._db);
        }
    }
}