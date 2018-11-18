﻿using org.apache.zookeeper;
using System;
using System.Threading.Tasks;

namespace DataLightning.Examples.Questions.Gateway.Lock
{
    internal class ZooKeeperWatcher : Watcher
    {
        public override Task process(WatchedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}