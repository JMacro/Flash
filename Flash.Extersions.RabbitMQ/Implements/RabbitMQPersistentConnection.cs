using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace Flash.Extersions.RabbitMQ
{
    /// <summary>
    /// RabbitMQ持久化连接
    /// </summary>
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<IRabbitMQPersistentConnection> _logger;
        private readonly int _retryCount;
        private IConnection _connection;
        private IModel _model;
        private bool _disposed;
        private object _async = new object();

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<IRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (retryCount < 0)
            {
                throw new ArgumentException("RetryCount not allowed to be negative");
            }
            this._retryCount = retryCount;
        }


        public bool IsConnected => (_connection != null && _connection.IsOpen && !_disposed);

        public IModel CreateModel()
        {
            if (!IsConnected) throw new InvalidOperationException("RabbitMQ connections is not establish connection");
            return _connection.CreateModel();
        }

        public IModel GetModel()
        {
            if (!IsConnected) throw new InvalidOperationException("RabbitMQ connections is not establish connection");
            if (this._model == null)
            {
                lock (_async)
                {
                    if (this._model == null)
                    {
                        _model = CreateModel();
                        _model.ConfirmSelect();
                    }
                }
            }
            return _model;
        }

        public bool TryConnect()
        {
            if (!this.IsConnected)
            {
                lock (_async)
                {
                    if (!this.IsConnected)
                    {
                        var policy = RetryPolicy.Handle<SocketException>()
                            .Or<BrokerUnreachableException>()
                            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                            {
                                _logger.LogWarning(ex.ToString());
                            }
                        );

                        policy.Execute(() =>
                        {
                            _connection = _connectionFactory.CreateConnection();
                        });

                        if (IsConnected)
                        {
                            _connection.ConnectionShutdown += OnConnectionShutdown;
                            _connection.CallbackException += OnCallbackException;
                            _connection.ConnectionBlocked += OnConnectionBlocked;
                            return true;
                        }
                        else
                        {
                            _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void Dispose()
        {
            if (this._disposed) return;

            this._disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
