using DataLightning.Examples.Questions.Gateway.Lock;
using org.apache.zookeeper;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using TestEnvironment.Docker;
using Xunit;

namespace DataLightning.Examples.Questions.Gateway.Tests.Lock
{
    public class ZookeeperContainerTests
    {
        [Fact]
        public async Task ConnectToContainer()
        {
            using (var environment = new DockerEnvironmentBuilder().AddContainer("zk-test", "zookeeper").Build())
            {
                await environment.Up();

                var zk = environment.GetContainer("zk-test");

                await Task.Delay(TimeSpan.FromSeconds(1));

                var connectstring = $"127.0.0.1:{zk.Ports[2181]}";

                var zookeeper = new ZooKeeper(connectstring, 2000, new ZooKeeperWatcher());

                await zookeeper.createAsync(
                    $"/ConnectToContainer",
                    null,
                    ZooDefs.Ids.READ_ACL_UNSAFE,
                    CreateMode.EPHEMERAL_SEQUENTIAL);

                await environment.Down();
            }
        }

        //[Fact]
        //public async Task ConnectToContainerManually()
        //{
        //    var zookeeper = new ZooKeeper($"127.0.0.1:32863", 2000, new ZooKeeperWatcher());

        //    await zookeeper.createAsync(
        //        $"/ConnectToContainerManually",
        //        null,
        //        ZooDefs.Ids.READ_ACL_UNSAFE,
        //        CreateMode.EPHEMERAL_SEQUENTIAL);
        //}
    }
}