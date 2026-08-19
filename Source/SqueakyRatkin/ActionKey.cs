using System;

namespace SqueakyRatkin;

/// <summary>
/// 内核动作键边界映射（§2.2/§4.1）：适配层唯一转换点。
/// 内置动作的规范键 = 枚举名（append-only 序数不变的镜像面）；外部动作键 = packageId.defName（0.4.x 动作门）。
/// 键与枚举名的一致性由 KernelCharacterization 的 validator 双向校验锁定。
/// </summary>
public static class ActionKey
{
    private static readonly string[] BuiltInKeys = Enum.GetNames(typeof(SqueakAction));

    /// <summary>枚举 → 规范键（内置 = 枚举名）。未知值返回 null（调用方按未知/外部键处理）。</summary>
    public static string? For(SqueakAction action)
    {
        int index = (int)action;
        return (uint)index < (uint)BuiltInKeys.Length ? BuiltInKeys[index] : null;
    }

    /// <summary>内置规范键 ↔ 枚举反射名一致性校验；成功时输出对应枚举。</summary>
    public static bool TryParseBuiltIn(string key, out SqueakAction action)
    {
        for (int i = 0; i < BuiltInKeys.Length; i++)
        {
            if (string.Equals(BuiltInKeys[i], key, StringComparison.Ordinal))
            {
                action = (SqueakAction)i;
                return true;
            }
        }
        action = default;
        return false;
    }
}
