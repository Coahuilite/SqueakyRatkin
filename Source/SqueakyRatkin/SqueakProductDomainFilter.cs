using System;
using System.Collections.Generic;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin;

/// <summary>
/// 0.3.x 版本限定产品域过滤器（决策文档 §4.4；0.4.x US 拆分时移除）。集中一处、白名单数据表驱动、
/// 随版本冻结，结构上不可能越过：catalog 构建过滤、GetTargetCandidates assembled-only 投影、
/// 池/内置 fallback 装配三处入口都强制经过它。0.3.x 装配域常量 = {Ratkin}；试验名单配置化升级
/// 是 0.3.x 通用化完成后的独立机制，不在此对象内。本类不写任何 race 特判，只有数据。
/// </summary>
public static class SqueakProductDomainFilter
{
    /// <summary>0.3.x 装配域白名单（编译期冻结常量，Ordinal 精确匹配）。</summary>
    public static readonly IReadOnlyCollection<string> AllowedRaceDefNames = Array.AsReadOnly(new[] { "Ratkin" });

    /// <summary>闸谓词：pack.raceDefName 是否属于装配域。</summary>
    public static bool Contains(string? raceDefName)
    {
        foreach (string allowed in AllowedRaceDefNames)
            if (string.Equals(allowed, raceDefName, StringComparison.Ordinal)) return true;
        return false;
    }

    /// <summary>内核池过滤器投影：池装配（条目域 + 内置 tier 查表上下文）同样只能命中装配域。</summary>
    public static DomainFilter ToKernelFilter()
    {
        HashSet<RaceKey> allowed = new();
        foreach (string name in AllowedRaceDefNames) allowed.Add(new RaceKey(name));
        return new DomainFilter(allowed);
    }
}
