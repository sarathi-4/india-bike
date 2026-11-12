using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using NUnit.Framework;

namespace IndianBikeLife.Tests.Editor
{
    public class ServiceLocatorTests
    {
        private class MockGameService : IGameService
        {
            public bool IsInitialized { get; private set; }
            public void Initialize() => IsInitialized = true;
            public void Shutdown() => IsInitialized = false;
        }

        [SetUp]
        public void Setup()
        {
            ServiceLocator.Reset();
        }

        [TearDown]
        public void Teardown()
        {
            ServiceLocator.Reset();
        }

        [Test]
        public void RegisterAndGet_ReturnsRegisteredService()
        {
            MockGameService mock = new MockGameService();
            ServiceLocator.Register<IGameService>(mock);

            IGameService retrieved = ServiceLocator.Get<IGameService>();
            Assert.AreSame(mock, retrieved);
        }

        [Test]
        public void Get_UnregisteredService_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<IGameService>());
        }

        [Test]
        public void TryGet_ReturnsFalseWhenNotRegistered()
        {
            bool found = ServiceLocator.TryGet(out IGameService service);
            Assert.IsFalse(found);
            Assert.IsNull(service);
        }

        [Test]
        public void Reset_ClearsAllServices()
        {
            MockGameService mock = new MockGameService();
            mock.Initialize();
            ServiceLocator.Register<IGameService>(mock);

            ServiceLocator.Reset();

            Assert.IsFalse(ServiceLocator.TryGet(out IGameService _));
            Assert.IsFalse(mock.IsInitialized);
        }
    }
}
