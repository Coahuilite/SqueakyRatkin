# SR 文档终审：结论、证据与收敛决定

## 0. 审查结论与边界

**结论：v2 可作为历史索引，不能直接升格为现行合同或发布许可。** 现行合同值得保留，但须重构并纠正文实不符；规划、发布证据、历史交接和本次管道材料应归档。本文先完成终审，再据此决定文档布局，不沿用上游预定的保留清单。

- 审查日期：2026-09-17 至 2026-09-19；09-19 接续时 HEAD 与原始文档未变化。SR 本地基线：`547e2f2539dec5bc50324433db3fc951097b7f10`，分支 `dev`。
- 输入：同目录 `handoff-package-v2.md`，覆盖六包、26 份原始文档；其冻结 revision 为 `4df9594713adbbba91e0aea788a7c7cd3503ab3f`。二者只有双语 CHANGELOG 时间行不同。
- `[F]` 为本次直接读到的文档/源码/本地事实，**不自动等于游戏行为实测**；`[C]` 为原记录的报告；`[I-终审]` 为本次判断；`[U]` 为未确定。源码回查另标 `[src-回查]`。
- 本次用户授权的是 **SR 目录文档精简**：先终审，后整理；双语 CHANGELOG 原样保留；旧文档保留在 `docs/archive/`。附件的“只读终审”和旧任务书的“删除 archive / 自动提交 / 记忆全面压缩”不作为本轮执行指令。
- 未读取 US 仓库，未查询远端或 Steam，未执行发布、历史重写或游戏实测。源码只用于定点核实高影响结论，不是全库代码审查。回查按下文问题组织，不采用旧提示词建议的八次硬上限。
- v2 声称约 50,776 / 50,788 字符，本次按 UTF-8 解码后的 UTF-16 单元实量为 **51,111**；其自报数字不一致。审查不以凑压缩率为目标。

## 1. 当前上下文

SR 是 RimWorld 1.6 的鼠族动作音效模组，品牌 `鼠辈啁啾` / `Squeaky Ratkin`，packageId `coahuilite.squeakyratkin`。产品承诺仍限 NewRatkinPlus Ratkin；路由机制按包声明的 `raceDefName` 处理域，二者不是矛盾。

本地产品版本为 0.3.3。最新本地正式 tag 为 `v0.3.0`，另有 `v0.3.2-pre1`。本地 `dev` 对本地缓存的 `origin/dev` 领先 6 提交；该缓存不能证明远端当前状态。CHANGELOG 已在 `547e2f2` 换成 2026-09-17 时间，**不构成 0.3.3 已上线证据**。本次未找到 0.3.3 Claim Pack；Steam 沿用最后“阻断、待维护者”的记录，当前外部状态未验证。

## 2. 决策演化

| 时间 | 决策及反转 | 对当前的意义 | 来源 |
| --- | --- | --- | --- |
| 2026-07 至 08-17 | 0.1.x 首发；0.2.1 发布污染重发；0.2.3 默认改 Fallback + 内置 Example | 旧默认不能覆盖现行默认；历史发布事故不能变成永久人工清单 | v2 §2、§5；旧 Claim Packs |
| 08-18 至 08-20 | 四案采纳“内核但薄”；吸收其他案的事务性迁移、候选投影、hook 核实和语料扩展；不拆程序集 | 纯内核、单向依赖和失败面验证仍有价值，按阶段的施工清单已过期 | v2 DEC-v2-4/5/6；原架构决策 §2–§5 |
| 08-20 至 08-21 | 0.3.0 A–H 通过，部分为论证/审计；BuildFallback 后置修复；随后正式发布 | 历史“阻塞”已被后续记录取代；“全绿”不等于无缺陷 | v2 §5；原 0.3.0 评审/检查表 |
| 08-21 至 08-24 | 17 键、年龄域、fallback、XML ABI、IsEgg；UI 专项转 US；0.3.2-pre1；Eat 两级开关；US 共存与退役方案追加 | 已实施的 SR 约束与尚待裁决的 US 路线必须分开 | v2 DEC-v2-7/8/9/13；原 compat/migration/Eat 文档 |
| 09-13 | V1 追认、V2 先方案不重写、V3 接受；发布源改“大版本分支 → main”，dev 只集成 | 不沿用旧 push dev → PR 或 dev 与 main 必须相等的发布模型 | 原 runbook 阶段 1；MEMORY 后续裁决 |
| 09-17 | CHANGELOG 换时间；本次终审 | 文案时间与发布证据分账；本次不替维护者放行 | 本地 git diff/log |

## 3. 独立审查发现与裁定

### R1 · 发布状态及分支路径：纠正，不宣布发布

v2 §1/DEC-v2-14/OQ-5 一面采用 09-13 状态，一面仍写 push dev → PR；它也仍称 CHANGELOG 为 Unreleased。`[F][src-回查]` 现行 runbook 阶段 1 已明确“大版本分支 → main”，阶段 2 却残留 main == dev，属于源文档自身未收敛。

`[I-终审]` 新 runbook 应统一到经批准的大版本分支与 main 的发布树；dev 不再作为发布合并源或发布树相等基准。本地 `0.3.x` 仍在 `b19d68a`，而 0.3.3 工作在 dev，**分支同步是后续发布工作，本次不能假定已完成**。V1/V3 已定案，V2 只批准出方案；不能再次把三项全部列成“尚未定案”。具体版本、渠道与操作授权仍待维护者。

### R2 · US“未建仓/当前安全”：改为历史条件结论

`[F][src-回查]` 兼容性报告开头已给出 US `0.4.x` @ `c8794ff` 加 4 个未提交改动的审查对象。因此 v2 同时宣称“08-22 暂不建仓”是当前状态，与自身后续证据冲突。该快照本来就不可只凭 SHA 完整复现。

`[I-终审]` “US 现已创建”有历史报告支持，但**当前 US 代码/服务域/发布情况均未验证**。“双开安全”只限那次无 US 型 Ratkin 包、无桥的组合；F1/F2/F3 是条件性推演，不是已经发生的玩家事故。保留 U1 → 包/桥 → SR 内容化的先后约束，不把旧版本号当作完成证明。

### R3 · XML ABI 起点：撤销“已闭合”

v2 §6.2 把 PKG-1#OQ-1 判已闭合，§11 K5 又把 released 解释为“正式版”，推至 0.3.3 或以后。`[F][src-回查]` 原合同只写 `first released version that carries the 0.3.1 ABI`，未排除 prerelease；`v0.3.2-pre1` 已有发布时间、tag、资产证据。

`[I-终审]` **不能因无正式 0.3.1 就推出冻结承诺尚未生效。** `v0.3.2-pre1` 是已有证据的候选锚点；首个携带版本及是否包含 prerelease 的精确解释仍待确认。当前作者 XML add-only / 17 动作 append-only / fail-closed 承诺继续执行；不利用起点歧义收回承诺，也不编造起始版本。

### R4 · 日志规范：两个遗漏及一个版本归属错误

`[F][src-回查]` `Logging/SqueakLogProtocol.cs` 的 v1 formatter 在 `enabled` 后、异常字段前已写 `pawn pawn_id`；`tools/SqueakLogCharacterization/Program.cs` 有相应字节期望。原日志合同和 v2 ANCH-C 都漏了这两字段。它不是本次新增的日志扩展。

原事件表有 32 行，却称“最后四行是 v2、前 28 行是 v1”；实际 `hook.mental_fit.unavailable` 在表中间且 registry 版本为 2，尾部 `devtools.workbench.open_failed` 仍为 1。新文档须明确分表：**28 个 v1 + 4 个 v2**，保留事件名、可见性、等级和人读句。

### R5 · 日志隐私：实现行为不等于规范合规

`[F][src-回查]` facade/formatter 接受并输出 pawn label；普通字段只经 `PercentEncode`，只有异常消息进入 `SanitizeExceptionMessage`。原“所有字符串先脱敏再编码”和“协议绝不写 pawn label”均不能描述该实现。v1 也已含 pawn，而非仅 0.3.2 才引入问题。

`[I-终审]` 新合同应分别写明实际编码行为、禁止提交诊断日志的仓库规则、尚未裁决的 label 隐私冲突。**percent-encode 不是匿名化**。保留当前协议形状只是如实记载，不是批准隐私例外；若要删字段或统一脱敏，应另立实现变更及兼容判断。本次不改日志代码。

### R6 · Remix 三层路径存在实际反例，不能写成已满足合同

`[F][src-回查]` `Kernel/SqueakPoolRegistry.cs` 的 `SelectRemixThree` 统计非空层数量，却以该数量抽出的索引访问未压紧的原三槽位。无显式 pack fallback、Xenotype 层缺失、Race 与 BuiltIn 层可用时：

| 输入 | 当前执行结果 | 合同要求 |
| --- | --- | --- |
| `(None, Race, BuiltIn)`，roll `0.25` | `None` | 应为某个可用层 |
| 同上，roll `0.75` | `Race` | 两可用层等权；BuiltIn 不应不可达 |

**验证**：直接读取当前 `Kernel/*.cs`，仅把 file-scoped namespace 包装为普通 namespace 以便内存编译；通过反射调用原函数，固定 `IRollSource.Next01()`，得到上表结果。未写源码、fixture、测试脚本或游戏存档。这是独立的纯函数反例，不是游戏内复现；整个适配/播放路径的玩家影响仍需后续验证。

**09-19 接续验证**：通过公开 `SqueakPoolRegistry.Select` 构造 Ratkin Race 域、一个 `Call` 声音、正式 `BuiltInFallbackCatalog.Create("Ratkin")`、全部可播放的 gate、无 pack fallback。固定 roll 0.25/0.75，Off 两次返回 `SR_Call`，Fallback 两次返回测试 Race 声音，Remix 分别返回 None / Race。`SqueakRuntimeSnapshot.Choose` 直接映射选择结果，`SqueakKernelAdapter.ToChoice` 将空键转 None，`CompSqueaker.PlayOneShot` 对空声音直接返回 `NoEligibleSound`，未发现补抽或内置兜底。**公开内核入口反例已实测、适配传播已读源码，游戏内听感仍未实测**；不得把这两种证据混成实机通过。

`[I-终审]` 该问题比 v2 的“Crying/Giggling 缺层未定义”更具体：源实现有证据支持的缺陷。现行合同保留“可用层等权”的期望，并紧邻标注实现偏差；不得把 bug 改写成预期规则。修复要考虑冻结语料的兼容裁决，本次只登记，不能声称已修复。原来的黄金语料零 delta 仅证明相对基线不变，不能证明语义正确。

### R7 · packageId 跟内容走有理由，“零损失”没有得到证明

`[F][src-回查]` 迁移方案 §8.3 宣称 B 保留 id 因而设置/选择零损失；同一文档 §5 却说明 US 是另一设置模型，P2 是否导入仍在 Q3 待决。

`[I-终审]` 保留 id 与 defName 可降低 PackKey 身份漂移，**不能证明 US 已读取 SR 的选择和调音设置**。B 仍是条件性推荐，不能写成已裁决或完整零迁移。应分别验收包键、订阅、配置文件、目标 schema 和读取/导入链；C（仅 GitHub legacy）同样保留主线 id，所以“拒绝一次重置则只能 B”也过强。平台原地升级、旧类型桥、依赖缺失行为均保留原证据等级，不推断完整迁移安全。

### R8 · 矛盾/开放项不是待办数量；删除的是重复热副本

`[F]` v2 §6.1 的 51 个 CNF 可逐号找到，但有些是决策反转、命名差异、文档陈旧或已解释的时间层，不是 51 个当前缺陷。Eat 本次 add-only 不 bump schema，与内部 schema 未来可变并不矛盾；已明确的父关=WholeJob 也无需因接收方 US 的开放问句而重新成为 SR 未决项。

`[F]` v2 宣称 OQ 96/96，却把 PKG-3#OQ-14 写成 `PKG-3#Q8'`。其问题内容存在，编号索引漏失；附录补回映射，不改原 v2。GAP 合并使用区间与不完整别名，计数不能证明语义零损失。

`[I-终审]` 全量 CNF/OQ/GAP 保存在本报告附录和原包；日常文档只提仍有后果的未决、阻塞与检查边界。过去“未推送”、C 链缺号、旧版页面时间差等冷证据不常驻 TODO；需要再查时通过归档索引定位。Kiiro 分支已删除的记忆层优先于旧“活跃实验”叙述，许可约束仍保留。

### R9 · 教训降格、门禁覆盖范围如实表述

v2 L12/L15 将 US 零玩家侧修复和 id 归属泛化成强原则；L21/L25 把事故时期的手工 tree 清单与 `merge -s ours` 固化为常规动作；A50 称“脚本不会漏”。这些都过强。

`[I-终审]` 保留的教训是：验证强度要可见；黄金基线不能替代语义断言；自动断言应替代重复人工清单；特殊恢复操作只适用于其前提；发布渠道各自取证；迁移须逐项证明持久化资产被消费。US 零玩家前提有时效，fail-open 仅在 Eat 兼容目标下成立，tier 折叠的原设计意图仍为 tentative。

`[F][src-回查]` `privacy-audit.ps1` 默认工作树向量用 `git grep`，不覆盖普通 untracked/ignored 文件；其模式不是任意敏感信息检测器。历史 known-debt 按模式名+路径匹配，未限定具体 revision/命中值；新 commit-message 和当前跟踪树是另扫的，不能把这点等同于历史例外能识别同路径的每种新泄漏。此次新增文档须单独纳入一次性交付核查，不改脚本、不加 CI 门。

### R10 · Workshop 草稿应匹配目标产品，不能伪装线上快照

旧草稿是 0.3.0 页面源：15 动作、所有鼠族同一触发路径、任何缺音必回退等说法不能无条件套用 0.3.3。17 个动作键不等于内置音频覆盖 17 个；玩家主动动作有身份门；Crying/Giggling 没有内置声音时可静默。旧置顶公告“已删”与 Claim Pack“待删”仍缺独立回执。

`[I-终审]` 重建为**面向 0.3.3 的待发布双语文案**，写明目标版本，链接既有 releases 入口而不虚构 v0.3.3 tag 可用；文字中纠正上述边界，保留品牌与作者入口。维护元信息写“未上传、线上未核验”，不续写已删公告的断言，不把常见 8000 字符当作已核实的平台硬限。字符数实际计算；页面编辑和发布仍由维护者后续执行。

## 4. 仍需维护者或后续实现解决的事

1. R6 Remix 可用层抽样修复与冻结基线取舍；适配路径/实机验证。
2. R5 pawn label 的隐私处理与日志兼容政策；R3 ABI 的精确发布锚点；日志 tier 是否扩展归因。
3. 0.3.3 实机结果、发布源分支同步、具体渠道与授权；Steam 恢复和公告状态复核。
4. US 当前快照、Q1 服务域、跨程序集检测、桥类型归属、U1–U4 回执及双开/卸载/旧 XML 矩阵。
5. 迁移方案 Q1–Q10（过渡版、legacy 渠道、配置导入、公告窗口、依赖声明、维护预算、id/类型归属），尤其不可将设置导入默认为完成。
6. 隐私方案维持 A；B 的授权、备份、工具、替换边界、tag 集合、commit-map、跨仓协调与不可回收副本风险仍开。旧 10/9 tag、227/230、54 处引用都是历史估计，不能拿来直接执行。
7. Eat 三态实机、身份门余项和低频回归仍以现有 TODO 为行动记录；无新证据不能勾掉。历史 Workshop 观察缺口不回填成已验证。

## 5. 由终审推导的文档布局

**日常阅读面收敛为九份文件**，而非另造一套完整历史知识库：

| 文件 | 必要性与处理 |
| --- | --- |
| `docs/CHANGELOG.md`、`docs/CHANGELOG.zh-CN.md` | 用户要求字节不变；已知排序/发布时间解释问题单独注明，不回改 |
| `docs/project-architecture-contract.md` | 重建：产品/触发/路由/XML ABI/持久化/纯内核约束，标注 R3/R6 |
| `docs/settings-ui-product-contract-zh.md` | 重建：独立 UI 验收与持久化合同，避免混进架构过程史；路径保持以兼容引用 |
| `docs/logging-protocol.md` | 重建：可解析的完整字段与事件表，修正 R4，显式保留 R5 未决 |
| `docs/release-runbook-zh.md` | 重建：三命令、最小发布仪式、统一分支路径、渠道证据模板 |
| `docs/steam-workshop-page-copy-draft.md` | 重建：0.3.3 待发布双语 BBCode，不宣称已上架 |
| `docs/maintenance-status-zh.md` | 汇总当前已知状态、待修复/裁决、退役顺序和历史债务指针；替代交接/规划/迁移多入口 |
| `docs/codemap.md` | 单一读序与权威索引，无运行逻辑 |

旧 24 份正文（除双语 CHANGELOG）和整个 consolidation 管道目录进入 `docs/archive/`，**不改原文件字节，不删除历史证据**。三份合同也先归档旧版再新写，不把旧正文简单搬回。归档内另有索引和本报告；不会把大段终审历史常驻现行合同。

根 README/CONTRIBUTING 与作者指南因三份合同路径不变可继续使用；仅修复必要的导航引用与当前交接指针。不执行旧任务书要求的 MEMORY/TODO 全面压缩；不改源代码、脚本、CI、作者 skill、版本或分支。

## 6. 回查记录与证据边界

以下路径相对仓库根，行号以审查前基线计。归档后原 docs 路径按 `docs/archive/README.md` 找到，源码保持原处。这里记录的是实际回查，没有把 v2 声明改称本次实测。

| 要验证的结论 / 预期 | 回查来源 | 结果与留下的问题 |
| --- | --- | --- |
| 冻结时点是否漂移，预期只有后续文档变化 | `git log -8`、冻结 revision 到 HEAD 的 diff、`git branch -vv`、本地 tag | 仅双语时间行变化；R1；没有远端新证据 |
| 核心合同是否支持 ABI 起点已闭合 / 17 键与 15 音频差异 | 原架构合同 §1/§3/§6/§7；pre1 Claim Pack §GitHub | R3；作者承诺保留，起点需确认 |
| UI 不变是否排除 Eat | 原设置合同“非目标与实现路径”；原 Eat §7.1/§8 | 显式两级控件例外，不构成全面 UI 开放；实机未补齐 |
| 日志字段是否匹配 v1 字节锁，预期表与 formatter 一致 | 原日志合同 v1/v2/事件表；`Logging/SqueakLogProtocol.cs:43`、`:163`、`:247`；`SqueakLog.cs:62`；logging harness `Program.cs:99`、`:225` | 不一致；R4/R5；未运行游戏日志 |
| Remix 缺层是否可按可用层等权 | `Kernel/SqueakPoolRegistry.cs:62`、`:113`、`:242`；`Pool.cs:53` | 内存编译当前内核并调用原方法，反例成立；R6 |
| Eat fail-open 方向是否被误读 | `SqueakEatOccurrence.cs:37`、`:45` | WholeJob / 营养 / 未确认或toil或营养的并集，源码确认；游戏采样未验证 |
| US 尚未建仓 / 当前双开安全 | 原兼容报告前言、§0/§1/§2 | 较早审过 US，不足以代表现状；R2 |
| id 不变是否足以零损失 | 原迁移方案 §5/§8 | 读取/导入未闭合；R7 |
| 三命令能否替代旧人工仪式 | 原 runbook 阶段 1/2；流程冗余评估 §7–§9；MEMORY 后写裁决；脚本参数和 release.yml | 分支残留需改；V1–V3分账；R1/R9 |
| 隐私扫描是否覆盖全部交付文件和任意敏感信息 | `scripts/privacy-audit.ps1:65`、`:85`、`:92`；旧重写方案 §4–§7 | 不是全覆盖；方案 B 未授权；R9 |
| 文案是否可直接继承到 0.3.3 | 原 Workshop 草稿完整双语与维护规则 | 15/17、身份门、缺音与线上状态需纠正；R10 |

## 7. 供下游挑战的钩子

- **R6 反例**：公开 Select 入口已复现，适配层与播放层未见补偿；剩余误判可能是某一实际装配组合不可达，或实机的声音可播放条件不同。需用上述输入对应的实际配置验证玩家影响，不将确定性内核反例写成游戏内已复现。
- **R5 隐私冲突**：误判可能是历史已明确批准 label，或调用方另有净化。回查具日期的裁决及 dispatch 采样；当前未找到批准，不把不存在证据当作确定拒绝。
- **R3 ABI 起点**：可能已有更早携带版本，或维护者约定 released 特指正式版。必须提供具体 tag/资产或裁决，不能仅凭版本号推断。
- **R7 零损失**：US 可能已实现完整导入，也可能 PackKey 形状已变化。本次没读取 US；需完整 commit、干净树和旧配置导入实测才能关闭。
- **R1 发布状态**：可能在本次本地检查之外已上线；需独立渠道证据。本报告没有宣布“远端肯定未发”。
- **精简是否丢约束**：按下方原台账和归档源逐条挑战；现行文件不重复每条旧争议，归档保留全部原字节，不把未入热文档等同废弃。

## 8. 自检与上游台账保留方式

上游十二项要求落点：1/2/5/6/7/9/11 → §2–§5；3 → §0 和逐条证据标签；4 → §6；8 → R9；10 → 本报告；12 → §5 的按内容收敛。至少三个独立不同意点见 R3/R4/R6/R7，不将二手标签升格。

核对的标识符包括 `Crying=15`、`Giggling=16`、17/15、v1 28/v2 4、`ChewIngestible`、`eatOnlyDuringChewing`、`eatIncludeDrugs`，均按原文/源码保留。v2 的 CNF 51、OQ 96、GAP 51 为上游条目口径，**不是本轮验证通过的数量**。

下方附录将保留 v2 的 §6 和 §8 全文作为原陈述账本，并另列原包编号全集。原 v2 与六个 v1 包同时保留，因此原有两边要旨、时间层、假设与证据缺口仍可回查。附录状态不覆盖本报告 R1–R10；特别是“已闭合”的 ABI、“未建仓”、“当前双开安全”、“零损失”、“脚本不会漏”应按本报告读取。


## 9. 原包编号全集（导航，不是验证通过计数）

### CNF

- `PKG-1#CNF-1`、`PKG-1#CNF-2`、`PKG-1#CNF-3`、`PKG-1#CNF-4`、`PKG-1#CNF-5`、`PKG-1#CNF-6`、`PKG-1#CNF-7`、`PKG-1#CNF-8`
- `PKG-2#CNF-1`、`PKG-2#CNF-2`、`PKG-2#CNF-3`、`PKG-2#CNF-4`、`PKG-2#CNF-5`、`PKG-2#CNF-6`、`PKG-2#CNF-7`、`PKG-2#CNF-8`
- `PKG-3#CNF-1`、`PKG-3#CNF-2`、`PKG-3#CNF-3`、`PKG-3#CNF-4`、`PKG-3#CNF-5`、`PKG-3#CNF-6`、`PKG-3#CNF-7`、`PKG-3#CNF-8`、`PKG-3#CNF-9`、`PKG-3#CNF-10`
- `PKG-4#CNF-1`、`PKG-4#CNF-2`、`PKG-4#CNF-3`、`PKG-4#CNF-4`、`PKG-4#CNF-5`、`PKG-4#CNF-6`、`PKG-4#CNF-7`、`PKG-4#CNF-8`、`PKG-4#CNF-9`、`PKG-4#CNF-10`、`PKG-4#CNF-11`
- `PKG-5#CNF-1`、`PKG-5#CNF-2`、`PKG-5#CNF-3`、`PKG-5#CNF-4`、`PKG-5#CNF-5`、`PKG-5#CNF-6`、`PKG-5#CNF-7`、`PKG-5#CNF-8`
- `PKG-6#CNF-1`、`PKG-6#CNF-2`、`PKG-6#CNF-3`、`PKG-6#CNF-4`、`PKG-6#CNF-5`、`PKG-6#CNF-6`

### OQ

- `PKG-1#OQ-1`、`PKG-1#OQ-2`、`PKG-1#OQ-3`、`PKG-1#OQ-4`、`PKG-1#OQ-5`、`PKG-1#OQ-6`、`PKG-1#OQ-7`、`PKG-1#OQ-8`、`PKG-1#OQ-9`、`PKG-1#OQ-10`、`PKG-1#OQ-11`、`PKG-1#OQ-12`、`PKG-1#OQ-22`
- `PKG-2#OQ-1`、`PKG-2#OQ-2`、`PKG-2#OQ-3`、`PKG-2#OQ-4`、`PKG-2#OQ-5`、`PKG-2#OQ-6`、`PKG-2#OQ-7`、`PKG-2#OQ-8`、`PKG-2#OQ-9`、`PKG-2#OQ-10`、`PKG-2#OQ-11`
- `PKG-3#OQ-1`、`PKG-3#OQ-2`、`PKG-3#OQ-3`、`PKG-3#OQ-4`、`PKG-3#OQ-5`、`PKG-3#OQ-6`、`PKG-3#OQ-7`、`PKG-3#OQ-8`、`PKG-3#OQ-9`、`PKG-3#OQ-10`、`PKG-3#OQ-11`、`PKG-3#OQ-12`、`PKG-3#OQ-13`、`PKG-3#OQ-14`、`PKG-3#OQ-15`、`PKG-3#OQ-16`、`PKG-3#OQ-17`、`PKG-3#OQ-18`、`PKG-3#OQ-19`、`PKG-3#OQ-20`
- `PKG-4#OQ-1`、`PKG-4#OQ-2`、`PKG-4#OQ-3`、`PKG-4#OQ-4`、`PKG-4#OQ-5`、`PKG-4#OQ-6`、`PKG-4#OQ-7`、`PKG-4#OQ-8`、`PKG-4#OQ-9`、`PKG-4#OQ-10`、`PKG-4#OQ-11`、`PKG-4#OQ-12`、`PKG-4#OQ-13`
- `PKG-5#OQ-1`、`PKG-5#OQ-2`、`PKG-5#OQ-3`、`PKG-5#OQ-4`、`PKG-5#OQ-5`、`PKG-5#OQ-6`、`PKG-5#OQ-7`、`PKG-5#OQ-8`、`PKG-5#OQ-9`、`PKG-5#OQ-10`、`PKG-5#OQ-11`、`PKG-5#OQ-12`、`PKG-5#OQ-13`、`PKG-5#OQ-14`、`PKG-5#OQ-15`、`PKG-5#OQ-16`
- `PKG-6#OQ-1`、`PKG-6#OQ-2`、`PKG-6#OQ-3`、`PKG-6#OQ-4`、`PKG-6#OQ-5`、`PKG-6#OQ-6`、`PKG-6#OQ-7`、`PKG-6#OQ-8`、`PKG-6#OQ-9`、`PKG-6#OQ-10`、`PKG-6#OQ-11`、`PKG-6#OQ-12`、`PKG-6#OQ-13`、`PKG-6#OQ-14`、`PKG-6#OQ-15`、`PKG-6#OQ-16`、`PKG-6#OQ-17`、`PKG-6#OQ-18`、`PKG-6#OQ-19`、`PKG-6#OQ-20`、`PKG-6#OQ-21`、`PKG-6#OQ-22`、`PKG-6#OQ-23`

### GAP

- `PKG-1#GAP-1`、`PKG-1#GAP-2`、`PKG-1#GAP-3`、`PKG-1#GAP-4`、`PKG-1#GAP-5`、`PKG-1#GAP-6`、`PKG-1#GAP-7`
- `PKG-2#GAP-1`、`PKG-2#GAP-2`、`PKG-2#GAP-3`、`PKG-2#GAP-4`、`PKG-2#GAP-5`
- `PKG-3#GAP-1`、`PKG-3#GAP-2`、`PKG-3#GAP-3`、`PKG-3#GAP-4`、`PKG-3#GAP-5`、`PKG-3#GAP-6`、`PKG-3#GAP-7`、`PKG-3#GAP-8`、`PKG-3#GAP-9`、`PKG-3#GAP-10`、`PKG-3#GAP-11`
- `PKG-4#GAP-1`、`PKG-4#GAP-2`、`PKG-4#GAP-3`、`PKG-4#GAP-4`、`PKG-4#GAP-5`、`PKG-4#GAP-6`、`PKG-4#GAP-7`、`PKG-4#GAP-8`、`PKG-4#GAP-9`、`PKG-4#GAP-10`
- `PKG-5#GAP-1`、`PKG-5#GAP-2`、`PKG-5#GAP-3`、`PKG-5#GAP-4`、`PKG-5#GAP-5`、`PKG-5#GAP-6`、`PKG-5#GAP-7`、`PKG-5#GAP-8`、`PKG-5#GAP-9`、`PKG-5#GAP-10`、`PKG-5#GAP-11`
- `PKG-6#GAP-1`、`PKG-6#GAP-2`、`PKG-6#GAP-3`、`PKG-6#GAP-4`、`PKG-6#GAP-5`、`PKG-6#GAP-6`、`PKG-6#GAP-7`

别名修正：`PKG-3#Q8'` 对应 `PKG-3#OQ-14`（迁移 Q8，见 compact §6 的第 14 行）；`PKG-1#OQ-22` 是对 PKG-6 Eat 诊断项的跨包引用。编号保留不等于待办仍开。

## 10. v2 原陈述账本（冻结转录）

以下状态继承自 v2，需结合 R1–R10 的纠正阅读。未修饰原陈述，以免抹去矛盾。

### 6. 矛盾与未决问题（零丢弃：CNF 51/51、OQ 96/96）

#### 6.1 矛盾（51 条全保留；按主题分组，每组保留双方要旨与时间层）

**同文档内部互斥（8）**：PKG-1#CNF-1 `pawn=<label>` 事件字段表 vs 通则禁写 pawn 标签（0.3.2 扩字段未回改通则；percent-encode≠匿名化）；PKG-1#CNF-2 tier 可发词汇三值 vs 产品四层链（折叠致 fallback 不可归因，归因损失是否已知取舍未记载）；PKG-1#CNF-3 日志协议状态戳 08-22 vs 内容含 0.3.2 事实（末次提交 08-23）；PKG-1#CNF-4 Eat 例外 settings schema「不 bump」vs 内部面 0.x 窗口（settings 面无冻结声明）；PKG-1#CNF-5 Remix 冻结三层形状 vs `Crying`/`Giggling` 缺内置层时抽样形状未定义。PKG-1#CNF-6 元规则「不得静默归一」vs 文档内部五处未消解（本包即首次登记）；PKG-1#CNF-7 套件产物落 `docs/` vs README「不提交」设计（隐私门从「默认不需要」变必须）；PKG-1#CNF-8 ABI 起点「first released version that carries the 0.3.1 ABI」不可判定（即使 OQ-1 已闭合仍保留）。——PKG-1 主源为合同文本，互斥均为同文档内部。

**0.3.x 交付与验收（8）**：PKG-2#CNF-1 派发「C1–C19」vs 语料止于 C14（编排方实测：可见 C1–C10、C12–C14+PKG-4 载体 C17–C19/C34–C40，缺席 C11/C15/C16/C20–C33，禁止写连续 C1–C40）；PKG-2#CNF-2 C11 全文无号（`5b51c6a`/`67028b8` 未编号两解）；PKG-2#CNF-3 语料 1622 vs 3782 例（同 corpus 两例数，扩容非错误勿合并）；PKG-2#CNF-4 交接「GitHub prerelease→Steam」vs 08-20「不设 prerelease tag、本地 dev 包」（预写被否定）；PKG-2#CNF-5 动作门窗口 0.4.x vs 08-22 修订「US 0.3.x 并行、0.4 随首版」（口径演化）；PKG-2#CNF-6 IsEgg 内部标签 vs 0.3.2 公开 ABI（有记录的废止）；PKG-2#CNF-7 交接「可能未提交/未推送」vs 检查表「origin/0.3.x 已推送」（时间层）；PKG-2#CNF-8 热修 `0.3.0.x` vs `vX.Y.Z-hotfixN`（定案为准）。

**双仓路线与拆分（10）**：PKG-3#CNF-1 Q1（US 是否服务 Ratkin）US 仓规则文档「0.4 不允许 Ratkin 装配」vs 维护者「Ratkin 已是 US 首发支持包之一」——冲突一方在语料外，**保留不归一**；PKG-3#CNF-2 note「不存在双实现共存窗口」vs 08-22 修订「0.4 双仓同步上架、SR 1.0 才收缩」（共存窗口=0.4→1.0 有意设计）；PKG-3#CNF-3 阶段表体「0.4.x 首版拆分发布」vs 修订注 SR 1.0 收缩（以注代改、引用必须同时引修订注）；PKG-3#CNF-4 「四层限定」vs 实际 7+ 条且编号断裂（下游不得把「四层」当可核验计数，替代=filter 三处入口）；PKG-3#CNF-5 compat 前言 08-23 vs E8 授权 08-24（文档后追加过或日期不可靠）；PKG-3#CNF-6 compat F4「修复方向无法定」vs mig「修复必须落 US」（层次不同：Ratkin 内容侧归属 vs 修复落点，可并存）；PKG-3#CNF-7 U1 过渡期修复 vs legacy 长期在线则永久必需（条件化非推翻）；PKG-3#CNF-8 反转：「无法继承 packageId」不是必然而是方案 A 前提造成的→packageId 跟内容走（隐含 Workshop 原地升级可行、平台层未取证）；PKG-3#CNF-9 compat Q1–Q4 ≠ mig Q1'–Q10'（共用 Q 前缀指代不同，**转述必须带文档限定**）；PKG-3#CNF-10 `SqueakBuiltInFallbackCatalog`（规划示例名）vs `BuiltInFallbackCatalog`（合同逐字名）——权威序合同>规划，若必须取一建议合同名，冲突记录保留、主源记录不得改写。

**发布渠道与外部状态（11）**：PKG-4#CNF-1 CHANGELOG 规则「旧前新后、未发布置顶」vs 正文实序 0.3.0→0.2.0 逆序（终裁=维护者改规则或重排）；PKG-4#CNF-2 0.2.0 前身词表 vs runbook 词表（细化提取、历史层保留）；PKG-4#CNF-3 Claim Pack 模板行集 vs 六实例偏差谱（0.2.1 缺字节、0.3.0 改行名、LoadFolders 行消失 [I]≈读时门接管）；PKG-4#CNF-4 GitHub UTC+8 时间 vs 页面 Updated 时区对不齐（[I]≈UTC-7；跨渠道时间账须标时钟）；PKG-4#CNF-5 0.3.0 当日「公告仍在线待删」vs 文案源「已于 0.3.0 如期删除」（后写口径不代表前段不存在、删除复核无独立留痕）；PKG-4#CNF-6 pre1 待办预设「正式 0.3.2 会发生」vs Notes「0.3.2 仅 prerelease、工作随 0.3.3」（后写=现行；0.3.3 是否继承 pre1 证据开放）；PKG-4#CNF-7 Change Notes 3→6 vs 有记录上传仅两次（+3 不匹配、或曾上传或未记录条目）；PKG-4#CNF-8 隐私行三称法（0 命中/0 真实命中/0 未接受命中——[I] 同义=台账外真实命中为零）；PKG-4#CNF-9 ZH/EN 版式漂移（缺空行、非口径冲突）；PKG-4#CNF-10 「0.3.0 失效公告」两所指（置顶失效预告 vs 拆分预告）不违规但依赖 U-7 快照；PKG-4#CNF-11 `merge -s ours` 只在真实分叉时用 vs squash 收尾本身制造分叉（[I] 主动对账即分叉第一时刻）。

**流程与隐私（14）**：PKG-5#CNF-1 「待裁决 3 项」vs「V1–V3 定案」vs「§1 未回写」（以 §7 为最新口径但不得删 §1 行）；PKG-5#CNF-2 前言「门禁覆盖没有缺失」vs R4「覆盖缺口（不是冗余）」（R4=身份面，并列保留）；PKG-5#CNF-3 227 revision vs 230 提交（差 3 无口径注记、不自行调查）；PKG-5#CNF-4 「10 tag 全部指向受影响提交」vs「`v0.2.0` 唯一内容干净」（10 vs 9 并列不择一，下游不得把 10 当受影响数）；PKG-5#CNF-5 隐私债务 1→5 文件反转（旧口径单独保留，解释 V2 为何重估）；PKG-5#CNF-6 「隔离的一次性仓库」vs「真实仓库被写脏」（措辞疑为事故后补救 [I]）；PKG-5#CNF-7 前言「已落地」vs 表头担保前 3 项+措施 8「待实现」（以逐行标注为准）；PKG-5#CNF-8 措施 8 属人工心智清单却未被 R1–R12 覆盖（需语义判断非机械可判定）；PKG-6#CNF-1 08-23「未提交未推送 0 提交」vs 09-13「本地三块已提交停在推送前」（分层写作，现行=09-13 层）；PKG-6#CNF-2 E§7 前言「已实施单开关」vs 同日两级已实施（较早草稿层残留）；PKG-6#CNF-3 注 1「无公开 toil 权威」vs §2.3 `CurToilString` public（复核结论被推翻，更正=B 立项前提）；PKG-6#CNF-4 「跳过」定案 vs §5/§9.2 同项列开放（定案=工作口径、开放=终局确认门，两态并存勿混引）；PKG-6#CNF-5 「known-debt 5 文件、范围比先前记录更大」vs「先前记录」数值未载（两包数字互为校验）；PKG-6#CNF-6 「三份新文档均无隐私串」vs untracked 4 个（三份排除交接正文，非实质矛盾防误报）。

**PKG-4 自身 11 条独立登记（已并入 6.1 组 4 与组 5 表述）**。

#### 6.2 OQ（96 条全保留；按主题分组，id 随主题就近）

**0.3.3 发布线（PKG-4 9 行 + PKG-6 5 + PKG-2 1）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-6#OQ-1 | 0.3.2 正式版是否**确认**跳过（口径=跳过、工作并入 0.3.3） | 口径不变、终局裁决开放 |
| PKG-6#OQ-2 | Steam 是否恢复发布（0.3.2 从未上 Steam、阻断中） | 开放，需授权+人工编辑面 |
| PKG-6#OQ-3 | 0.3.3 GitHub full vs prerelease 先行 | 开放 |
| PKG-6#OQ-4 | 发布裁决 V1–V3 与推送仪式绑定 | 开放；内容本体 [xref: PKG-5] |
| PKG-6#OQ-5 | 完整推送链 push dev→PR→CI→squash→tag `v0.3.3`→资产核验→CHANGELOG 换时间→Claim Pack→渠道核验 | 停在远端推送前（冻结时点） |
| PKG-4#OQ-6 | Steam 二进制下载级验证从未执行=全部 Workshop 格上限 | 需授权+人工 |
| PKG-4#OQ-7 | 0.3.3 无发布面（无 Claim Pack、文案锚点 0.3.0、一次核对未做） | 开放 |
| PKG-4#OQ-9 | `vX.Y.Z-hotfixN` 与 `archive/` 归档法零实例未检验 | 常设 |
| PKG-4#OQ-11 | 「发布裁决」无规定留痕格式（pre1 头注为孤例） | 可审计性风险 |
| PKG-4#OQ-12 | `release-<version>-review-zh.md` 命名无 prerelease/hotfix 规范 | pre1 先例未收编 |
| PKG-4#OQ-13 | 「0.3.0 失效公告」短语无原文快照 | 影响对外承诺链 |
| PKG-4#OQ-4 | changelog 时间精度政策未裁决 | 开放 |
| PKG-4#OQ-8 | CI Node20 警告是否消除无后续记录 | 开放 |
| PKG-4#OQ-10 | known-debt 5 条清偿依赖授权 | 方案 [xref: PKG-5] |
| PKG-2#OQ-1 | merge `0.3.x→dev`+渠道发布授权（检查表时点阻塞） | PKG-4 已闭合 0.3.0 已发布；时层并存 |

**身份/迁移/US（PKG-3 16 行 + PKG-6 1 行交叉）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-3#OQ-1 | **Q1（compat §6）**：US 是否服务 Ratkin？ | **阻塞**；F1 修复方向、S1 条款、DEC-20/21/22/23 |
| PKG-3#OQ-2 | Q2（compat）：跨程序集检测=类型全名 vs 标记接口 | U1 形态、依赖时序 |
| PKG-3#OQ-3 | Q3（compat）：桥上线重叠期、谁拥有 `SqueakyRatkin.SqueakVoicePackDef` | R3/S4/U3 |
| PKG-3#OQ-4 | Q4（compat）：0.4 双开是否需 Domain 级跳过（报告自陈未覆盖选择层） | 选择层双响缺口 |
| PKG-3#OQ-5 | S1「Ratkin 装配唯一写者」是否已写入 SR 合同与 MEMORY | 语料外回查 |
| PKG-3#OQ-6 | U1–U4 转述/落地进度、P0 退出条件是否达成 | P1–P3 全阻塞在 U1 后 |
| PKG-3#OQ-7 | Q1'（mig）：是否插 SR 0.5 过渡版 | 版本序列、P2/P3 边界 |
| PKG-3#OQ-8 | Q2'：是否另开冻结 legacy Workshop 条目 | DEC-25/27、R6 维护预算 |
| PKG-3#OQ-9 | Q3'：P2 设置导入做不做（US 反射读 SR 设置） | R4（调音丢失）、P2 退出 |
| PKG-3#OQ-10 | Q4'：公告窗口长度（建议 4–8 周） | P3 需前置 ≥1 完整窗口 |
| PKG-3#OQ-11 | Q5'：1.0 的 US 前置硬还是软（按 B2 均不崩档、差别=警告可见性） | F5/R2/P3 退出 |
| PKG-3#OQ-12 | Q6'：legacy 独立线跨版本维护预算（承诺 1.7？） | R6 |
| PKG-3#OQ-13 | Q7'：id 归属 B（主线继承，推荐）还是 A（legacy 保留） | **阻塞**；PackKey 重置、设置跟随 |
| PKG-3#Q8' | 是否接受迁移玩家音源选择被重置一次（A 的代价；若否 B 是唯一选择） | Q7' 硬蕴含 |
| PKG-3#OQ-15 | Q9'：legacy 上 Workshop 长期并行 vs 仅 GitHub 归档 | S2/U1 永久化、R3/R6 |
| PKG-3#OQ-16 | Q10'：新 pack-only 线用 US 原生类型还是 SR 旧类型 | DEC-28、作者指南 R5 |
| PKG-3#OQ-17..20 | 交接风险三则+未验证门：无实施回执、US 基准不可复现、US 规则文档单向不可达、双开实机验收矩阵五场景未跑 | GAP-7/GAP-2/GAP-3；矩阵=F1 修复后 P1/P3 退出 |
| PKG-6#OQ-6..9 | 迁移/身份 Q1–Q10 逐项分派；身份归属推荐（packageId 跟内容走）未裁决；US Q1–Q4；U1–U4 实施进度 | 条文 [xref: PKG-3]；GAP-6 |
| PKG-2#OQ-2..5 | US 并行四问：建仓需授权（08-22 暂不建仓、语料内未建）；Workshop 显示名/许可待确认；US 在 SR 1.0.0 时点版本号待定；0.3.x 与 US 发布顺序暂不决定+ABI「首个携带」浮动表述 | 全部开放；PKG-4 已闭合版本序列 |
| PKG-2#OQ-7,9,10,11 | 6 条 no_sound 无根因关闭（本包质疑保留）；双开共存验证未到执行窗口；身份门控矩阵余项「自然覆盖」/全文在 TODO；C15–C19 与 C11 缺号延伸未知 | 未验证/编排；PKG-2#CNF-1、G10 承接 |

**Eat 与日志可观测性（PKG-1 13 + PKG-6 8 行）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-1#OQ-1 | 「首个承载 0.3.1 ABI 的发行版本」=哪一版（已由 PKG-4 证据闭合：0.3.1 无正式版、0.3.2 仅 pre1）；PKG-1#CNF-8 保留不删 | 已闭合、CNF 保留 |
| PKG-1#OQ-2 | `pawn=<label>` 是否被接受为日志常态字段 | 未决（PKG-1#CNF-1） |
| PKG-1#OQ-3 | 是否拆细 tier 区分 pack_fallback/built_in_fallback | 未决（PKG-1#CNF-2） |
| PKG-1#OQ-4 | settings schema 是否承诺永不 bump | 未决（PKG-1#CNF-4） |
| PKG-1#OQ-5 | `Crying`/`Giggling` Remix 缺层抽样形状 | 未决（PKG-1#CNF-5） |
| PKG-1#OQ-6 | 未来 action gates 与内置动作键 append-only 的边界 | 未决 |
| PKG-1#OQ-7 | map-lifecycle reset 未实现是缺陷还是有意；once-key 1024 清空噪音 | 未决 |
| PKG-1#OQ-8 | orphan 数量/保留期上限 | 未决 |
| PKG-1#OQ-9 | profile 副本「当前版本」判据 | 未决 |
| PKG-1#OQ-10 | 实现权威路径清单是否覆盖 1.6/Assemblies 之外 flavor | 未决 |
| PKG-1#OQ-11 | `prompts/**` 允许面、产物落 docs 后 README §10 三选一处置 | 需授权/编排方 |
| PKG-1#OQ-12 | 三份合同无签名/版本/生效日期——是否补版本头 | 未决 |
| PKG-1#OQ-22（跨包 PKG-6#OQ-22） | Eat 粒度已裁决不新增日志事件；替代 Dev 诊断面板是否推进 | **仍开**（面板未实施） |
| PKG-6#OQ-10 | Eat 三态手工验收矩阵未实机（meal/烟卷/薄片/啤酒/仙馔/营养膏/背包吃/动物/尸体 ×三态） | 未验证 |
| PKG-6#OQ-19 | 是否泛化其他动作粒度（Sleep/Work/Social/Joy 同类问题） | 0.4/US |
| PKG-6#OQ-20 | 粒度偏好放策略层还是注册表层 | DEC-4 条 8 已给 SR 倾向 |
| PKG-6#OQ-21 | 默认值是否写进兼容政策（父关=job 级不允许再翻） | 未决 |
| PKG-6#OQ-22 | Dev 诊断面板（显示当前模式与 `chewToilNameConfirmed`）是否实施 | 未实施 |
| PKG-6#OQ-23 | 是否加 `ThingDef.IsDrug` 排除（啤酒/仙馔父开子关时静默） | 悬置备选（挂 OQ-17） |
| PKG-6#OQ-15..18 | （对接收方 US）两级 vs 3 态卡；子项命名覆盖面；啤酒/仙馔仍响接受与否；回落口径未确认⇒宽松 vs 静默 | SR 已定案、外部开放 |
| PKG-6#OQ-11..14 | US 双开矩阵未实机（四组合「恰好一条声音」+跳过记录）；CI 接线推送前无法实跑（YAML 仅人工校对）；历史/tag 重写 5 `[known-debt]` 需按 5 文件重估+授权；Steam 文案/包核验随解除阻断执行 | 未验证/需授权；矩阵 [xref: PKG-3]、方案 [xref: PKG-5] |

**发布流程与隐私（PKG-4 余 4 + PKG-5 16）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-4#OQ-1 | 0.2.2 Workshop 永停「维护者报告已发布」 | 页面历史不可回采则按 [U] 记账 |
| PKG-4#OQ-2 | 0.2.3 是否实际上传过 | 联动 PKG-4#CNF-7 |
| PKG-4#OQ-3 | 置顶公告删除无带日期复核 | PKG-4#CNF-5 一次复采可关闭 |
| PKG-4#OQ-5 | pack-github 基点校验无记录 | I-7 |
| PKG-5#OQ-1 | 0.2.4 是否重发（不含 version.txt） | 维护者定 |
| PKG-5#OQ-2 | 措施 8（Comments→TODO）是否落地 | 待实现 |
| PKG-5#OQ-3 | CI 加门+SDK 10.0.x 首跑（「已落地」≠「已运行」） | 推送后首跑即真验 |
| PKG-5#OQ-4..8 | 隐私 Q1–Q5：是否执行方案 B；filter-repo vs filter-branch；占位符；GitHub 旧 SHA 残余风险；跨仓通知 | **全未决⇒授权缺口开放** |
| PKG-5#OQ-9 | SR 无 mirror 备份=方案 B P0 阻塞 | 动手前必须先建 |
| PKG-5#OQ-10 | tag 口径（10 还是 9） | PKG-5#CNF-4 并列 |
| PKG-5#OQ-11 | 54 处 hash 引用无逐条清单 | P6 第 12 步不可验收 |
| PKG-5#OQ-12 | force-push 顺序待确认 | 方案 B 内 |
| PKG-5#OQ-13 | 推送窗口协调（重写期间禁止推送） | 单人维护风险可控 [U] |
| PKG-5#OQ-14 | 门实现细节不在 docs/** | GAP-5#1 |
| PKG-5#OQ-15 | 交接序列=两门→本地提交→停在推送前 | 冻结口径 |
| PKG-5#OQ-16 | §1 回写未完成 | 待裁决回填 |

#### 6.3 认识论交叉分账要点（C/I/U 非台账 id，只列跨包影响判断的项）

- **C 面依赖语料外**：UniversalSqueaker 09-06 最小仪式裁决（PKG-4#C-7/PKG-5#ASM-6）；US 仓规则文档现行文本（PKG-3#PKG-3#CNF-1 一方）；维护者「Ratkin 已是 US 首发支持包」口径；Kiiro「已证明」自述；Eat 反馈原文；分支保护声称；0.2.0 自评 8/8/4（一次性）。
- **I 面高影响**：PKG-3#I-3 F1/F2/F3 是预测非已观察事故（不得写进已发生清单）；PKG-1#I-1 「机械判定违规」≠「自动发现违规」（合同无检查门）；PKG-2#I-1 tier 折叠可能是有意命名（`vanilla` 语义边界未定义）；PKG-5#I-2 tag 并集 9；PKG-6#I-3 两块 commit 建议被吸收扩展为三块。
- **U 面硬缺口**：六门通过条数无记录（PKG-3#GAP-7）；Eat/US 双开矩阵零实机；0.2.2/0.2.3 Workshop 态；0.1.x 无任何证据级；tag 10/9；US 基准不可复现；三块提交 hash 未记（`git log` 可恢复，v1 禁取证）。


### 8. 证据缺口（GAP 51 条合并为 19 主题行；原 id 全列）

| # | 问题 | 为什么答不了 | 需要什么 | 原 id |
| --- | --- | --- | --- | --- |
| G1 | 三份合同条文与实现是否逐条一致 | 合同自述 source review 非实测 | 实现-合同对照或 harness 清单 | PKG-1#GAP-1 |
| G2 | `audio.route.selected` 真实输出行形状 | 只有 schema 无样例（禁复制日志摘录） | 已净化样例或机读解析测试 | PKG-1#GAP-2 |
| G3 | 0.3.2 是否正式发行、渠道 | 发布事实不在主源 | PKG-4 渠道矩阵（已闭合 OQ-1#1） | PKG-1#GAP-3 |
| G4 | 诊断叠加层是否暴露真实 fallback 层 | 合同只规定折叠 | 读 overlay 规范/代码 | PKG-1#GAP-4 |
| G5 | Eat 三级模式回归测试守护标识 | 主源只声明要求 | 测试清单或 CI 记录 | PKG-1#GAP-5 |
| G6 | `Toddler` 桶可发行路径 | 合同只记 1.6 无原生 | 跨 mod 兼容评估 | PKG-1#GAP-6；PKG-3#GAP 部分 |
| G7 | settings 是否需版本号/世代字段 | 合同未定义 | settings 持久化规范补充 | PKG-1#GAP-7 |
| G8 | 四份并行架构草稿全文 | 只留比拼表 | 草稿存档 | PKG-2#GAP-1 |
| G9 | 0.3.1/0.3.2 是否按计划过门；0.3.1 实施记录 | PKG-2 止于计划+0.3.0 门槛 | PKG-6 接续+PKG-4 发布评审 | PKG-2#GAP-2；PKG-4#U-5 |
| G10 | C15–C19/C11 缺号真相 | 语料缺席 | 编排方实测已部分闭合（C17–C19/C34–C40 在 PKG-4；C15/C16/C20–C33 仍缺席） | PKG-2#GAP-3 |
| G11 | fixture 9 场景 vs S1-S5+F03-F07 对齐 | 无对照表 | Scenarios.cs | PKG-2#GAP-4 |
| G12 | INC-1 引入提交与暴露面 | 只记修复不记引入 | git log（v1 禁） | PKG-2#GAP-5 |
| G13 | F1/F2/F3 是否实机观察到；选择层是否双服务；CNF-1 US 仓规则现行文本；六门通过条数；U1/桥落地；srdiag v2 冻结；「四层」所指；schema↔US 模型映射；年龄 ABI 冻结记录；「决策文档 §5」身份；Workshop 原地升级平台可行性 | 全为只读审查+推演或语料外 | 双开实机矩阵+skip 日志；PKG-1/2/4 对照；授权读取 US 仓 | PKG-3#GAP-1,2,3,4,5,6,7,8,9,10,11 |
| G14 | 0.1.x tag/zip/哈希/首传包身份 | 无 Claim Pack 体系 | 外部归档 | PKG-4#GAP-1；PKG-4#U-4 |
| G15 | Change Notes 6 条实际内容；0.2.2/0.2.3 Workshop 真实态；Steam 页面时区 | 页面历史不可回采 | 带日期页面复采+三对照 | PKG-4#GAP-2,3,4 |
| G16 | CHANGELOG 排序终裁；8000 字符出处；0.2.0 artifact manifest 所指；item ID 数值全文档分布 | 文本语料内无裁决；分布=隐私规则有意不可答 | 维护者裁决/官方文档/当次会话记录；隐私审计定位（已定位 5 处） | PKG-4#GAP-5,8,9,10 |
| G17 | UniversalSqueaker 09-06 裁决原文；0.3.3 是否沿用 pre1 证据 | 语料外 | 授权读取；未来发布会话 | PKG-4#GAP-6,7 |
| G18 | 门实现细节（断言集合）；5 处债务形态明细；tag 枚举；227/230；54 处位置；CI 首跑；runbook 第 15 条；triage §3；0.2.1 教训 (d) 原件；mirror 与 archive 分支；陈旧包清理 | 主源无载/维护者本地 | 脚本对照/授权/后续会话 | PKG-5#GAP-1..11 |
| G19 | Eat 三态/US 双开矩阵实机记录；0.3.2 跳过终局裁决；隐私债务先前口径；Q1–Q10/Q1–Q4/V1–V3 逐项内容；Steam 评论原文；US 侧进度；三块提交标识 | 语料外/待会话 | 实机会话记录；发布/决策会话；`git log` | PKG-6#GAP-1..7 |


