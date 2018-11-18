using NUnit.Framework;
using System;

namespace Vdev.Messaging.Tests
{
    [TestFixture(Author = "Vdev", Category = "Messaging")]
    public class TestMessagingExceptions
    {
        private sealed class TestException : Exception
        {
            public TestException()
                : base()
            { }
        }

        private sealed class ThrowExceptionMessage : Message<ThrowExceptionMessage, bool>
        { }

        /// <summary>
        /// Test that broadcast throws exception of correct type.
        /// </summary>
        [Test]
        public void CatchException()
        {
            var testHub = new MessagesHub("test");

            Action<bool> throwTestException = _ =>
            {
                throw new TestException();
            };

            using (ThrowExceptionMessage.Subscribe(throwTestException, null, testHub))
            {
                Assert.Throws<TestException>(() =>
                {
                    ThrowExceptionMessage.Broadcast(true, testHub);
                });
            }
        }

        /// <summary>
        /// Test successfully broadcast after exception was throw.
        /// </summary>
        [Test]
        public void RecoveryAfterException()
        {
            var testHub = new MessagesHub("test");

            int counter = 0;

            Action<bool> neverThrow = needThrow =>
            {
                ++counter;
            };

            Action<bool> throwAction = needThrow =>
            {
                if (needThrow)
                {
                    throw new Exception("test");
                }

                ++counter;
            };

            using (var subscription0 = ThrowExceptionMessage.Subscribe(neverThrow, null, testHub))
            using (var subscription1 = ThrowExceptionMessage.Subscribe(throwAction, null, testHub))
            using (var subscription2 = ThrowExceptionMessage.Subscribe(neverThrow, null, testHub))
            {
                ThrowExceptionMessage.Broadcast(false, testHub);

                Assert.AreEqual(counter, 3);

                Assert.Throws<Exception>(() =>
                {
                    ThrowExceptionMessage.Broadcast(true, testHub);
                });

                Assert.AreEqual(counter, 4);

                ThrowExceptionMessage.Broadcast(false, testHub);

                Assert.AreEqual(counter, 7);
            }
        }
    }
}