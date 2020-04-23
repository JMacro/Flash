using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extersions.RabbitMQ
{
    public enum MessageState
    {
        NotPublished = 0,
        Published = 1,
        PublishedFailed = 2
    }
}
