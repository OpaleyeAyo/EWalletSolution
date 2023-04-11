using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Utility.Configurations
{
    public class RabbitMQSettings
    {
        public string Host { get; set; }

        public string Port { get; set; }

        public string ExchangeString { get; set; }
    }
}
