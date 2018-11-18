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

        public EntityProcessingLockTests()
        {
            _environment = new DockerEnvironmentBuilder()
                .AddContainer("zk-test", "zookeeper")
                .Build();

            _environment.Up().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _environment.Down().GetAwaiter().GetResult();
            _environment.Dispose();
        }

        [Fact]
        public async Task ObtainLock()
        {
            var sut = new EntityProcessingLock("127.0.0.1:2181");

            var result = await sut.LockAsync<string>(2);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task FailLockOnAlreadyLockedEntity()
        {
            var sut = new EntityProcessingLock("127.0.0.1:2181");

            await sut.LockAsync<string>(2);
            
            var result = await sut.LockAsync<string>(2);

            result.Should().BeFalse();
        }

    }
}