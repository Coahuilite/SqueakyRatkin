namespace SqueakyRatkin;

// 动作域枚举（0.3.1 波 1 移入 Kernel/；零游戏运行时引用，namespace 不变以维持存档按名序列化 ABI）。
// 红线：SqueakAction 为 append-only，序数稳定（0.3.2 在末尾 append Crying/Giggling）。

public enum SqueakMood { Good, Neutral, Bad, Break }

// Keep the original nine serialized enum values stable. New built-ins are append-only.
public enum SqueakAction { Call, Eat, Sleep, Wounded, Select, Move, Social, Joy, Death, Draft, Undraft, Attack, Work, Equip, MentalBreak }

public enum SqueakCooldownClock { GameTicks, Realtime }

/// <summary>触发模式,由 XML 配置驱动,C# 通用适配。</summary>
public enum SqueakTriggerMode
{
    EachTime,
    RandomOneShot,
    External,
    Sustained
}
