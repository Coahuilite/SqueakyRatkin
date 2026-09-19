# SR × US 兼容性检查（SR 退役为纯音频包前的必须项）

> 检查对象：演进项目 US（同级仓库，packageId `coahuilite.universalsqueaker`，本地 `0.4.x` 分支 @ `c8794ff`，工作树含 4 个未提交改动）。
> 检查方：SR 仓库 `dev`（本次推进版本 0.3.3）。方式：只读审查 US 源码/Defs/文档，并与 SR 逐文件对拍。
> 迁移与退役方案（改动归属/顺序/时间窗/玩家路径）见 [`us-sr-migration-plan-zh.md`](./us-sr-migration-plan-zh.md)。
> 本文不含本机绝对路径、日志摘录或凭据。
> 日期：2026-08-23。

## 0. 结论摘要

1. **当前双开是安全的**：US 的 catalog 只枚举 `UniversalSqueaker.SqueakVoicePackDef`，SR 的包是 `SqueakyRatkin.SqueakVoicePackDef`（另一个程序集/类型）；US 也没有 legacy 桥实现、没有 Ratkin 字面量。因此现在 SR+US 双开只有 SR 在 Ratkin 上发声，两 mod 各响各的、互不串音。
2. **但"Ratkin 作为 US 首发支持包"落地的那一刻会立刻双响**：US 的自动装配逃逸门只认**自己程序集**的 `UniversalSqueaker.CompProperties_Squeaker`；SR 通过 XML patch 挂的是 `SqueakyRatkin.CompProperties_Squeaker`。类型不匹配 ⇒ US 会在 Ratkin `ThingDef.comps` 上**追加第二个** squeak comp。两仓 16 个 patch 文件有 15 个同名同目标（US 是 SR 的分叉），每个 patch 只 `GetComp<自己类型>()` ⇒ 每个事件双响、周期采样双跑。
3. **退役窗口前必须解决三件事**：① Ratkin 装配的唯一写者（需要跨程序集的检测机制）；② legacy 桥的类型名所有权与"两 DLL 同时在"的重叠期；③ 两仓规则冲突（US 仓库文档写"0.4 不允许 Ratkin 装配"，维护者口径是"Ratkin 已是首发支持包之一"）。
4. **归属与顺序（存量玩家约束下的唯一解）**：修复必须落在 US 侧（US 让位 + 让位日志），SR 本窗口**零运行时改动**；顺序硬门 = `U1 检测落地 → US 型 Ratkin 包/桥 → SR 1.0 内容化`。详见迁移方案 §2。

## 1. 检查证据（关键代码与事实）

| # | 证据 | 事实 |
| --- | --- | --- |
| E1 | US `Source/UniversalSqueaker/Catalog/VoicePackCompAttach.cs`（`Apply`，判据在 `def.comps.Any(comp => comp is CompProperties_Squeaker)`） | 逃逸门按**类型身份**判断；只认 US 自己的 `CompProperties_Squeaker`。已存在 SR 的 comp 时判据为 false → 追加挂载并记 `CompAutoAttached` |
| E2 | SR `1.6/Patches/Ratkin_AddSqueakComp.xml` | 用 XPath 给 `AlienRace.ThingDef_AlienRace[defName="Ratkin"]/comps` 加 `<li Class="SqueakyRatkin.CompProperties_Squeaker">` |
| E3 | US `Source/UniversalSqueaker/Mod.cs:103` `VoicePackCompAttach.Apply(SqueakXenotypeCatalog.Current)` | 装配在 `ExecuteWhenFinished`（晚于 XML patch）⇒ US 能"看到" SR 的 comp，但类型不匹配仍会重复挂 |
| E4 | US `Catalog/SqueakXenotypeCatalog.cs`（`DefDatabase<SqueakVoicePackDef>.AllDefs`；挂载集合 = 全部被承认包的 `raceDefName` 去重并集） | US 只承认 US 类型包；SR 包对它不可见（反向同理） |
| E5 | 逐文件对拍 `Patches/Patch_Selector_Select.cs`（SR/US 仅 namespace 不同） | 两仓 patch 目标与实现同源；结尾均为 `pawn.GetComp<CompSqueaker>()?.Notify_Select()`，各绑自己程序集的类型 ⇒ 双 comp 时双派发 |
| E6 | US `CompSqueaker.cs` `CompProperties_Squeaker.CreateDefault()` | 与 SR XML 同基线（216 t；Eat EachTime 144；Call 864/1.2%；Move 504/1.2%…）⇒ 双响是同节奏双发，不是互补 |
| E7 | 全仓 grep US `Source`/`1.6`：`Ratkin`、`SqueakyRatkin`、`SR_` 零命中；`1.6/Defs`、`1.6/Patches` 为空 | US 当前无内置内容/无 Ratkin 特判/无 SR 检测 |
| E8 | US `docs/mod-structure-reference-zh.md` | 写明 0.4 共存规则"US 仓库内不允许 `SqueakyRatkin.*` 类型、`SR_` Def、Ratkin 装配/profile/attachment"，**唯一例外**：授权（2026-08-24）legacy 兼容桥的薄空类 `SqueakyRatkin.SqueakVoicePackDef`；SR 源码 grep 显示该桥**尚未实现** |
| E9 | US `About/About.xml` | US 已有硬前置 `coahuilite.ferritelib`（`<modDependencies>` + 代码内版本断言） |
| E10 | US `Patches/Patch_RedirectModSettingsWindow.cs` | 只拦截 owner 为 `UniversalSqueakerMod` 的设置对话框；不劫持 SR 窗口 |
| E11 | 两仓 `LoadFolders.xml` | 均为 `/` + `1.6`，无 `IfModActive` 门控（符合两仓各自纪律） |
| E12 | 设置/配置文件名 | 各自带 packageId（SR `SqueakyRatkin_*`、US `UniversalSqueaker_*`）；Scribe 类型分属两个命名空间 ⇒ 无共享文件、无序列化互踩 |

## 2. 发现清单

| ID | 严重度 | 触发条件 | 影响 | 建议修复（归属） |
| --- | --- | --- | --- | --- |
| **F1** | 高（条件性） | SR + US + 任一 US 承认的包声明 `raceDefName=Ratkin`（首发支持包落地即触发） | Ratkin 被挂两个 squeak comp ⇒ 双响、双冷却、双诊断 | US 侧把逃逸门扩展为**跨程序集**检测：按"已存在任意 squeak comp"跳过（类型全名/标记接口），并新增 skip 原因日志（U1/U2） |
| **F2** | 高（同源） | 同 F1 | 15/16 个 Harmony patch 同目标，各推自己 comp ⇒ 每个外部事件与周期采样双发；玩家听感为"回声/重音"，且两套设置的冷却各自计时 | 根因同 F1：**单装配**是唯一干净解；短期至少要有互认门（双方都跳过"对面已服务"的种族） |
| **F3** | 高（窗口期） | US 实现 legacy 桥（授权已在文档）后，即使没有 US 原生 Ratkin 包，SR 的旧 XML 包也会被 US 承认 | ① 直接触发 F1（US 开始挂 Ratkin）；② 若 SR.dll 与 US.dll 同时定义 `SqueakyRatkin.SqueakVoicePackDef`，XML `Class=` 解析按加载序 first-wins 可能绑到另一程序集，旧包走错 upcast 路径 | US 侧：桥启用条件与 SR 程序集是否在载做互斥（运行时 gate 或文档硬约束）；两仓统一"重叠期谁拥有该类型名"（U3/S4） |
| **F4** | 中 | 规则冲突本身 | US 文档"0.4 不允许 Ratkin 装配"与维护者口径"Ratkin 是首发支持包之一"不能同时为真；不裁决则 F1 的修复方向无法定 | 维护者裁决：**US 服务 Ratkin（需 U1）** 或 **US 不服务 Ratkin（Ratkin 内容侧由 SR/独立包提供）**（Q1） |
| **F5** | 中 | 1.0 窗口 SR 变成 US 前置 | 依赖链 SR → US → FerriteLib；0.4 共存期 US 单独就要求玩家装 FerriteLib | 两侧 About/README 写清；SR 1.0 声明 US 前置时确认三级链路（S3） |
| **F6** | 低 | 双开 | 设置窗口互不劫持；配置/设置文件互不覆盖；诊断各自独立 | 无需改动（保留为回归断言） |
| **F7** | 低 | 双开 | 无 Def/类型/Action 键/日志 once-key 冲突（命名空间与 packageId 分离） | 无需改动（保留 E4/E12 为验证项） |
| **F8** | 低 | 卸载任一 | 两侧装配都只改内存 `ThingDef.comps`，不写存档 | 无需改动；但 F1 修复不得引入存档写入 |

## 3. 组合矩阵（Ratkin 视角）

| 装机组合 | Ratkin 行为 | 判定 |
| --- | --- | --- |
| 仅 SR | 单响 | 现状 |
| 仅 US | 不响（US 无内置 Defs；需外部 US 型包） | 现状 |
| SR + US，无 US 型 Ratkin 包、桥未实现 | 单响（仅 SR） | **当前双开安全** |
| SR + US + US 型 Ratkin 包 | **双 comp → 双响**（F1/F2） | 首发包落地即触发 |
| SR + US，桥已实现（US 承认 SR 旧包） | **双 comp → 双响**（F3） | 桥落地即触发 |
| SR 1.0（纯音频、无 DLL）+ US | 单响（US 装配 + 桥承认 SR 包） | 目标态 |

## 4. 退役窗口前的行动清单

**SR 侧（本仓库）**
- S1：把"Ratkin 装配唯一写者"写进 SR 合同与 MEMORY：0.4 共存期 Ratkin 由 SR 装配；US 服务其他种族（或按 Q1 裁决反向）。
- S2：保持 `SqueakyRatkin.CompProperties_Squeaker` / `SqueakyRatkin.CompSqueaker` **全名稳定**（不迁移、不改名、不拆类型），因为跨程序集检测只能依赖类型全名或双方共同标记接口——这是 SR 能单方面提供的兼容面。
- S3：1.0 窗口：SR 退化为纯音频包时，确认 US 侧 legacy 桥与 `SR_*` SoundDef 交叉引用可用；声明 US（与 FerriteLib）前置链路。
- S4：与 US 对齐"`SqueakyRatkin.SqueakVoicePackDef` 类型名的重叠期所有权"，避免两 DLL 同时定义。

**US 侧（需转述）**
- U1：`VoicePackCompAttach` 逃逸门改为跨程序集"已存在任意 squeak comp 即跳过"，并给出可核验的日志/诊断原因（如 `foreign_squeak_comp`）。
- U2：按 Q1 裁决落实"是否服务 Ratkin"；若服务，U1 是发布硬前置。
- U3：legacy 桥的启用前置条件（SR 程序集不存在 / 或运行时不加载）与类型名所有权约定。
- U4：把 §5 的双开矩阵纳入 US 发布门（与 SR 侧同步跑）。

## 5. 双开实机验收矩阵（F1 修复后必须全绿）

| 场景 | 期望 |
| --- | --- |
| SR + US（无 US 型 Ratkin 包） | Ratkin 单响；US 服务其他种族单响 |
| SR + US + US 型 Ratkin 包 | **Ratkin 仍单响**；US 侧日志显示跳过 Ratkin 装配 |
| SR 旧包经 legacy 桥被 US 承认 | Ratkin 仍单响 |
| 事件矩阵（Select/Draft/Undraft/Attack/Wounded/Death/Equip/MentalBreak/BabyFits + 周期 Eat/Move/Call/Work/Social/Joy/Sleep） | 每次事件恰好一条声音，无双发 |
| 卸载 US / 卸载 SR | 另一侧照常；存档无残留（不得新写存档） |

## 6. 待维护者裁决

- Q1：**US 是否服务 Ratkin**？（决定 F4 走向与 U1 是否为硬前置）
- Q2：跨程序集检测用哪种：类型全名匹配（零新依赖，脆弱度低）还是双方共同标记接口（更干净，但需要 SR 在 1.0 依赖 US 后实现）？
- Q3：legacy 桥上线时，SR 程序集是否与 US 同装（重叠期长度）？重叠期谁拥有 `SqueakyRatkin.SqueakVoicePackDef`？
- Q4：0.4 双开是否需要"任一侧检测到另一侧在服务同一 Domain 时跳过整个 Domain"（本报告只覆盖 comp 装配层，未覆盖音频包选择层）。
