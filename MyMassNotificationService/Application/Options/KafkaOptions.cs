﻿namespace MyMassNotificationService.Application.Options
{
    public class KafkaOptions
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
        public string GroupId { get; set; }
    }
}
