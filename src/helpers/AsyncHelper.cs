using System;
using AysncTask = System.Threading.Tasks.Task;

namespace CheatMenu;

public static class AsyncHelper {
    public static AysncTask WaitSeconds(int seconds){
        return AysncTask.Delay(TimeSpan.FromSeconds(seconds));
    }
}