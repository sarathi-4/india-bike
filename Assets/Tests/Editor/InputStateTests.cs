using IndianBikeLife.Core.Input;
using NUnit.Framework;

namespace IndianBikeLife.Tests.Editor
{
    public class InputStateTests
    {
        [Test]
        public void Reset_ClearsAllInputFields()
        {
            MotorcycleInputState state = new MotorcycleInputState
            {
                steerInput = 0.8f,
                throttleInput = 1.0f,
                brakeInput = 0.5f,
                nitro = true,
                horn = true,
                headlights = true,
                handbrake = true,
                pausePressed = true,
                changeCameraPressed = true
            };

            state.Reset();

            Assert.AreEqual(0f, state.steerInput);
            Assert.AreEqual(0f, state.throttleInput);
            Assert.AreEqual(0f, state.brakeInput);
            Assert.IsFalse(state.nitro);
            Assert.IsFalse(state.horn);
            Assert.IsFalse(state.headlights);
            Assert.IsFalse(state.handbrake);
            Assert.IsFalse(state.pausePressed);
            Assert.IsFalse(state.changeCameraPressed);
        }
    }
}

