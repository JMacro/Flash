using System;

namespace Flash.Extersions.EventBus
{
    public static class MessageCarrierExtersions
    {
        /// <summary>
        /// 重试，（有等待时间，有重试次数限制）
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static MessageCarrier WaitAndRetry(this MessageResponse response, Func<int, int> retryAttempt, int maxRetries)
        {
            var @event = MessageCarrier.Clone(response);
            var numberOfRetries = response.GetNumberOfRetries();
            var TTL = retryAttempt(numberOfRetries);

            //当前重试次数小于最大重试次数
            if (numberOfRetries < maxRetries)
            {
                @event.WithWait(TTL);
                @event.WithRetry(maxRetries, ++numberOfRetries);
            }
            else
            {
                @event.WithNoRetry();
            }

            return @event;
        }

        public static int GetNumberOfRetries(this MessageResponse response)
        {
            var numberOfRetries = 0;
            if (response.Headers.ContainsKey("x-message-retries"))
            {
                numberOfRetries = response.Headers["x-message-retries"].ToString().ToInt();
            }
            return numberOfRetries;
        }

        /// <summary>
        /// 设置延时策略
        /// </summary>
        /// <param name="event"></param>
        /// <param name="TTL">延时时间（秒）</param>
        /// <returns></returns>
        public static void WithWait(this MessageCarrier @event, int TTL)
        {
            @event.Headers["x-first-death-queue"] = $"{@event.RouteKey}@Delay#{TTL}"; //死信队列名称
            @event.Headers["x-message-ttl"] = TTL * 1000; //当一个消息被推送在该队列的时候 可以存在的时间 单位为ms，应小于队列过期时间  
            @event.Headers["x-dead-letter-exchange"] = @event.Headers["x-exchange"];//过期消息转向路由  
            @event.Headers["x-dead-letter-routing-key"] = @event.RouteKey;//过期消息转向路由相匹配routingkey 
        }

        /// <summary>
        /// 设置重试策略
        /// </summary>
        /// <param name="event"></param>
        /// <param name="MaxRetries">最大重试次数</param>
        /// <param name="NumberOfRetries">当前重试次数</param>
        /// <returns></returns>
        public static void WithRetry(this MessageCarrier @event, int MaxRetries, int NumberOfRetries)
        {
            @event.Headers["x-message-max-retries"] = MaxRetries;
            @event.Headers["x-message-retries"] = NumberOfRetries;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <param name="expires">消息过期时间</param>
        public static void WithNoRetry(this MessageCarrier @event)
        {
            @event.Headers["x-first-death-queue"] = $"{@event.RouteKey}@Failed"; //死信队列名称
            @event.Headers.Remove("x-message-ttl");
            @event.Headers["x-dead-letter-exchange"] = @event.Headers["x-exchange"];//过期消息转向路由  
            @event.Headers["x-dead-letter-routing-key"] = @event.RouteKey;//过期消息转向路由相匹配routingkey 
        }
    }
}
