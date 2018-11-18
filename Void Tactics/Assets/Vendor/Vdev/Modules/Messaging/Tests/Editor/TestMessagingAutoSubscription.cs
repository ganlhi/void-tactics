using System;
using System.Collections.Generic;
using UnityEngine;

using NUnit.Framework;

using Vdev.Utils;

namespace Vdev.Messaging.Tests {

[TestFixture(Author = "Vdev", Category = "Messaging")]
public class TestMessagingAutoSubscription
{

	[MessageDescription(HandlerRequirement.Required)]
	sealed class NotificationMessage1 : Message<NotificationMessage1, string>
	{}


	[MessageDescription(HandlerRequirement.Required)]
	sealed class NotificationMessage2 : Message<NotificationMessage2, string, int>
	{}


	class HandlerObject
	{
		public string StrValue {get; private set;}

		public int IntValue {get; private set;}

		public HandlerObject()
		{
			StrValue = string.Empty;

			IntValue = 0;
		}

		[MessageHandler(typeof(NotificationMessage1))]
		void OnMessage1(string value)
		{
			StrValue = value;
		}

		[MessageHandler(typeof(NotificationMessage2), AllowPartialParameters = true)]
		void OnMessage2(string value)
		{
			StrValue = value;
		}

		[MessageHandler(typeof(NotificationMessage2), AllowPartialParameters = false)]
		void OnMessage22(string str, int i)
		{
			IntValue = i;
		}
	}


	class HandlerObject2
	{
		[MessageHandler(typeof(NotificationMessage2), AllowPartialParameters = false)]
		void OnMessage2(string value)
		{
		}
	}


	/// <summary>
	/// 
	/// </summary>
	[Test]
	public void TestAutoSubscription()
	{
		var testHub = new MessagesHub();

		var instance = new HandlerObject();

		var subscriptions = MessagesUtils.AutoSubscribe(instance, testHub);

		using (Disposable.FromAction(() =>
		{
			foreach (var s in subscriptions)
			{
				s.Subscription.Dispose();
			}
		}))
		{
			string str1 = "value1";
			string str2 = "value2";
			int ival1 = 77;

			Assert.AreEqual(instance.StrValue, string.Empty);
			Assert.AreEqual(instance.IntValue, 0);

			NotificationMessage1.Broadcast(str1, testHub);

			Assert.AreEqual(instance.StrValue, str1);

			NotificationMessage2.Broadcast(str2, ival1, testHub);

			Assert.AreEqual(instance.StrValue, str2);
			Assert.AreEqual(instance.IntValue, ival1);
		}

		Assert.Throws<Exception>(() =>
		{// all subscriptions must gone after using (...) block
			NotificationMessage1.Broadcast(string.Empty, testHub);
		});
	}

	/// <summary>
	/// 
	/// </summary>
	[Test]
	public void TestPartialParametersRestriction()
	{
		var testHub = new MessagesHub();

		Assert.Throws<MessagesUtils.MessageHandlerSignatureException>(() =>
		{
			var instance = new HandlerObject2();

			MessagesUtils.AutoSubscribe(instance, testHub);
		});


	}
}


}
