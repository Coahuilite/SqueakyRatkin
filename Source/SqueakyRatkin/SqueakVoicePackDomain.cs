namespace SqueakyRatkin;

// 包域枚举（零 Verse 引用，0.3.0 自 SqueakVoicePackModels.cs 原样提取，namespace 不变）。
// 0.3.1 起 SelectionMode 是内核 API；本文件的 SqueakVoicePackMode 仅保留为设置/适配层 ABI。

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
