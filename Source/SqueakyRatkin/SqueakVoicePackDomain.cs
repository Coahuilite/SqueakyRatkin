namespace SqueakyRatkin;

// 包域枚举（零 Verse 引用，供 Kernel/ 链接编译；0.3.0 自 SqueakVoicePackModels.cs 原样提取，namespace 不变）。

public enum SqueakVoicePackScope
{
    Unspecified = 0,
    Race = 1,
    Xenotype = 2
}

/// <summary>Audio selection policy. It is intentionally versioned separately from retired remix settings.</summary>
public enum SqueakVoicePackMode
{
    Off,
    Fallback,
    Remix
}
