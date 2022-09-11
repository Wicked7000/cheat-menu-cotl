using System;
using AysncTask = System.Threading.Tasks.Task;

namespace cheat_menu;

public static class AsyncHelper {
    public static AysncTask WaitSeconds(int seconds){
        return AysncTask.Delay(TimeSpan.FromSeconds(seconds));
    }
}