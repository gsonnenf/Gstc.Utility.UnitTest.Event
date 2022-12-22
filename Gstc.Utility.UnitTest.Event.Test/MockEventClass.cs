using System;

namespace Gstc.Utility.UnitTest.Event.Test;

public class MockEventClass {
    public event EventHandler<Event1EventArgs>? UserEvent1;
    public int UserEvent1NumberOfCallbacks => UserEvent1!.GetInvocationList().Length;
    public void TriggerUserEvent1(int number) => UserEvent1?.Invoke(this, new Event1EventArgs(number));

}

public class Event1EventArgs : EventArgs {
    public int Number { get; set; }
    public Event1EventArgs(int number) {
        Number = number;
    }
}
