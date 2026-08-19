using System;
using System.Collections.Generic;
using Verse;

namespace SqueakyRatkin;

// ---- 0.2.4 枚举副本（序数稳定性红线）----
// 这些枚举定义在 Source/SqueakyRatkin/CompSqueaker.cs（0.2.4），因 CompSqueaker 依赖面
// （RimWorld/Verse.Sound/UnityEngine）无法在纯 net472 harness 中链接，此处按 v0.2.4 tag
// 逐值复制。存档按名序列化 + Scribe 保存用 ToString()：只要名字与序数不变即与真实代码等价。
// 红线：SqueakAction 为 append-only（0.3.x 提取到领域文件时保持名字与序数不变）。
public enum SqueakMood { Good, Neutral, Bad, Break }

public enum SqueakAction { Call, Eat, Sleep, Wounded, Select, Move, Social, Joy, Death, Draft, Undraft, Attack, Work, Equip, MentalBreak }

public enum SqueakCooldownClock { GameTicks, Realtime }

public enum SqueakTriggerMode { EachTime, RandomOneShot, External, Sustained }

public enum SqueakTriggerOutcome { NotAttempted, Success, Cooldown, ProbabilitySkipped, VocalBlocked, Silent }

// 0.2.4 SqueakyRatkinSettings.cs 顶部（L34）；SqueakActionConfig 定义于 CompSqueaker.cs（L188-197），
// 此处为 SqueakActionModel.FromLegacy 编译所需的最小副本（字段与 0.2.4 一致）。
public enum SqueakDistancePreset { Conservative, Balanced, Strong, Custom }

public class SqueakActionConfig
{
    public SqueakAction action = SqueakAction.Call;
    public SqueakTriggerMode mode = SqueakTriggerMode.RandomOneShot;
    public int minIntervalTicks = 300;
    public float probabilityPerCheck = 0.02f;
    public bool ignoreGlobalCooldown;
    public SqueakCooldownClock cooldownClock = SqueakCooldownClock.GameTicks;
}

/// <summary>
/// 0.2.4 副本（v0.2.4 tag，SqueakyRatkinSettings.cs 顶部，逐行对应）：
/// 心情→音色调制参数，settings 字典值类型。ExposeData 与 0.2.4 逐字段一致。
/// </summary>
public class SqueakMoodMod : IExposable
{
    public SqueakMood mood = SqueakMood.Neutral;
    public float pitchFactor = 1f;
    public float volumeFactor = 1f;
    public FloatRange pitchJitter = FloatRange.One;

    public void ExposeData()
    {
        Scribe_Values.Look(ref mood, "mood");
        Scribe_Values.Look(ref pitchFactor, "pitchFactor", 1f);
        Scribe_Values.Look(ref volumeFactor, "volumeFactor", 1f);
        Scribe_Values.Look(ref pitchJitter, "pitchJitter", FloatRange.One);
    }

    public SqueakMoodMod Clone() => new() { mood = mood, pitchFactor = pitchFactor, volumeFactor = volumeFactor, pitchJitter = pitchJitter };
}
