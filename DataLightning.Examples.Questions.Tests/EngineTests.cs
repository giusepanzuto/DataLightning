//using System;
//using DataLightning.Examples.Questions.Model;
//using FluentAssertions;
//using Moq;
//using Orleans.Configuration;
//using Orleans.Hosting;
//using System.Collections.Generic;
//using System.Net;
//using Orleans;
//using Xunit;

//namespace DataLightning.Examples.Questions.Tests
//{
//    public class EngineTests
//    {
//        private readonly Mock<IQaApiPublisher> _outputWriterMock;
//        private readonly Engine _sut;

//        private QaApiContent _lastResult;

//        //public EngineTests()
//        //{
//        //    _outputWriterMock = new Mock<IQaApiPublisher>();
//        //    _outputWriterMock.Setup(o => o.Publish(It.IsAny<QaApiContent>()))
//        //        .Callback<QaApiContent>(content => _lastResult = content);

//        //    _sut = new Engine(_outputWriterMock.Object);
//        //}

//        public EngineTests()
//        {
//            var siloBuilder = new SiloHostBuilder()
//                .UseLocalhostClustering()
//                .Configure<ClusterOptions>(options =>
//                {
//                    options.ClusterId = "dev";
//                    options.ServiceId = "Orleans2GettingStarted";
//                })
//                .Configure<EndpointOptions>(options =>
//                    options.AdvertisedIPAddress = IPAddress.Loopback)
//                /*.ConfigureLogging(logging => logging.AddConsole())*/;

//            using (var host = siloBuilder.Build())
//            {
//                host.StartAsync().GetAwaiter().GetResult();

//                var clientBuilder = new ClientBuilder()
//                  .UseLocalhostClustering()
//                  .Configure<ClusterOptions>(options =>
//                  {
//                      options.ClusterId = "dev";
//                      options.ServiceId = "Orleans2GettingStarted";
//                  })
//                  .ConfigureLogging(logging => logging.AddConsole());

//                using (var client = clientBuilder.Build())
//                {
//                    await client.Connect();

//                    var random = new Random();
//                    string sky = "blue";

//                    while (sky == "blue") // if run in Ireland, it exits loop immediately
//                    {
//                        int grainId = random.Next(0, 500);
//                        double temperature = random.NextDouble() * 40;
//                        var sensor = client.GetGrain<ITemperatureSensorGrain>(grainId);
//                        await sensor.SubmitTemperatureAsync((float)temperature);
//                    }
//                }
//            }

//        }

//        [Fact]
//        public void ShouldReturnTheRightId()
//        {
//            var qId = _sut.AddQuestion(0, "How to build a solution?");

//            _sut.AddAnswer(qId, 0, "Press F6");
//            _sut.AddAnswer(qId, 0, "Right click on solution and then click Build.");

//            _lastResult.QuestionId.Should().BeEquivalentTo(qId.ToString());
//        }

//        [Fact]
//        public void ShouldReturnTheRightContent()
//        {
//            var qId = _sut.AddQuestion(0, "How to build a solution?");

//            _sut.AddAnswer(qId, 0, "Press F6");
//            _sut.AddAnswer(qId, 0, "Right click on solution and then click Build.");

//            var expected = new QaApiContent
//            {
//                QuestionId = qId.ToString(),
//                Question = "How to build a solution?",
//                Answers = new List<string>{
//                    "Press F6",
//                    "Right click on solution and then click Build."
//                }
//            };

//            _lastResult.Should().BeEquivalentTo(expected);
//        }
//    }
//}