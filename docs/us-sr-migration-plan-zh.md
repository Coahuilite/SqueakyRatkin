# SR → US 迁移与退役方案评估（存量玩家优先）

> 背景约束（维护者 2026-08-23 定调）：SR 是**已有玩家的模组**，需要足够的退役时间且不得影响正在使用的玩家；US 是**框架级继承者**。
> 技术面证据见 [`us-sr-compatibility-check-zh.md`](./us-sr-compatibility-check-zh.md)；本文只谈"改动放在哪一侧、按什么顺序、玩家看到什么"。
> 日期：2026-08-23。不含本机绝对路径。

## 1. 决定可行性的五条事实基线

| # | 事实 | 依据 | 对迁移的含义 |
| --- | --- | --- | --- |
| B1 | **comp 根本不进存档**：RimWorld 1.6 `ThingWithComps.ExposeData()` 只调用 `base.ExposeData()`；`LoadingVars` 时执行 `InitializeComps()`，按**当前 def 的 comps** 重新 `Activator.CreateInstance`，从不从存档读 comp；SR 的 `CompSqueaker` 也没有任何 Scribed 状态 | RimSage：`Verse/ThingWithComps.cs` `ExposeData`/`InitializeComps` | **comp 换型/移除对存档零影响**，也没有"未知 comp 残留"警告。迁移的存档面天然安全，唯一要守的是既有纪律"不写存档" |
| B2 | 依赖缺失是 **mod 列表警告**（`ModUnsatisfiedDependency`），不是崩档 | RimSage：`Verse/ModsConfig.cs` `UnsatisfiedDependencies` | 1.0 起 SR 声明 US 前置时，缺 US = 列表警告 + Ratkin 静默；但**前提是 SR 1.0 不编译引用 US 类型**，否则会变成加载错误 |
| B3 | Steam Workshop 订阅**自动更新**，玩家无法"停在旧版"（除非取消订阅） | 平台行为 | 退役只能靠"长公告 + 时间窗 + 提供不受影响的替代路径"，不能靠"让玩家别更新" |
| B4 | US 当前**零玩家、零内置内容包**（`1.6/Defs`、`1.6/Patches` 为空） | US 仓库 0.4.x 只读审查 | **兼容修复放 US 侧迁移成本为零**；放 SR 侧则直接打在存量玩家身上 |
| B5 | 单写者只能由 US 让位实现：US 装配前检测"该种族是否已有任意 squeak comp"并跳过 | 兼容性报告 F1 | 反向做法（SR 检测到 US 就不装配）会在"玩家只装 US、没有 Ratkin 包"时让 Ratkin 变哑，属于影响存量玩家 |

## 2. 结论：改动归属与硬顺序

1. **本窗口内 SR 侧运行时零改动**：只做"冻结兼容面"（`SqueakyRatkin.CompProperties_Squeaker` / `CompSqueaker` 全名不变、XML 装配契约不变、不写存档）+ 文档；任何 SR 行为改动都会立刻作用于存量玩家。
2. **唯一允许先落的运行时改动是 US 侧 U1**（`VoicePackCompAttach` 逃逸门改跨程序集检测 + 让位 + skip 日志），且必须早于任何"US 服务 Ratkin"的动作。
3. **顺序硬门（倒置即事故）**：
   `U1 落地` → `US 型 Ratkin 包 / legacy 桥启用` → `SR 1.0 内容化（DLL 退役）`。
   - 若先上 Ratkin 包/桥 → 存量双装玩家**双响**。
   - 若 SR 1.0 先于 US 的 Ratkin 能力 → 存量玩家更新后 **Ratkin 静默**。
   - 若两侧 DLL 同时定义 `SqueakyRatkin.SqueakVoicePackDef`（重叠期）→ XML `Class=` 解析 first-wins 风险。

## 3. 分阶段计划（玩家可见动作 + 退出条件）

| 阶段 | 内容 | 改动方 | 存量玩家影响 | 公告 | 退出条件 |
| --- | --- | --- | --- | --- | --- |
| **P0（现在，SR 0.3.3）** | SR 照常；US 落地 U1（可先于任何 Ratkin 支持） | US | **0** | 无（US 未发布） | U1 有测试/日志证据；US 自测 Ratkin 跳过 |
| **P1（SR 0.4 + US 0.4 同窗）** | US 服务其他种族并对 Ratkin 让位；SR 不动 | US（SR 只发布已有行为） | **0**（SR 行为未变；US 可选装） | "两 mod 可同时启用；Ratkin 由 SR 提供，其他种族交给 US" | 兼容性报告 §5 双开矩阵全绿 |
| **P2（过渡期，建议 ≥4–8 周，SR 0.4.x patch 层）** | US 提供一次性设置导入（反射读 SR 设置，可选实现）；US 备好 Ratkin 能力但**内容包先不发**；SR 只在设置页/说明加一行"未来需要 US"提示，**不改发声行为** | US 为主 | 提示级 | 1.0 时间表与影响（需 US + FerriteLib）、替代路径三选一 | 导入路径或"重设"路径任选其一有实机证据；公告已发满窗口 |
| **P3（SR 1.0 退役）** | SR 内容化：无 DLL、不引用 US 类型；US 启用 legacy 桥承接 SR 包（`SqueakyRatkin.SqueakVoicePackDef`）；Ratkin 由 US 装配 | SR + US | **行为等价切换**（Ratkin 仍响，音源与选择经 US 路由）；只装 SR 且不装 US 的玩家会静默（见 §4 替代路径） | 前置 ≥1 个完整窗口 + Workshop 描述与更新日志明确 | US 单装 + SR 包可发声；双装无重复；旧存档正常；1.0 依赖缺失 = 警告 + 静默而非报错 |
| **P4（收尾）** | SR 仓库只维护内容/ABI；旧独立线冻结 | SR | 无 | 收敛说明 | 旧线无未决兼容问题 |

## 4. 不想迁移的玩家的三条路（必须在 1.0 公告里写清）

1. **另开冻结 legacy Workshop 条目**（如 "Squeaky Ratkin (Legacy)"，冻结在最后一个独立版本，建议 ≤0.5.x）。代价：RimWorld 1.6→1.7 更新时该条目需要偶尔兼容维护（维护预算要预留）。
2. **GitHub 保留旧 tag/资产 + Workshop 描述给手动安装指引**。代价：普通玩家门槛高。
3. **什么都不做**：1.0 自动更新后必须装 US+FerriteLib，否则 Ratkin 静默。这与"不影响存量玩家"冲突，不推荐作为默认。

建议 1+2 组合，并在公告里明确"三选一"。

## 5. 各面迁移清单

| 面 | 现状 | 窗口内变化 | 措施 |
| --- | --- | --- | --- |
| **存档** | comp 不进存档（B1）；两侧都不写存档 | 无风险 | 迁移改动不得引入任何 Scribe 写存档；沿用既有卸载安全纪律 |
| **设置** | SR `Mod_coahuilite.squeakyratkin.xml`（schema 4/2：voicePackSelections / xenotypePresets / moodOverrides / globalActionEnabled…）；US 为分层调音新模型 | P2 一次性导入或"重设一次" | SR 文件保留不删；导入失败不阻断启动；导入结果在 US 设置里可核对 |
| **音频包（作者面）** | SR XML ABI 已冻结；包用 `Class="SqueakyRatkin.SqueakVoicePackDef"` | P3 由 US 桥承接 | 作者**不需要改写**；桥必须兼容旧包字段（raceDefName/ageTag/IsEgg/fallbacks）；重叠期类型名归属见 §2 硬门 |
| **mod 列表/依赖** | SR 无前置；US 硬前置 FerriteLib | P3 SR 声明 US（传递 FerriteLib） | SR 1.0 不编译引用 US 类型 → 缺前置为警告+静默；依赖链在两侧 About/README 写清 |
| **日志/诊断** | srdiag 与 usdiag 两套协议 | 不合并 | 迁移期以 usdiag 为准；SR 旧协议在 SR 1.0 随 DLL 一起退役 |
| **卸载** | 任一侧卸载不影响存档（B1 + 既有纪律） | 不变 | P3 验收必须复测"卸 US / 卸 SR"两向 |

## 6. 风险登记

| ID | 风险 | 触发 | 影响 | 缓解 |
| --- | --- | --- | --- | --- |
| R1 | US 先上 Ratkin 包/桥、U1 未落 | 顺序倒置 | 存量双装玩家双响（最严重） | §2 顺序硬门 + US 发布门；SR 侧无语义改动 |
| R2 | Steam 自动更新把 1.0 推给没装 US 的玩家 | 1.0 发布 | Ratkin 静默 + 列表警告（不崩档） | 公告前置窗口 + §4 替代路径 + 1.0 不引用 US 类型 |
| R3 | 两 DLL 同时定义 `SqueakyRatkin.SqueakVoicePackDef` | 重叠期 | XML `Class=` 解析歧义、旧包走错 upcast | 发布顺序纪律 + 两侧发布门断言（桥只在 SR DLL 退役后启用） |
| R4 | 玩家调音设置丢失 | P3 | 体验损失、差评 | US 一次性导入（优先）或"重设一次"明确公告 |
| R5 | 作者困惑改投哪种 def 类型 | P2–P3 | 生态分裂 | 作者指南在 P3 更新：旧类型经桥继续有效；不强制改写 |
| R6 | 旧独立线遇 1.6→1.7 | 游戏更新 | legacy 条目失效 | 预留最小兼容维护预算，或公告"不承诺跨版本" |

## 7. 待维护者裁决

- Q1：是否插入一个 **SR 0.5 过渡版**（保持独立可用 + 显示迁移提示），还是只用 0.4.x patch 做过渡？
- Q2：是否另开**冻结 legacy Workshop 条目**（§4 选项 1）？
- Q3：P2 的**设置导入**做不做（US 侧反射读 SR 设置）？不做则改为"重设一次"公告。
- Q4：公告窗口长度定多少（建议 4–8 周，或一个完整发布周期）？
- Q5：1.0 的 US 前置取**硬前置**（`<modDependencies>`）还是**软前置**（不声明依赖，无 US 即静默）？按 B2，两者都不崩档，差别在玩家是否看到列表警告。
- Q6：legacy 独立线的跨游戏版本维护预算（是否承诺 1.7）。

## 8. 身份归属：packageId 该留给谁（2026-08-23 追加，回应"SR 改 legacy + 新开 pack-only SR"提案）

### 8.1 packageId 到底绑定了什么（代码级）

| 绑定面 | 证据 | 换 id 的后果 |
| --- | --- | --- |
| **玩家的音源选择** | PackKey = `modContentPack.ModMetaData.PackageIdNonUnique + ":" + defName`（SR 与 US 同构，`SqueakVoicePackModels.cs` `TryGetPackKey`）；持久化在 `voicePackSelections.enabledPackKeys` | 所有已选 PackKey 变 **orphan**（保留但不再解析）⇒ 玩家更新后音源被重置，需要重选 |
| 设置文件 | `Mod_coahuilite.squeakyratkin.xml`（按 packageId 命名） | 新 id 读不到旧文件 → 设置全丢（除非做导入） |
| Fallback profile Config 副本 | `SqueakFallbackProfileStore` 以 `SqueakyRatkinMod.PackageId` 校验/命名 | 旧副本失效重建 |
| Harmony 实例 id / 日志身份 | `Mod.cs` `Harmony = new Harmony(PackageId)` | 无实质影响 |
| 第三方 `IfModActive` 门与元数据 patch | `Patch_ModMetaData_LocalizedMetadata` 用 `SamePackageId(PackageId)`；外部模组可能 gate 旧 id | 外部兼容补丁指向旧 id，跟着 legacy 走 |
| **存档** | 上轮已核实：1.6 comp 不进存档（`ThingWithComps` 按当前 def 重建） | **无影响** |

⇒ 迁移唯一真正会"影响正在使用的玩家"的持久化资产是：**PackKey 选择 + 设置文件**。

### 8.2 两条线不能共用 id，也不能共用 defName

- **重复 packageId**：`ModLister.TryAddMod` → `Log.Error("Tried loading mod with the same packageId multiple times … Ignoring the duplicates")`，只有一个被加载（Steam 副本会走 postfix 路径，但 `PackageIdNonUnique` 仍是原始小写 id ⇒ PackKey 模糊）。所以"两个 item 共用 `coahuilite.squeakyratkin`"不可行。
- **重复 defName**：`DefDatabase.Add` → `Log.Error("Adding duplicate … name: …")` 并**给重复的 defName 追加随机数字后缀** ⇒ PackKey 被改名 ⇒ 老选择失配。所以 legacy 与新线**不能同时启用**（除非 legacy 改 defName，那等于改 PackKey 与作者引用，代价更大）。

### 8.3 方案对比

| 方案 | id 归属 | 迁移玩家的损失 | legacy 玩家的损失 | 维护成本 |
| --- | --- | --- | --- | --- |
| **A（提案）** 旧 item = legacy（留旧 id），新 item = pack-only（新 id） | 旧 id 给冻结线，新 id 给主线 | **音源选择重置**（PackKey 全变）+ 设置不跟随 | 无（订阅不变） | 两个 item 长期并行；若想保住选择需 US 侧做"旧键→新键"映射导入 |
| **B（推荐）** 同一 item / 同一 id 就地升级为 pack-only；legacy = 新 id 冻结归档（或仅 GitHub） | **旧 id 跟内容走** | **零损失**（PackKey / 设置文件 / 订阅 / 第三方门全不变） | 需手动换到 legacy 条目（他们本就在主动退出框架） | 一个主线 + 一个冻结归档 |
| **C（最小）** 不提供 Workshop legacy，只留 GitHub 归档 | 旧 id 给主线 | 零损失 | 需手动装 GitHub 版 | 最低 |

### 8.4 结论

1. **"无法继承 packageId"不是必然，而是方案 A 的前提造成的**。packageId 应该跟**内容**走：pack-only 继承人就地继承 `coahuilite.squeakyratkin`（现有 Workshop item 原地升级），legacy 独立线拿新 id（建议 `coahuilite.squeakyratkin.legacy`）并冻结。
2. **新 pack-only 线的包应使用 US 原生 def 类型**（`UniversalSqueaker.SqueakVoicePackDef`），不要用 `SqueakyRatkin.SqueakVoicePackDef`：只要玩家机器上还装着 legacy 的 DLL，`Class=` 可能解析到 legacy 类型，US 的 catalog 就看不见该包（类型不同 ⇒ 不承认）。
3. **只要 legacy 长期在线，US 侧 U1（跨程序集让位检测）就从"过渡期修复"升级为"永久必需"**——分裂方案不会让兼容工作消失，只会让它常态化。
4. 若选 A，必须额外做：US 侧"旧 PackKey → 新 PackKey"映射导入；legacy 与新线互斥声明（defName 冲突）；新线包用 US 原生类型。

### 8.5 追加裁决项

- Q7：id 归属按 **B（主线继承 id，推荐）** 还是 A（legacy 保留 id）？
- Q8：是否接受"迁移玩家音源选择被重置一次"（A 的代价）？若否，B 是唯一选择。
- Q9：legacy 是否上 Workshop 长期并行（若上，是否写 `incompatibleWith` 新线），还是仅 GitHub 归档（方案 C）？
- Q10：新 pack-only 线的包用 US 原生类型（推荐）还是 SR 旧类型（与 legacy 冲突）？

## 9. 与既有文档的关系

- 技术面（双装配/双 Harmony/桥/依赖链）：[`us-sr-compatibility-check-zh.md`](./us-sr-compatibility-check-zh.md)
- 本方案（归属/顺序/时间窗/玩家路径）：本文
- 行动项：`TODO.md`「US 兼容性」条目；耐久指针：`MEMORY.md`
