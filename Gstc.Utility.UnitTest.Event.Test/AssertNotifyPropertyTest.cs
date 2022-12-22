using System;
using NUnit.Framework;

namespace Gstc.Utility.UnitTest.Event.Test;

[TestFixture]
public class AssertNotifyPropertyTest {

    public MockPropertyClass<int> MockPropertyObject { get; set; } = new();

    [SetUp]
    public void Setup() => MockPropertyObject = new();

    #region Property number of callbacks

    [Test, Description("Verifies that TestTimesCalled(...) succeeds.")]
    public void Success_AssertPropertyCount() {
        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 20;
        MockPropertyObject.MyProperty2 = 30;
        //Assert
        Assert.True(notifyTest.TestPropertyCalled(2, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Assert.True(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty2)), notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 2 calls instead of expected 1 calls.")]
    public void Fail_AssertPropertyCount_low() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 2 calls instead of expected 1 calls.")]
    public void Fail_AssertPropertyCount_High() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestPropertyCalled(3, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 0 calls instead of expected 2 calls.")]
    public void Fail_AssertPropertyCount_NotCalled() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        //No Act
        Assert.False(notifyTest.TestPropertyCalled(2, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    #endregion

    #region property callback invoked

    [Test, Description("Verifies that all callbacks were called.")]
    public void Success_AssertPropertyCallbacks() {

        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(
            propertyName: nameof(MockPropertyObject.MyProperty1),
            invokeOrder: 2,
            description: "MyProperty1 should be equal to 100 on 3rd call.",
            callback: () => Assert.That(MockPropertyObject.MyProperty1, Is.EqualTo(100))
        );

        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), () => { });

        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 50;
        MockPropertyObject.MyProperty1 = 100;
        MockPropertyObject.MyProperty2 = 0;

        //Assert
        Assert.True(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Assert.True(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty2)), notifyTest.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because invoke order callback not triggered.")]
    public void Fail_AssertPropertyCallbacks_CallOrderNotTriggered() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 2, () => throw new NotSupportedException());
        MockPropertyObject.MyProperty1 = 0;
        Assert.False(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because no callback not triggered.")]
    public void Fail_AssertPropertyCallbacks_NoCallbackTriggered() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 2, () => throw new NotSupportedException());
        //No Act
        Assert.False(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }
    #endregion

    #region Property callback and number of calls

    [Test, Description("Verifies that all callbacks were called and test number of calls succeeds.")]
    public void Success_AssertPropertyAll() {

        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(
            propertyName: nameof(MockPropertyObject.MyProperty1),
            invokeOrder: 1,
            description: "MyProperty1 should be equal to 100 on 3rd call.",
            callback: () => Assert.That(MockPropertyObject.MyProperty1, Is.EqualTo(100))
        );

        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), () => { });

        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 100;
        MockPropertyObject.MyProperty2 = 100;

        //Assert
        Assert.True(notifyTest.TestPropertyAll(2, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Assert.True(notifyTest.TestPropertyAll(1, nameof(MockPropertyObject.MyProperty2)), notifyTest.ErrorMessages);
    }

    [Test, Description("Fails because 1 calls instead of expected 2 calls.")]
    public void Fail_AssertPropertyAll_Count() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestPropertyAll(2, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Fails because callback was not triggered.")]
    public void Fail_AssertPropertyAll_Callback() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 1, () => { });
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestPropertyAll(1, nameof(MockPropertyObject.MyProperty1)), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    #endregion

    #region OnChanged Number of calls

    [Test, Description("Verifies that OnChanged was called the expected amount of times.")]
    public void Success_AssertOnChanged() {
        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty2 = 20;
        //Assert
        Assert.True(notifyTest.TestOnChangedTimesCalled(2), notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 2 calls instead of expected 1 calls.")]
    public void Fail_AssertOnChanged_low() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty2 = 10;
        Assert.False(notifyTest.TestOnChangedTimesCalled(1), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 2 calls instead of expected 1 calls.")]
    public void Fail_AssertOnChanged_High() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty2 = 10;
        Assert.False(notifyTest.TestOnChangedTimesCalled(3), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests number of calls. Fails because 0 calls instead of expected 2 calls.")]
    public void Fail_AssertOnChanged_NotCalled() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        //No Act
        Assert.False(notifyTest.TestOnChangedTimesCalled(2), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }
    #endregion

    #region OnChanged All Callbacks

    [Test, Description("Verifies that all callbacks were called.")]
    public void Success_AssertOnChangedCallbacks() {
        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(
            propertyName: nameof(MockPropertyObject.MyProperty1),
            invokeOrder: 1,
            description: "MyProperty1 should be equal to 100 on 2nd call.",
            callback: () => Assert.That(MockPropertyObject.MyProperty1, Is.EqualTo(20))
        );

        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), () => { });

        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 20;
        MockPropertyObject.MyProperty2 = 0;

        //Assert
        Assert.True(notifyTest.TestAllCallbacksInvoked(), notifyTest.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because invoke order callback not triggered.")]
    public void Fail_AssertOnChangedAllCallbacks_CallOrderNotTriggered() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 1, () => { });
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), invokeOrder: 1, () => throw new NotSupportedException());

        MockPropertyObject.MyProperty1 = 0;
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestAllCallbacksInvoked(), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Tests callbacks for assertion. Fails because no callback not triggered.")]
    public void Fail_AssertOnChangedAllCallbacks_NoCallbackTriggered() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 2, () => throw new NotSupportedException());
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), invokeOrder: 2, () => throw new NotSupportedException());
        //No Act
        Assert.False(notifyTest.TestAllCallbacksInvoked(), notifyTest.ErrorMessages);
        Assert.That(notifyTest.ErrorLog.Count, Is.EqualTo(2));
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }
    #endregion

    #region All Callbacks and OnChanged number of calls

    [Test, Description("Verifies that all callbacks were called and test number of calls succeeds.")]
    public void Success_AssertOnChangedAll() {

        //Arrange
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(
            propertyName: nameof(MockPropertyObject.MyProperty1),
            invokeOrder: 1,
            description: "MyProperty1 should be equal to 100 on 2nd call.",
            callback: () => Assert.That(MockPropertyObject.MyProperty1, Is.EqualTo(100))
        );

        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty2), () => { });

        //Act
        MockPropertyObject.MyProperty1 = 10;
        MockPropertyObject.MyProperty1 = 100;
        MockPropertyObject.MyProperty2 = 100;

        //Assert
        Assert.True(notifyTest.TestOnChangedAll(3), notifyTest.ErrorMessages);
    }

    [Test, Description("Fails because 1 calls instead of expected 2 calls.")]
    public void Fail_AssertOnChangedAll_Count() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestOnChangedAll(2), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Fails because callback was not triggered.")]
    public void Fail_AssertOnChangedAll_NoCallbackInvoked() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 1, () => { });
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestOnChangedAll(1), notifyTest.ErrorMessages);
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }

    [Test, Description("Fails because callback was not triggered and fails because 1 calls instead of expected 2 calls.")]
    public void Fail_AssertOnChangedAll_CallbackCount() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), invokeOrder: 1, () => { });
        MockPropertyObject.MyProperty1 = 10;
        Assert.False(notifyTest.TestOnChangedAll(2), notifyTest.ErrorMessages);
        Assert.That(notifyTest.ErrorLog.Count, Is.EqualTo(2));
        Console.WriteLine("Error log: " + notifyTest.ErrorMessages);
    }
    #endregion
    #region reset triggers

    [Test, Description("Tests if the reset test times called flag is working properly.")]
    public void Success_Reset_Count() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.IsResetCountOnAssert = true;

        MockPropertyObject.MyProperty1 = 10;

        Assert.True(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty1)));
        Assert.False(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty1)));
        notifyTest.ErrorLog.Clear();
        Assert.True(notifyTest.TestOnChangedTimesCalled(1));
        Assert.False(notifyTest.TestOnChangedTimesCalled(1));
        notifyTest.ErrorLog.Clear();

        notifyTest.IsResetCountOnAssert = false;
        MockPropertyObject.MyProperty1 = 10;

        Assert.True(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty1)));
        Assert.True(notifyTest.TestPropertyCalled(1, nameof(MockPropertyObject.MyProperty1)));
        Assert.True(notifyTest.TestOnChangedTimesCalled(1));
        Assert.True(notifyTest.TestOnChangedTimesCalled(1));
    }

    [Test, Description("Tests if the reset callback invoked flag is working properly.")]
    public void Success_Reset_Callback() {
        var notifyTest = new AssertNotifyProperty(MockPropertyObject);
        notifyTest.AddCallback(nameof(MockPropertyObject.MyProperty1), () => { });
        notifyTest.IsResetCountOnAssert = true;

        MockPropertyObject.MyProperty1 = 10;


        Assert.True(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)));
        Assert.False(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)));

        notifyTest.ErrorLog.Clear();

        notifyTest.IsResetCountOnAssert = false;

        MockPropertyObject.MyProperty1 = 10;

        Assert.True(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)));
        Assert.True(notifyTest.TestCallbacksInvoked(nameof(MockPropertyObject.MyProperty1)));
    }
    #endregion

    #region dispose
    [Test, Description("Test that callbacks are unassigned from the test object.")]
    public void Success_Dispose() {
        //Arrange
        MockPropertyObject.PropertyChanged += (_, _) => { }; //Ensure invocationList is not null.
        using (var notifyTest = new AssertNotifyProperty(MockPropertyObject)) {
            Assert.That(MockPropertyObject.PropertyChangedNumberOfCallbacks, Is.EqualTo(2));
            MockPropertyObject.MyProperty1 = 10;
            Assert.True(notifyTest.TestOnChangedAll(1));
        }
        Assert.That(MockPropertyObject.PropertyChangedNumberOfCallbacks, Is.EqualTo(1));
    }
    #endregion
}
