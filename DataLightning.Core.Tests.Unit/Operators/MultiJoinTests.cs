using DataLightning.Core.Operators;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace DataLightning.Core.Tests.Unit.Operators
{
    public class MultiJoinTests
    {
        private readonly MultiJoin _sut;
        private readonly PassThroughUnit<TestEntityA> _inputA;
        private readonly PassThroughUnit<TestEntityB> _inputB;
        private readonly PassThroughUnit<TestEntityC> _inputC;

        private IDictionary<string, IList<object>> _result = null;

        public MultiJoinTests()
        {
            _inputA = new PassThroughUnit<TestEntityA>();
            _inputB = new PassThroughUnit<TestEntityB>();
            _inputC = new PassThroughUnit<TestEntityC>();

            _sut = new MultiJoin(
                new JoinDefinitionAdapter<TestEntityA>(_inputA, "A", e => e.KeyA),
                new JoinDefinitionAdapter<TestEntityB>(_inputB, "B", e => e.KeyB),
                new JoinDefinitionAdapter<TestEntityC>(_inputC, "C", e => e.KeyC));

            var resultInspector = new CallbackSubcriber<IDictionary<string, IList<object>>>(r => _result = r);

            _sut.Subscribe(resultInspector);
        }

        [Fact]
        public void ShouldReturn2EntityMatch()
        {
            _inputA.Push(new TestEntityA { KeyA = 1, Value1 = "A1" });
            _inputB.Push(new TestEntityB { KeyB = 1, Value1 = "B1" });

            var expected = new Dictionary<string, IList<object>>
            {
                ["A"] = new List<object> { new TestEntityA { KeyA = 1, Value1 = "A1" } },
                ["B"] = new List<object> { new TestEntityB { KeyB = 1, Value1 = "B1" } },
            };

            _result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldReturn3EntityMatch()
        {
            _inputA.Push(new TestEntityA { KeyA = 1, Value1 = "A1" });
            _inputB.Push(new TestEntityB { KeyB = 1, Value1 = "B1" });
            _inputC.Push(new TestEntityC { KeyC = 1, Value1 = "C1" });

            var expected = new Dictionary<string, IList<object>>
            {
                ["A"] = new List<object> { new TestEntityA { KeyA = 1, Value1 = "A1" } },
                ["B"] = new List<object> { new TestEntityB { KeyB = 1, Value1 = "B1" } },
                ["C"] = new List<object> { new TestEntityC { KeyC = 1, Value1 = "C1" } },
            };

            _result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldDiscardNoMatchingEntities()
        {
            _inputC.Push(new TestEntityC { KeyC = 2, Value1 = "C1" });

            _inputA.Push(new TestEntityA { KeyA = 1, Value1 = "A1" });
            _inputB.Push(new TestEntityB { KeyB = 1, Value1 = "B1" });

            var expected = new Dictionary<string, IList<object>>
            {
                ["A"] = new List<object> { new TestEntityA { KeyA = 1, Value1 = "A1" } },
                ["B"] = new List<object> { new TestEntityB { KeyB = 1, Value1 = "B1" } },
            };

            _result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldReturn3EntityMatchAndDiscardNoMatchingEntities()
        {
            _inputA.Push(new TestEntityA { KeyA = 2, Value1 = "A3" });
            _inputB.Push(new TestEntityB { KeyB = 2, Value1 = "B3" });
            _inputC.Push(new TestEntityC { KeyC = 2, Value1 = "C3" });

            _inputA.Push(new TestEntityA { KeyA = 1, Value1 = "A1" });
            _inputB.Push(new TestEntityB { KeyB = 1, Value1 = "B1" });
            _inputC.Push(new TestEntityC { KeyC = 1, Value1 = "C1" });

            _inputA.Push(new TestEntityA { KeyA = 1, Value1 = "A2" });
            _inputB.Push(new TestEntityB { KeyB = 1, Value1 = "B2" });
            _inputC.Push(new TestEntityC { KeyC = 1, Value1 = "C2" });

            var expected = new Dictionary<string, IList<object>>
            {
                ["A"] = new List<object> {
                    new TestEntityA { KeyA = 1, Value1 = "A1" },
                    new TestEntityA { KeyA = 1, Value1 = "A2" }},
                ["B"] = new List<object> {
                    new TestEntityB { KeyB = 1, Value1 = "B1" },
                    new TestEntityB { KeyB = 1, Value1 = "B2" } },
                ["C"] = new List<object> {
                    new TestEntityC { KeyC = 1, Value1 = "C1" },
                    new TestEntityC { KeyC = 1, Value1 = "C2" }},
            };

            _result.Should().BeEquivalentTo(expected);
        }
    }
}