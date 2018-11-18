using System;
using System.Collections.Generic;
using UnityEngine;

using NUnit.Framework;

namespace Vdev.Messaging.Tests {


[TestFixture(Author = "Vdev", Category = "Messaging")]
//[Category("Messaging")]
public class TestMessagingBasic
{

	[MessageDescription(HandlerRequirement.NotRequired)]
	class DefaultTestMessage : Message<DefaultTestMessage, int>
	{}


	[MessageDescription(HandlerRequirement.Required)]
	class HandlerRequiredTestMessage : Message<HandlerRequiredTestMessage, int>
	{}



	[Test]
	public static void SubscribeUnsubscribe()
	{
		var testHub = new MessagesHub();

		int counter = 0;

		int testValue = 10;

		using (var subscription = DefaultTestMessage.Subscribe(value =>
		{
			counter = value;

		}, null, testHub))
		{
			DefaultTestMessage.Broadcast(testValue, testHub);

			Assert.AreEqual(counter, testValue);

			DefaultTestMessage.Broadcast(0, testHub);

			Assert.AreEqual(counter, 0);
		}

		DefaultTestMessage.Broadcast(testValue, testHub);

		Assert.AreEqual(counter, 0);
	}


	[Test]
	public static void UnsubscribeWithinHandler()
	{
		var testHub = new MessagesHub();

		int counter = 0;

		ISubscription subscription2 = null;

		var subscription1 = DefaultTestMessage.Subscribe(value =>
		{
			counter += value;

			subscription2.Dispose();

		}, null, testHub);
		
		
		subscription2 = DefaultTestMessage.Subscribe(value =>
		{
			counter += value;

			subscription1.Dispose();

		}, null, testHub);


		DefaultTestMessage.Subscribe(value =>
		{
			counter += value;

		}, null, testHub);


		DefaultTestMessage.Broadcast(1, testHub);

		Assert.AreEqual(counter, 2);

		DefaultTestMessage.Broadcast(1, testHub);

		Assert.AreEqual(counter, 4);
	}

	[Test]
	public static void DisableSubscriber()
	{
		var testHub = new MessagesHub();

		int counter = 0;

		DefaultTestMessage.Subscribe(value =>
		{
			counter += value;

		}, null, testHub);
		
		
		var subscription2 = DefaultTestMessage.Subscribe(value =>
		{
			counter += value;

		}, null, testHub);

		DefaultTestMessage.Broadcast(1, testHub);
		Assert.AreEqual(counter, 2);

		subscription2.Enabled = false;

		DefaultTestMessage.Broadcast(1, testHub);
		Assert.AreEqual(counter, 3);

		subscription2.Enabled = true;

		DefaultTestMessage.Broadcast(1, testHub);
		Assert.AreEqual(counter, 5);
	}


	[Test]
	public void RequiredHandler()
	{
		var emptyHub = new MessagesHub();

		var testHub = new MessagesHub();

		var subscription = HandlerRequiredTestMessage.Subscribe(value => 
		{},
		null, testHub);

		HandlerRequiredTestMessage.Broadcast(1, testHub);

		subscription.Enabled = false;

		Assert.Throws<Exception>(() =>
		{
			HandlerRequiredTestMessage.Broadcast(1, testHub);
		});


		subscription.Enabled = true;

		Assert.DoesNotThrow(() =>
		{
			HandlerRequiredTestMessage.Broadcast(1, testHub);
		});

		Assert.Throws<Exception>(() =>
		{
			HandlerRequiredTestMessage.Broadcast(1, emptyHub);
		});
	}
}


}
