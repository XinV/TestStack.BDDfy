using System.Collections.Generic;
using Bddify.Core;
using NUnit.Framework;
using System.Linq;

namespace Bddify.Tests.BddifySpecs.RunScenarioWithArgs
{
    // This is to test that in RunScenarioWithArgs scenarios, one object per scenario is created so that changed object state
    // in one scenario does not cause another scenario to fail
    public class WhenScenarioIsDecoratedToRunWithArgs
    {
        private List<Scenario> _scenarios;

        enum ArgsNumber
        {
            ArgSet1,
            ArgSet2,
            ArgSet3,
        }

        [RunScenarioWithArgs(ArgsNumber.ArgSet1, 1, 2, 3)]
        [RunScenarioWithArgs(ArgsNumber.ArgSet2, 4, 5, 9)]
        private class ScenarioWithArgs
        {
            private int _expectedResult;
            private int _input1;
            private int _input2;
            private ArgsNumber _argsNumber;
            private int _result;

            public int Input1
            {
                get { return _input1; }
            }

            public int Input2
            {
                get { return _input2; }
            }

            public ArgsNumber ArgsNumber
            {
                get { return _argsNumber; }
            }

            public int ExpectedResult
            {
                get { return _expectedResult; }
            }

            private void RunScenarioWithArgs(ArgsNumber argsNumber, int input1, int input2, int expectedResult)
            {
                _input1 = input1;
                _input2 = input2;
                _expectedResult = expectedResult;
                _argsNumber = argsNumber;
            }

            private void WhenResultIsAccumulative()
            {
                _result += _input1 + _input2;
            }

            private void ThenTheResultMatchesExpectedResultBecauseEachScenarioTakesAUniqueObject()
            {
                Assert.That(_result, Is.EqualTo(_expectedResult));
            }
        }

        [SetUp]
        public void Setup()
        {
            var testObject = new ScenarioWithArgs();
            var bddify = testObject.LazyBddify();
            bddify.Run();
            _scenarios = bddify.Scenarios.ToList();
        }

        [Test]
        public void ThenOneScenarioIsRunPerAttribute()
        {
            Assert.That(_scenarios.Count, Is.EqualTo(2));
        }

        [Test]
        public void ThenEachScenarioTakesAUniqueTestObjectInstance()
        {
            Assert.IsFalse(object.ReferenceEquals(_scenarios[0].Object, _scenarios[1].Object));
        }
    }
}