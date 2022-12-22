using System;
using NUnit.Framework;

namespace Gstc.Utility.UnitTest.Event.Test;

[TestFixture]
public class AssertEventTest {
    public MockEventClass MockEventObject { get; set; } = new();

    [SetUp]
    public void Setup() => MockEventObject = new();

    #region Event Number of Calls
    [Test, Description("Tests number of calls. Is successful.")]
    public void Success_numberOfCalls() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);
        //Asserts
        Assert.True(event1Test.TestTimesCalled(2), event1Test.ErrorMessages);
    }

    //Test equals if null on Sender
    [Test, Description("Tests number of calls. Fails because 1 calls instead of expected 3 calls.")]
    public void Fail_numberOfCalls_Low() {
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));
        MockEventObject.TriggerUserEvent1(10);
        Assert.False(event1Test.TestTimesCalled(2), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 3 calls instead of expected 2 calls.")]
    public void Fail_numberOfCalls_High() {
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);
        MockEventObject.TriggerUserEvent1(30);
        Assert.False(event1Test.TestTimesCalled(2), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }
    #endregion

    #region Callbacks
    [Test, Description("Tests callbacks for assertion. Succeeds.")]
    public void Success_AssertCallbacks() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            description: "Tests parameter are as expected.",
            callback: (sender, e) => {
                Assert.That(sender, Is.EqualTo(MockEventObject));
                Assert.That(e.Number >= 0);
            });

        event1Test.AddCallback(
            invokeCallOrder: 2,
            description: "Tests event args on the third call.",
            callback: (_, e) => Assert.That(e.Number, Is.EqualTo(30))
        );
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);
        MockEventObject.TriggerUserEvent1(30);
        //Asserts
        Assert.True(event1Test.TestAllCallbacksInvoked(), event1Test.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because invoke order callback not triggered.")]
    public void Fail_AssertCallbacks_InvokeCallOrderNotReached_NoCallbackInvoked() {
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            description: "This description should appear in the error results to let us know which callback failed.",
            invokeCallOrder: 2,
            callback: (_, _) => throw new InvalidOperationException()
            );

        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);
        Assert.False(event1Test.TestAllCallbacksInvoked(), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because no callback not triggered.")]
    public void Fail_AssertCallbacks_NoCallbackInvoked() {
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback((_, _) => throw new InvalidOperationException());
        //No Act
        Assert.False(event1Test.TestAllCallbacksInvoked(), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }

    #endregion

    #region All
    [Test, Description("Tests callbacks for assertion. Succeeds.")]
    public void Success_AssertAll() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            invokeCallOrder: 1,
            description: "Tests event args on the second call.",
            callback: (_, e) => Assert.That(e.Number, Is.EqualTo(20))
        );
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);

        //Asserts
        Assert.True(event1Test.TestAll(2), event1Test.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because callback was not triggered.")]
    public void Fail_AssertAll_Callback() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            invokeCallOrder: 2,
            description: "Tests event args on the fourth call.",
            callback: (_, _) => throw new InvalidOperationException()
        );
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);

        //Asserts
        Assert.False(event1Test.TestAll(2), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion.Fails because times called did not match expected times called.")]
    public void Fail_AssertAll_Count() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            invokeCallOrder: 1,
            description: "Tests event args on the fourth call.",
            callback: (_, e) => Assert.That(e.Number, Is.EqualTo(20))
        );
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);

        //Asserts
        Assert.False(event1Test.TestAll(3), event1Test.ErrorMessages);
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion.Fails because times called did not match expected times called.")]
    public void Fail_AssertAll_CountAndCallback() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));

        event1Test.AddCallback(
            invokeCallOrder: 3,
            description: "Tests event args on the fourth call.",
            callback: (_, _) => throw new InvalidOperationException()
        );
        //Act
        MockEventObject.TriggerUserEvent1(10);
        MockEventObject.TriggerUserEvent1(20);

        //Asserts
        Assert.False(event1Test.TestAll(3), event1Test.ErrorMessages);
        Assert.That(event1Test.ErrorLog.Count, Is.EqualTo(2));
        Console.WriteLine("Error log: " + event1Test.ErrorMessages);
    }
    #endregion

    #region reset triggers

    [Test, Description("Tests if the reset on trigger is working properly.")]
    public void Success_Reset_Count() {
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));
        event1Test.IsResetCountOnAssert = true;

        MockEventObject.TriggerUserEvent1(10);

        Assert.True(event1Test.TestTimesCalled(1));
        Assert.True(event1Test.TestTimesCalled(0));

        event1Test.IsResetCountOnAssert = false;

        MockEventObject.TriggerUserEvent1(10);

        Assert.True(event1Test.TestTimesCalled(1));
        Assert.True(event1Test.TestTimesCalled(1));

    }

    [Test, Description("Tests if the reset on trigger is working properly.")]
    public void Success_Reset_Callback() {
        //Arrange
        //Arrange
        var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1));
        event1Test.AddCallback(callback: (_, _) => { });
        event1Test.IsResetCountOnAssert = true;

        MockEventObject.TriggerUserEvent1(10);

        Assert.True(event1Test.TestAllCallbacksInvoked());
        Assert.False(event1Test.TestAllCallbacksInvoked());

        event1Test.ErrorLog.Clear();

        event1Test.IsResetCountOnAssert = false;

        MockEventObject.TriggerUserEvent1(10);

        Assert.True(event1Test.TestAllCallbacksInvoked());
        Assert.True(event1Test.TestAllCallbacksInvoked());
    }

    #endregion

    #region dispose
    [Test, Description("Test that callbacks are unassigned from the test object.")]
    public void Success_Dispose() {
        //Arrange
        MockEventObject.UserEvent1 += (_, _) => { }; //Ensure invocationList is not null.
        using (var event1Test = new AssertEvent<Event1EventArgs>(MockEventObject, nameof(MockEventObject.UserEvent1))) {
            Assert.That(MockEventObject.UserEvent1NumberOfCallbacks, Is.EqualTo(2));
            MockEventObject.TriggerUserEvent1(10);
            Assert.True(event1Test.TestAll(1));
        }
        Assert.That(MockEventObject.UserEvent1NumberOfCallbacks, Is.EqualTo(1));
    }
    #endregion
}
