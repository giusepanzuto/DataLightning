using DataLightning.Examples.Questions.Gateway.Lock;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using TestEnvironment.Docker;
using Xunit;

namespace DataLightning.Examples.Questions.Gateway.Tests.Lock
{
    public class EntityProcessingLockTests : IDisposable
    {
        private readonly DockerEnvironment _environment;
        private readonly EntityProcessingLock _sut;

        public EntityProcessingLockTests()
        {
            _environment = new DockerEnvironmentBuilder()
                .AddContainer("zk-test", "zookeeper")
                .Build();

            _environment.Up().GetAwaiter().GetResult();

            var zk = _environment.GetContainer("zk-test");

            Task.Delay(TimeSpan.FromMilliseconds(1000)).GetAwaiter().GetResult();

            _sut = new EntityProcessingLock($"127.0.0.1:{zk.Ports[2181]}");
        }

        public void Dispose()
        {
            _environment.Down().GetAwaiter().GetResult();
            _environment.Dispose();
        }

        [Fact]
        public async Task ObtainLock()
        {
            var result = await _sut.LockAsync<string>(2);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task FailLockOnAlreadyLockedEntity()
        {
            await _sut.LockAsync<string>(2);
            
            var result = await _sut.LockAsync<string>(2);

            result.Should().BeFalse();
        }

    }
}