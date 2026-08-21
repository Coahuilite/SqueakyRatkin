namespace SqueakyRatkin;

// SqueakActionConfig 定义于 CompSqueaker.cs（0.2.4，含 Verse 依赖不可链接）；
// 此处为 SqueakActionModel.FromLegacy 编译所需的最小副本（字段与 0.2.4 一致，不执行）。
public class SqueakActionConfig
{
    public SqueakAction action = SqueakAction.Call;
    public SqueakTriggerMode mode = SqueakTriggerMode.RandomOneShot;
    public int minIntervalTicks = 300;
    public float probabilityPerCheck = 0.02f;
    public bool ignoreGlobalCooldown;
    public SqueakCooldownClock cooldownClock = SqueakCooldownClock.GameTicks;
}
