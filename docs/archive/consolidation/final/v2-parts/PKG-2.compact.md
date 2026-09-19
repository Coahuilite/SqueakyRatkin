# PKG-2 压缩件 v2-part（0.3.x 架构决策与等价验收）— two-pass 第一轮

## 0. 元数据与压缩账本
- repo_rev `4df9594713adbbba91e0aea788a7c7cd3503ab3f`｜frozen_at 2026-09-17｜mode two-pass 第一轮（逐包）｜生成者：提炼 agent
- 输入 `packages/PKG-2-0.3x-decision.md` = 60,861 字符（coverage-check §2）。pass-1 不再以 25% 为硬帽：维护者裁定压缩服务于终审可独立判断。§4–§12 已从 v1 表补全，P0 零丢弃。
- 源简称：决策文档=`docs/0.3x-refactor-architecture-decision-zh.md`；等价评审=`docs/0.3x-equivalence-review-zh.md`；检查表=`docs/0.3x-release-gate-checklist-zh.md`；交接=`docs/handoff-0.3.0-zh.md`（章节/行区间集中见 §10）
- 台账 in/out：DEC 16/16、ALT 8/8（1a/1b/1c/1d/1x/4a/4b/4c）、ASM 15/15、CNF 8/8、OQ 11/11、GAP 5/5、ANCH 10/10、TL 10/10、INC 4/4、EV 16/16、LES 6/6（合计 109/109，零丢弃，仅压表达）。v1 §5 认识论分账 C-1..7/I-1..5/U-1..7 非台账 id，要点并入 §5/§6/§11 对应条目标注。[I-提炼]
- 已知损失：证据叙述过程、mermaid 图体、C# 签名块原文、措辞草案四段原文、冷启动导航、例行命令——均 §12 登记可回查；逐字面 = §9 锚点全部标识符/哈希/计数。
- 只读边界与隐私：仅写本文件；无盘符/绝对路径/日志摘录/凭据/`PublishedFileId` 值；Player.log 只转述事件名+计数。

## 1. 项目上下文
鼠辈啁啾/Squeaky Ratkin（RimWorld mod）0.3.x 实现架构决策与验收账本 [F]。主线：以「内核但薄」把选择语义收进零 Verse 内核，用黄金语料+A–H 门槛把验收变成确定性证据；0.3.0 八面全绿（2026-08-20）、发布动作待授权 [F]；2026-08-22 重定为「SR UI 不变 + US 仓库并行」双仓演化，0.4 同步上架、SR 1.0.0 时 US 转前置 [F]。本包时间层 2026-08-18→09-17；权威序：合同（PKG-1）> 决策文档 > 交接。发布执行事实与前向接续出本包语料 → [xref: PKG-4][PKG-6]；US 数据模型与拆分门本体 → [xref: PKG-3]。C 链本包只可见 C1–C10、C12–C14（C11 缺号；C15+ 不在本包，禁止并写连续序列）。

## 2. 主要时间线
| id | 时间 | 事件 | 当时 vs 现行 |
| --- | --- | --- | --- |
| TL-1 | 08-18 | 决策文档「已接受」：四草稿比拼、方案 A 采纳；`0.3.x` 分支自 dev 切出隔离 0.2 线 | 设计接受→已实施至 0.3.0 并经 08-21/22 重排扩展 |
| TL-2 | 08-18 | 0.x 破坏窗口口径修正：1.0.0=不可破坏承诺起点、真实硬约束两类 | 现行（08-22 再修订冻结口径） |
| TL-3 | 08-18 | 动作门三方向→受控开放定案；同批设置全项等价补强 + A–H 门槛制定 | 现行（落地窗口 08-22 修订，CNF-5） |
| TL-4 | 08-19 | 交接写入：换链完成、任务 15 项 13 done/2 blocked、离线全绿、本地未推送 | 当时态；实机/决策/推送被 TL-6/7 取代（CNF-4/7） |
| TL-5 | 08-20 | 维护者确认路由公理+机械验证；彩蛋定案+同日 YAGNI 修订去运算符 | 现行（08-21 重排、08-22 IsEgg 公开修订叠加） |
| TL-6 | 08-20 | 三项发布决策定案+同日修订（不设 GitHub prerelease tag）；实机完成+后置修复（C12）+八面全绿落表（C13/C14） | 门槛已达成；发布执行仍阻塞（授权） |
| TL-7 | 08-21 | 阶段重排：0.3.1 扩为全分离收口+race/年龄/fallback/彩蛋运行时+XML ABI 定型+全部验证配置；机械验证分阶段落地行入 §2.3；彩蛋并入 0.3.1 同批 | 计划口径；实施证据不在本包（U-4） |
| TL-8 | 08-22 | 密集修订日：作者 XML 面冻结口径；动作门转 US 0.3.x 并行窗口；IsEgg 入公开 ABI；0.3.2 原 UI 专项全转 US；身份门控批准并实现；US repo/packageId 确认、暂不建仓需授权；0.4/SR 1.0.0 双仓同步上架+前置切换+双开七规则+legacy 桥定案且离线原型完成；发布顺序暂不决定 | 本包语料内最新口径 |
| TL-9 | 08-23 | 作者指南正本统一 `.github/skills/squeaky-voicepack-authoring/SKILL.md`（唯一正文）+ `scripts/new-voicepack.ps1`；维护者实机覆盖身份门控关键场景并确认 egg/`pawn_faction`/`pawn_ctrl` 字段 | 现行 |
| TL-10 | 09-17 | (冻结动作) 语料冻结于 repo_rev；行号只对该 revision 有效 | — |

## 3. 关键决策史（DEC-1..16；ALT-1a/1b/1c/1d/1x、ALT-4a/4b/4c）
### DEC-1 0.3.x 采纳「内核但薄」（方案 A）
- 状态：已定案、已实施（0.3.0 换链完成；0.3.1 切核收口为计划面 [U]）。2026-08-18，输入=四份并行架构草稿；裁决主体未点名，按同文档惯例建议为维护者 [I]
- 触发：0.3.x 阶段门=「选择语义正确性门」（per-race 池/fallback/静音/年龄），游戏内验证慢/抖/不可复现；约 500 行内核+200 行 harness 把验收变成确定性单测+可回放语料；「不拆内核技术上也做得了，但阶段门证据质量是真实瓶颈」（ASM-1/2/3）
- ALT-1a 方案 A 内核但薄：强项=阶段门证据确定性、迁移锚点、0.4.x 内核文件夹搬出即 US 本体；弱点=内核纪律负担、0.3.0 一波机械 churn、双份表示心智成本；裁决=**主体采纳**
- ALT-1b 方案 B 契约先行分层（CF-3）：不采纳形态；**吸收硬细节三项**：①迁移事务性（临时 clone 上规范化/去重/校验全成功才替换并 bump schema；失败保留原列表旧 schema；中断以旧 schema 幂等重跑不半迁移）②UI 泄漏修复（`GetTargetCandidates` 只从 assembled `(race,xenotype)` 投影；canonical/har hint 泄漏 0.3.1 修复；HAR 发现仅 dev 诊断）③BabyFits hook 源码核验（RimWorld 1.6 源码一手确认：`MentalFitGenerator.TickInterval` 最终成功调用 `MentalStateHandler.TryStartMentalState(..., transitionSilently: true)`；0.3.1 窄 patch=`TryStartMentalState` 成功 postfix，仅当 `__result && stateDef` ∈ `MentalFitDef` 反向映射验证的 `MentalState_BabyCry`/`MentalState_BabyGiggle` 时通知新动作；No-DLC `Prepare=false`；不得扩大/替代 `MentalBreakWorker.TryStart` hook）
- ALT-1c 方案 C 数据驱动极简：不采纳；diff 最小（约 +1500/−600）但测试性最弱、0.3.1 等价回人工、0.4.x 无编译器强制边界；其 YAGNI 立场被 A 收敛版继承（TimingModel 原地、不拆程序集）
- ALT-1d 方案 D 双内核渐进替换（影子对拍器+黄金语料）：不采纳形态；**黄金语料升级吸收** = 0.3.0 黄金样本断言升级为全矩阵黄金语料（设置场景 × 15 action × 域 × 多种子统计直方图，冻结入库；0.3.1 切核后回放零 delta，并随 Crying/Giggling append 扩为 17 action 矩阵、同提交重建）。ALT-1x=比拼编号占位（§2 表四行）。
- 后果：内核五文件 `Domain.cs`/`Pool.cs`/`Modulation.cs`/`FallbackProfile.cs`/`DomainFilter.cs`〔0.4.x 删〕+ `ActionKey` 边界；适配层 `SqueakFallbackProfileStore.cs`/`SqueakLifeStageResolver.cs`+`tools/KernelCharacterization`（先例 `SqueakLogCharacterization`）；依赖禁环 `Kernel ← {Resolver, Catalog, Settings, CompSqueaker, FallbackStore, LifeStageResolver} ← {UI, Debug, Logging, Patches}`；删除清单（0.3.0 删 `SqueakSoundChoice.Or`、resolver `vanilla` 字典、`ChoosePack`、`ResolvedAudioPack`；0.3.1 删 `ResolvedSqueakContext.Packs`、无 race 域收集路径、`ComposeDomainKey` 两段式；0.4.x 删 `DomainFilter` 与试验开关；整文件删除 0 个、符号内嵌删除约 150 行）；不变（`SqueakTimingModel`/`SqueakVocalCapability`/`SqueakActionDefinitions` 原地；`SqueakXenotypeCatalog` 类名保留=改名纯 churn）；内核接缝全库仅两处生产调用点（`GetRuntimeContext` 与 `PlayOneShot` 内 `ChooseProductionSound`）；vanilla 层迁移（0.3.0 Ratkin 表以 `SqueakActionDefinitions.AudioKey` 播种=Off 音底逐字节等价；0.3.1 合同提升 Fallback 末端=pack fallback→内置 profile→无声，表外 race 无 pack=无声）；黄金语料覆盖=选择结果+合格集合+行为 delta（调制不在语料范围）
- 修订：08-20 追加 DEC-5 机械验证立即生效不分阶段；08-22 DEC-12 内核升级为 US 通用状态、迁移清单=`Kernel/`+0 根文件；无废止。证据 EV-1/2/3；INC-1（吸收项全部落成 0.3.1 硬条款 → LES-6）

### DEC-2 不建独立程序集：内核=源码文件夹+tools 链接编译集
已定案现行（0.4.x 才整体搬 US 新 csproj）；2026-08-18 随 DEC-1，「收敛决定」主体未点名 [U]。独立 csproj 在 0.3.x 的代价（stage-package 只复制 `1.6/`、flavor 常量双份、双构建、0.4.x 重画边界）无对应收益（独立版本/发布/类型隔离无消费者，ASM-4）。纯度强制三件套=harness 链接编译（Kernel 文件引用 Verse/Unity 即编译失败，ASM-5）+codemap 红线+可选 stage-package grep 断言。`Source/SqueakyRatkin/Kernel/` 即编译集；`SqueakAction` 枚举保留适配层设置/存档面（ABI 不动）；纯枚举按领域归属文件（动作域/包域六枚举），不设集中式 `Enums.cs`，namespace 均保持 `SqueakyRatkin`。备选=ALT-1x（B/D 隐含「独立程序集」倾向），语料对 csproj 本身无独立备选记录。

### DEC-3 0.x 破坏窗口与「定型/冻结」口径
08-18 口径修正+08-22 冻结口径修订（ASM-8），主体未点名 [U]。真实硬约束两类：①玩家数据（已保存设置/存档→Scribe 事务迁移 schema bump+幂等；卸载安全=存档零写入）②已发布面（srdiag v1 协议 28 事件/字段序/once key 已随 0.2.x 发布并被 characterization 锁定、扩展走 v2 版本化；`SR_*` SoundDef 键与 PackKey=已发布包引用面）。其余一切 0.x 可大胆改（内部结构/域模型/动作清单〔append-only 序数不变可增〕/未公开 XML 字段），改错成本=修正本身不构成违约。术语：「定型」=0.x 内部冻结（0.4.x US 发布前可修正，代价=内部返工）；「冻结」=已发布契约（修正须版本化兼容）。**08-22 修订**：作者面对外 XML（`SqueakVoicePackDef` 的 `raceDefName`/`scope`/`targetDefName`/`weight`/`fallbacks`/`actions(action,ageTag,IsEgg,sounds)` 与所引 SoundDef 契约）自**首个携带 0.3.1 XML ABI 的发布版本**起公开稳定——字段只增不改、内置 action 键 append-only、validator fail-closed；0.3.1 定型=该面首个内部定型点；「全部可改」旧口径废止；其余内部面（内核/域模型/fallback profile schema/动作门机制）破坏窗口仍延伸至 0.4.x 首版；作者指南在 US 拆分前不公开 fallback profile schema；1.0.0=官方宣告全部公开面不再破坏。

### DEC-4 动作门开放策略：受控开放为主形态
08-18 定案（三方向=三份并行设计；主体未点名 [U]）；SR 0.3.x 仅做两件零成本事，机制本体改在 US 仓库开发（08-22 修订口径）。**共同共识八条**（无论哪条路都成立）：内置 17 枚举 ABI 不动；外部动作走命名空间字符串键（`packageId` 派生，伪造不可能）；`SqueakVoicePackAction.action` 字段枚举→string 但节点名不变（存量包零迁移）；触发=第三方自 patch 自事件调 SR 公开 API（无事件总线、SR 不 patch 第三方）；主线程断言+异常隔离（第三方不能经 SR 打崩游戏）；防刷屏底线（冷却下限+vocal 门）；卸载=DefDatabase 生命周期+既有 orphan/dormant 语义；恶意 mod 无解（可直调 `SoundDef.PlayOneShot`）→诚实安全模型（SR 只保自身漏斗健康）。
- ALT-4a 封闭（Apple 路线）：不采纳为主形态；**机制吸收**=注册闸 fail-closed、内置 fallback 表封闭于内置键。ALT-4b 受控开放（Android 清单+技术化校验）：**主形态采纳**。ALT-4c 全面开放（Android 隐式 Intent）：不采纳；**机制吸收**=dormant 配音（跨 mod 配音是特性、加载序无关回归自动复活）。治理=技术校验+玩家代理+文档礼仪（RimWorld 无商店→无人工审核；ASM-9/10）。
- 七要素：①动作表达=双轨字符串键（内置=枚举名 17 ABI 原样冻结+启动隐式注册；外部=`SqueakActionDef` 注册：defName 语法校验、禁 `SR_` 前缀、键由真实 owner packageId 派生词法隔离不可能碰撞；Def 只带行为元数据〔scope/冷却计划/vocal 策略〕**不载音频**）②触发通路=`SqueakCompat.NotifyAction(Pawn, string actionKey)` public 门面→既有 `NotifyExternal` 漏斗**全闸门复用**（policy/时序/冷却/vocal/池选择/调制与内置同权、无 bypass）+内置键从公开 API 拒绝（防双响）+External-only（无周期模式）+`minIntervalTicks`≥60 硬下限+全局冷却不可豁免 ③校验治理=启动期确定性校验 fail-closed（语法/重复键剔除/元数据完整/冷却底线），pack 引用侧非法 error、内置未知 error、外部未注册 **dormant** ④玩家控制两级=pack 选择（细粒度既有）+外部动作总闸 `allowExternalActions`（默认开；在 `SqueakGlobalActionPolicy` 层，`GetScope(string key)` 外部键先查总闸，关→Disabled；只影响外部键，内置 17 动作与 pack 内置音频零影响；关闭时外部触发走 `RecordOutcome`（ExternalBlocked）不裸 return；UI 设置页可见；srdiag v2 记开关状态/切换事件；`settingsSchemaVersion` additive 递增）⑤卸载语义=零新机制（orphan/dormant 沿用、存档零写入）⑥落地窗口（原文）=0.3.x 只做两件零成本事：srdiag v2 `action` 字段定型=字符串键（一次决策省 v3 协议）+内核动作键出生即字符串；机制本体「0.4.x US 拆分发布时与消费者同窗口落地」；0.3.x 建任何注册机制=YAGNI 违反 ⑦**放弃信号三条**=无第三方表达动作扩展需求→退回封闭（第三方只做 VoicePack）；出现需人工仲裁「标准键」诉求且无法承诺治理→转纯约定或封闭；语料回放非零 delta（语义漂移实证）→撤销键迁移（→OQ-6）。
- 受控开放=**八道闸门链**（每道独立可调、任一关闭 fail-closed 静默、无 bypass）：注册闸→所有权闸→线程闸→触发闸（spawned/CurrentMap/视口复用）→冷却闸→vocal 闸→玩家授权闸→玩家总闸。三扇门协同：外部动作音频=任意 VoicePack 按注册键声明；内置 fallback 表封闭于内置键（维护者保底不开放）；外部动作无 pack=无声（无 SR 兜底）。关键前置：Kernel 出生即用字符串动作键（`ActionKey.For(SqueakAction)` 单一映射点）→迁移成本归零。**08-22 修订**：机制本体改在 US 仓库 0.3.x 并行窗口开发、0.4 随 US 首版发布；SR 0.4 只做 bugfix 并同步反馈 US；旧表述按新路线理解（表体未改写，CNF-5）。srdiag v2 `action` 定型细则见 DEC-14。

### DEC-5 路由核心公理+机械验证设计约束
08-20 **维护者确认**（C-1：语料内声明无独立佐证）；08-21 增补分阶段落地行。对全部新增决策逻辑立即生效不分阶段（「公理约束一切分层决策」）。路由（域键/池/链形状/回退序/抽取）=系统核心；触发闸门/时机/vocal/音频分配闸门/调制/诊断=叠在路由上的执行层，允许适度耦合但须满足**四条公理**：①链唯一事实源（链形状/层级顺序/回退语义只有路由核心一个所有者；执行层不得旁路、重定义或部分复制链逻辑）②依赖方向单向（执行层→路由核心禁止反向；执行层之间避免互耦，参照 `SqueakGlobalActionPolicy` 零下游依赖模式）③只经注入面影响候选（只允许作为路由输入 mode/ctx 或注入函子过滤候选〔`ISoundGate` 模式〕；闸门不选 tier、不改链形状）④语义决策下沉核心（凡「决定放什么声音」的语义尽量下沉路由核心；执行层只留机械 I/O 与少数纯策略函子=机械验证覆盖面最大化前提）。**机械验证约束四条**：新决策逻辑出生即纯（零 Verse 链接编译、与内核同一 harness；已存在于 Verse 文件的纯逻辑如 `SqueakTimingModel` 在改动它的窗口同变更提取）；新动作通路必须配语料/断言场景或显式声明 out-of-scope（等价评审 §4 模式升级为固定检查项）；内置动作五处同步（枚举/XML/SoundDef/本地化/统计）由离线一致性断言机械检查；分阶段落地（08-21 重排）=五处同步断言+漏斗纯逻辑提取（TimingModel/TriggerInvocation/ActionPlan）**0.3.1**、0.4.x 动作门八道闸门决策**出生即纯**。备选无记录（直接确认无比拼）；DEC-6 实施形态被划入规则 3 合法区；DEC-16 末条引本公理。

### DEC-6 彩蛋语音裁决（field-presence+默认关+加性池成员）
08-20 **维护者定案**；**同日 YAGNI 修订：去掉运算符**；08-21 重排并入 0.3.1 同批；08-22 公开 ABI 修订；0.3.2 日志适配已实机（08-23 确认）。裁决：彩蛋标签=**field-presence 标记，无运算符、无条件表达式**（YAGNI：种族定位由条目所属 pack 的 `raceDefName` 路由声明承担，不设彩蛋专属条件）；开关**默认关**（彩蛋级别=非承诺面、玩家显式开启）；开=带标签条目**加入**所属 pack/域候选池与普通条目同权混抽，关=候选池只含普通条目（例：Kiiro 征召——开=猫哈气入池、关=仅正常征召音；Kiiro 域由 pack 声明路由而来，与彩蛋无关）；彩蛋条目=**加性池成员**（不是替换、不是独立层级、不触碰链形状〔规则 1/3〕）。实施形态（规则 3 合法区）：条目级 `IsEgg` 标记；开关状态作为路由输入随快照（切换走离散 resolver 重建）；内核候选过滤=彩蛋资格→playability→抽取；无谓词编译、无表达式求值。被去掉的「运算符/条件表达式」前版方案细节语料未记（U-1）。**08-22 修订**：`IsEgg` 自 0.3.2 起纳入公开作者 ABI（作者指南已公开、validator fail-closed 已覆盖）；旧口径「0.3.x 内为维护者内部标签、第三方作者指南 0.4.x 才开放」**明文废止**（CNF-6）。Kiiro 许可门不变：机制种族中立，Kiiro 彩蛋内容沿用「实验分支不发布、未经许可不宣传」。后果：0.3.1 同批落地（schema bump 一并）+语料加彩蛋维度（开关×三模式）；0.3.2 发配日志适配（维护者要求）：`ChainResult.IsEgg`→`SqueakSoundChoice.IsEgg`→`SqueakDebug` 装配器；成功路径不再并列发 `audio.dispatch.ok`(v1)+`audio.route.selected`(v2) 两条、只发一条 v2 明细（字段串 `sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`；`pawn_faction=<FactionDef.defName>`、`pawn_ctrl=player|nonplayer` 二值；人类句 `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`）；v1 28 事件与 registry 字节不动、v2 characterization 同步；新增 `dist/SqueakyRatkinEggTestVoices/`（gitignored，单 Select 蛋条目+开关两态验收矩阵；verify 第 10 项在产物存在时并入校验）。实机 EV-12：egg=true 5 条/egg=false 对照 11 条零红字；player 10/nonplayer 26 无 Select nonplayer 泄漏。

### DEC-7 0.3.0 等价验证体系（设置全项第一原则+三层证据+14 行+诚实边界）
已定案并已执行（C4/C5/C7；等价评审随提交入库）。**第一原则**（0.3.x 全窗口）：已发布功能=玩家可能使用→设置全项验证，禁止抽样或「冷门项豁免」；**唯一豁免=dev 隐藏功能**（七次点击版本号解锁面：`developerToolsEnabled`/`devLoggingMode`/`localizeDebugActions` 等——隐藏、非公开承诺面，坏了维护者自行发现〔自身使用+dev 日志可见〕，不占等价验证门）。换面项仅 4：`voicePackMode`（Off=仅 vanilla/Fallback=xeno→race→vanilla/Remix=三级等权→Select mode 链形状表）；`voicePackSelections`（选择集→池成员→域池 `VoicePackEntry[]` PackKey 序数稳定序）；隐式 vanilla 兜底（C# 字典 `SR_*`→内置表种子=`SqueakActionDefinitions.AudioKey`）；playability/clip 状态、pawn/xenotype 身份、Rand（`HasPlayable`→Rand→`ISoundGate`/`IRollSource` 同一实例）。不换面项（原地仍须审计零语义改动）：`globalActionEnabled`/scope、`xenotypePresets`、`moodOverrides`（`ResolveMoodMod`）、`scaleCooldownWithTimeSpeed`/`globalCooldownMultiplier`/启动相位（TimingModel）、`scaleFrequencyWithTalking`/vocal、`scalePeriodicWithAudiblePopulation`、`distancePreset`/`distanceRange`（`ApplyDistanceRange`）、Debug/日志设置项、`voicePackSchemaVersion`/`settingsSchemaVersion`（序列化面，0.3.0 不 bump 不迁移）。
- 三层证据（**诚实边界：非同 seed 逐项对拍——那需双实现=方案 D，已否**）：①语义规范（评审锚）：旧实现每设置项→算法语义规范（链形状三态、过滤顺序 HasPlayable→年龄→抽取、等权/带权累计权重、排序键 PackKey 序数），新实现逐条对照，评审随 0.3.0 提交=0.3.1 切核对照基线 ②黄金语料全场景矩阵（机器）：mode{Off,Fallback,Remix}×选择{无/仅Example/Example+Xeno/orphan/dormant}×Biotech{无/有选择/无选择}×15 action×多种子，换链后固化 ③分布与实机（统计）：等权分布有界断言（多种子直方图）+实机 fixture resolver 计数/触发统计 vs 0.2.4 基线。三个已知漂移点（点名缓解）：Remix 折叠（旧「三级各等权再折叠」vs 新 `MixedTier` 等权合成——折叠顺序/去重/权重归一须精确复刻；语料加密场景）；Off 键等价（vanilla 字典 vs 内置表——逐 action 键比对进验证门、不假设）；池排序与稳定键（旧收集顺序 vs PackKey 序数——分布等价成立但 `PoolStableKey`/确定性语料依赖列表顺序；语料按新排序固化〔自洽〕、旧基线靠分布断言衔接；语义规范写明防 0.3.1 切核误判回归）。
- **14 行语义对照（等价评审 §2，全部判「等价」）**：1 Off 分支→`SelectBuiltIn`（键源=`SqueakActionDefinitions.AudioKey` 单源投影）；2 Fallback 三级短路；3 Remix 固定序 [xeno,race,vanilla] 等权折叠；4 无 xeno pawn（旧 globalContext Packs 空→x tier 恒 None）≡`ctx.Domain.Xenotype != null` 才查 xeno 池；5 pack 级过滤≡entry 级过滤（`TryGetAction+HasPlayableKey`）；6 池内等权 `Rand.Range`≡`DrawEntry`（0.3.0 全 Weight=1；分布等价、roll 序列自洽）；7 pack 内 `pack.Choose`≡`DrawSoundKey`；8 `Sort(PackKey Ordinal)`≡DomainPool 构建序；9 pack 声音构建（非 null、去 `_Preview` 后缀、Distinct、defName Ordinal 排序）投影规则留适配层保持；10 vanilla 兜底 `GetNamedSilentFail`（缺失=无声）≡内置表种子 15 动作全列（逐 action 键等价由单测锁定）；11 无声语义≡`ChainResult.None` 传播；12 `PoolStableKey`（pack 命中=PackKey、vanilla=null）；13 全年龄（`AgeTag` 全 null、Select 不受 ctx.Age 影响、单测锁定）；14 域键（scope+targetDefName 字符串→`AudioDomain(RaceKey, XenotypeKey?)` 值类型、适配层唯一转换点；0.3.0 注入 `(Ratkin,*)`、0.3.1 迁移后启用完整域面）。
- 诚实声明三条（§4）：①非「同 seed 逐项对拍」：`Rand.Range` 与内核 `floor(Next01()*N)` 分布等价但逐次抽取不同；玩家承诺=同池同分布（0.2.x 本就随机）、语料按新 roll 序列固化（自洽基线）；逐项对拍须双实现（方案 D 已否）——ASM-12 ②池排序差异面：旧 `GetSelected` 只对选中 pack 排序、内核 DomainPool 对所有注入条目排序——0.3.0 注入面=旧选择面无行为差异——ASM-11 ③0.3.0 不启用：年龄优先级、pack fallback、带权（Weight≠1）、`ClassifyDomainStatus` 完整接线（0.3.1）——类型与纯逻辑已入库、未接线不构成行为面。
- 基线地位：换面 4 项逐条承接+不换面零改动审计通过+dev 豁免单列；语料生成+回放零 delta；0.3.1 切核以本文档+语料为回归基线；任何语料 delta=回归，先查实现后查场景构造（`Scenarios.cs` 冻结不得改动）。例数双层：1622（C4 口径）→3782（C7 后）勿合并（CNF-3）。fixture 编号矛盾保留：「fixture 9 场景」vs 编号面 `S1-S5`+`F03-F07`（5+5=10 个编号）映射语料未给（U-6/GAP-4）。

### DEC-8 0.3.0 发布门槛 A–H（八面全绿才推 Steam）
08-18 制定、08-20 八面全绿落表（执行记录=维护者回机+日志核验）；发布执行仍阻塞于授权（OQ-1）。背景：Steam Workshop 自动更新=发布即全员无灰度；0.3.0 必须「安装后体验与 0.2.4 无差别」否则玩家当 bug 报。八面（保证/证据→结果）：A 听感（同 pawn/动作/心情/设置→同 SoundDef；黄金语料回放零 delta）→绿；B 时机（间隔/概率/冷却/全局冷却/启动相位/倍速；TimingModel 原地+characterization 对齐）→绿；C 设置/存档（0.2.4 设置读→写字节一致、无迁移执行、UI 行集合相同；fixture 字节 diff+UI 快照断言）→绿；D 日志（v1 28 事件/字段序/once key/human 文案零变化、无新错误事件；双 flavor characterization）→绿；E 距离/调制/人口/vocal（全零改动；改动清单审计）→绿（审计）；F Def/内容（`SR_*` 键、Example 音频、stage 镜像不变；stage-package SHA256 校验）→绿；G 性能（触发路径无劣化、内核持平或优于旧 `ChoosePack`；实机对比〔可加 dev 计数〕）→绿（论证）；H 依赖/降级（No-DLC、Biotech dormant、HAR 缺失、Harmony 版本；受控 DLC 全关基线实测）→绿。门槛全绿与同日**后置修复**（INC-1/2/4）并存→LES-1；证据 EV-4~EV-11、链 C8/C12/C13/C14。

### DEC-9 三项发布决策（双轨/措辞/热修；08-20 定案含同日修订）
主体=维护者 [I]（「维护者自选渠道」「不设…tag」措辞）；发布执行仍阻塞于授权。①双轨：试用载体=**本地 dev 包**（`build-dev.ps1` 产物；dev/steam 包均本地纪律、CI 不参与，GitHub CI 只为 GitHub 包服务），维护者自选渠道分发自愿测试者（社区玩家不依赖 GitHub Release 页）；**不设 GitHub prerelease tag**；观察窗 2 周起、无实质差异报告→按既有 dev→main→GitHub 打包（`release.yml` tag 流水线）→Steam 正式流程发布；试用包 Dev flavor（dev 日志默认开〔反馈诊断有利〕、dev-only 设置可见〔分发时告知测试者〕）。②措辞=内核重构+拆分预告方向：不提拆分出的模组名称、只预告「将拆分为独立前置 mod」；**维护边界：不提 US 名称/多种族/Kiiro/通用化**；页面义务=Workshop 旧公告「预计 0.3.0 失效」随正式发布同步替换（中英双预览+字符数重算）；四段文案原文归 PKG-4 [xref]。③热修（**方案 a**）：真回归→`vX.Y.Z-hotfixN`（严格 SemVer prerelease 后缀、`x.y.z` 三节不变；`release.yml`/`pack-github` 正则已支持；GitHub 侧自动 prerelease 标记=已知代价）；hotfix 不 bump z、内容在下一个 z 版本正式合并；双渠道同步（GitHub 随 tag 自动、Steam 人工上传）；不提前 0.3.1（避免污染 race-aware 迁移计划）。备选：GitHub prerelease 双轨（交接 §6.2 时代预设「GitHub prerelease → Steam 正式」）被 08-20 同日修订否定（CNF-4）；热修对照方案 b/c 语料未记 [U]（U-2）。

### DEC-10 0.3.1 计划重排：全分离收口+race/年龄/fallback/彩蛋运行时+XML ABI 定型+全部验证配置
08-21 重排定案（主体同 DEC-1 [I]）；计划状态、执行结果不在本包主源 [U]→[xref: PKG-6]。四组：
- 全分离（内核自足）六项：`FallbackProfile.SoundKeys` 键→字符串键+`Kernel/BuiltInActionKeys`（17 键唯一权威）+`TryGetSoundKey` 纯字符串查表（键∈清单校验）；`Select` mode 改内核自有枚举（SR 侧一处映射，同 ActionKey 模式）；`ActionKey` 移入 `Kernel/`；`SqueakActionModel` 退出 harness 链接清单（SR 产品常量不入内核编译单元）；迁移清单=US 编译集 `Kernel/`+0 根文件；临时层拆除（4 处 `Ratkin` 字面量〔含 resolver `Choose` 域构造〕+`ComposeDomainKey` 两段式+`ResolvedSqueakContext.Packs`/无 race 域收集等旧路径同变更删除）。
- race-aware：`SqueakVoicePackDef.raceDefName`（必填语义）+validator+Example 声明；catalog DomainFilter 闸+域化收集+`GetTargetCandidates` assembled-only 投影；事务性 Scribe 迁移（`settingsSchemaVersion` 3→4、`voicePackSchemaVersion` 1→2，fixture 先行）；srdiag v2（`SettingsOrigin`+race/xenotype 身份+域拒绝事件+`action` 字段定型字符串键）；试验名单开关（隐藏、替换非叠加）；两处接缝切核；外来 race per-race 池端到端实证（合成输入，不进交付）。
- 年龄/fallback/彩蛋运行时：`ageTag` 解析+内核年龄优先级（exact age entry→all-age entry）；`SqueakLifeStageResolver` 直映 `CurLifeStage.developmentalStage`（Newborn/Baby→Baby、Child→Child、其余→Adult；Toddler 无 1.6 原生对应、不按年龄阈值重算，ASM-14）；`Modulation.ComposeModulation` 接入 `ResolveMoodMod`（与 `SqueakMoodMod` 同构叠加、独立轴、不写 `if (age == Baby)`）；pack 级 `fallbacks` 入链（Fallback 末端→pack fallback→内置 profile→无声）；`BuiltInFallbackCatalog` 正式数据+`SqueakFallbackProfileStore`（Config 副本单写者 temp+atomic、`DecideCopy`/`Merge`）；Crying/Giggling append 15/16+`TryStartMentalState` 窄 hook+五处同步（默认无内置 Def=静默、pack 声明才发声、内置表不列条目；`MentalBreakWorker.TryStart` hook 保持真实崩溃限定不动）；彩蛋 `IsEgg` 落地（DEC-6）；`raceDefName` 缺省默认删除（validator 硬要求）。
- XML ABI 定型清单（同批合同提升）：17 动作 ABI、`raceDefName`/`ageTag`/`fallbacks`/`weight`/`IsEgg` 字段名与语义、内置表 15 键格式、life-stage 直映口径、Fallback 末端序、fallback 写通道工件化（Config 副本不经 `WriteSettings`）。全部验证配置：五处同步一致性断言；漏斗纯逻辑提取（`SqueakTimingModel`/`SqueakTriggerInvocation`/`SqueakActionPlan`→纯文件+harness 链接）；语料 17 动作矩阵重建+彩蛋维度（开关×三模式）；per-race 池隔离 harness（两 race 池互不串扰）；Config 三场景 harness（缺失/损坏/版本低→重建；delta 合并；重置覆盖）；v2 characterization；`SqueakyRatkinSettings` 拆 partial ×3（ExposeData+迁移/运行时桥分离，随事务性迁移同窗口，Scribe 契约面存档 fixture）；决策文档 §3.1/§4.1/§5 与合同、作者指南同步修订。
- 验证门（逐项）：迁移幂等（fixture 加载→迁移→再加载不再变）/重启/失败不丢（事务性）；语料 17 动作回放零 delta；年龄优先级单测（exact→all-age）；per-race 池隔离；实机 immediate publish、350 ms 合并保存、close flush、No-DLC dormant 降级（全程不碰 Xenotype 路径）、orphan/dormant 语义；UI 泄漏断言（race 行=={Ratkin}、catalog 非装配域条目==0）；BabyFits 实机只产新 action outcome、不产 MentalBreak outcome；fallback 删 pack→内置表发声、17 action 逐一路由验证（Crying/Giggling 无内置条目走静默）；五处同步断言绿；v2 characterization 绿；三 harness+双 flavor 0 warning。
- fallback 机制配套（§4.6）：`BuiltInFallbackCatalog`（内核 C# 单源、编译期冻结）创建 `BuiltInFallbackTable`（race→动作字符串键→SoundDef key+内容版本；Ratkin 表固定 15 键 Call…MentalBreak，Crying/Giggling 无内置音频则不列条目；`For(RaceKey)` 装配经 DomainFilter）。`SqueakFallbackProfileStore`（适配层）：启动链 DefDatabase 就绪后按 packageId 读写 `SqueakyRatkin_Profile_<race>.xml`（field-presence delta，复用 `XenotypeMoodOverride` 模式）；`DecideCopy`（内核纯逻辑）：缺失/损坏/版本<单源→RebuildFromSource；有 delta 且版本齐→MergeDelta；否则 KeepCopy；写入=单写者 temp+atomic replace、try/catch（失败→dev 日志+内存态继续）、不参与 350 ms ModSettings 队列、**不调用 `WriteSettings()`**——独立正式工件（设计笔记已锁定），`base.WriteSettings()` 单通道红线只约束 ModSettings。玩家入口（fallback 路由编辑器/主动重建按钮/原版音频浏览器下放）0.3.2 Ratkin 域内正式化→08-22 UI 面转入 US（DEC-12）。

### DEC-11 0.3.2 计划重排：原 UI 专项转 US+身份门控+XML ABI 固化+彩蛋日志适配
08-22 重排（标题「原 UI 专项转 US 仓库，SR 0.3.x UI 不变」）；身份门控=维护者批准并已实现；日志彩蛋适配=维护者要求。①**玩家触发内容身份门控**：`PlayerSelection`/`ActiveCommand` 来源要求 `Pawn.IsPlayerControlled`；`PlayerSelection` 另要求可响应=`!Downed && Awake()`；gate 位于 `NotifyExternal` 既有触发闸层（plan.Configured 后、TryTrigger 前）、fail-closed 静默；纯标志（`IsPlayerInitiated`/`RequiresResponsivePawn`）进 `SqueakTriggerInvocation`+`TriggerInvocationRules` 单测；Verse 采样声明 out-of-scope；实机矩阵见 TODO（语料外，U-7）。**`Selector.Select(playSound:false)` 已定案=过滤**（1.6 源码核验：全部 `playSound:false` 调用点均为复活/区域创建/培养舱/基因提取/传送等程序性后续选择、非玩家点击反馈；patch 经 `__1` 位置注入过滤；EV-11）。②**XML ABI 固化**（作者面优先）：合同新增「VoicePack 作者 XML ABI」公开稳定面（节点表/默认值/validator 规则/兼容政策：字段只增不改、action 键 append-only、fail-closed）；§1.1/§2.4 口径修订；`verify-local` 新增第 10 项 XML ABI 一致性锁（示例 XML×validator×作者指南三向对照，含 C# 17 键顺序与彩蛋测试包）；作者指南升格 ABI 合同→08-23 正本统一 `.github/skills/squeaky-voicepack-authoring/SKILL.md`（唯一正文、人类可读优先、兼作 agent skill、自包含：不依赖外部文档/模板、仅允许专用脚本；原 docs 指南删除）+同批 `scripts/new-voicepack.ps1` 脚手架。③发配日志彩蛋适配+dev 日志重排简化（见 DEC-6）。④`action` 枚举→string、动作门机制本体在 US 仓库 0.3.x 窗口实现、0.4 随 US 首版发布（对作者 XML 不可见、零迁移、SR 0.3.x 不提前）。⑤**发布顺序暂不决定**（08-22）：ABI 文档按「首个携带 0.3.1 XML ABI 的发布版本」表述（OQ-5）。验证门执行：身份门控矩阵（清醒可控响；睡眠/倒地/精神崩溃/敌对/访客/野化静默；任务加入与奴隶响；Draft/Equip/Attack/周期零变化；`playSound:false` 程序性选择静默）——08-23 维护者已覆盖睡眠/非玩家 pawn/精神崩溃玩家 pawn 静默、余项「自然覆盖」[C]（源文标注→OQ-11；门控全量矩阵另见 OQ-10）；XML ABI 一致性锁绿（含彩蛋测试包）；彩蛋两态路由+日志矩阵 08-23 实机确认（EV-12）；双语料/五处同步/v2 characterization 零 delta。

### DEC-12 0.3.x 路线重定：US 仓库并行、SR UI 不变（08-22）
已定案（现行最新口径；建仓待授权）。①SR 0.3.x 期间 **UI 保持不变**；原 UI 专项（fallback 路由编辑器、重建按钮、原版音频浏览器下放、Race 域编辑器、Debug 入口合并、三档缩放验证）全部转入 US 仓库开发面。②SR 内核升级为 US 通用状态，**新开 US 仓库**并行开发：已确认 repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker`；命名空间/前缀/日志按推荐组合 `UniversalSqueaker`/`US_`/`usdiag`；Workshop 显示名与许可**待最终确认**（OQ-3）；**08-22 暂不建仓，仓库创建为外部操作需另行授权**（OQ-2）。③SR 0.3.2 功能调整与 XML ABI 固化按 DEC-11 执行；作者 XML ABI 随内核进入 US 后由 **US 继承并继续履行公开稳定承诺**（首个携带 0.3.1 XML ABI 的发布版本起冻结）。④opt-in 框架与动作门机制本体在 US 0.3.x 窗口实现、不在 SR 0.3.x 出现。触发信号 [I]（I-4 累写结构）；直接动因语料未展开。后果：US 拆分/迁移/退役专门设计→[xref: PKG-3]。

### DEC-13 0.4/SR 1.0.0：双仓同步上架、服务域共存、前置切换与 legacy 桥（08-22 定案）
前瞻计划已定案；多条验证门未执行（OQ-9）。**0.4**：US 首版与 SR 0.4 同步上架创意工坊、US 暂不作 SR 依赖；共存策略（定案）=可同时启用+服务域划分：0.4 期间 SR 独占 Ratkin（保留 DLL 与既有 XPath 挂载）、US 0.4 不含任何 Ratkin 装配/profile/attachment 只服务其他种族；两 mod 不写 `<incompatibleWith>`；打包门断言 US 0.4 无 Ratkin 条目、实机双开矩阵验证「同一种族只经一方单响」；SR 0.4 只做 bugfix、触及共享面的修复同步反馈 US、不引入新机制。**双开冲突防御硬规则七条（定案）**：①US 0.3.x–0.4 禁止定义 `SqueakyRatkin.*` 类型（GenTypes first-wins+ignoreCase、同名类型静默错解析；legacy 桥只在 SR 无 DLL 的接管版本引入）②程序集/命名空间/Harmony id/Def 前缀/日志前缀/Config 文件全分离（`UniversalSqueaker.*`/`US_`/`usdiag`/`UniversalSqueaker_*` vs SR 对应面）③双方事件 patch 只操作自身 comp、`GetGizmos` 等包装前先判 `GetComp<OwnComp>() != null` ④US 的 DebugAction 方法名与翻译键全部 `US_` 前缀、SR 现有通用键列入 US 保留禁用清单 ⑤双方只对自身 catalog 内 SoundDef 改写 distRange ⑥启动时各记一次 `coexistence=*_active` 日志（只读检测、不做硬禁用）⑦打包/verify 机械门断言以上规则。**SR 1.0.0**：下一版本直接从 0.4 推进到 1.0.0、SR 退化为纯音频包（内容仓库）；此时 **US 成为 SR 前置依赖**；1.0.0=作者 XML ABI 官方冻结版本；US 该时点版本号待定（OQ-4）。**作者包 legacy 桥（定案：零迁移、不污染内核、随时可拆）**：US 接管版本提供 `SqueakyRatkin.SqueakVoicePackDef` 薄继承 legacy 类型（仅 root 类型；字段全继承自 canonical，Verse `XmlToObjectUtils.SearchTypeHierarchy` 已核验可填充继承字段，ASM-15）；`LegacyVoicePackSource` 单点 upcast 进既有 validator/`SqueakKernelAdapter`，内核与 catalog 不引用 legacy 命名空间；legacy 包按 `SR_` 前缀、canonical 包按 `US_` 前缀校验；拆除=删 `Legacy/`+一个调用点。离线原型已完成（08-22）：`tools/LegacyBridgePrototype/`（真实 Verse API 编译期证明）+`tools/LegacyBridgeHarness/`（共享桥源运行时语义：层级填充、前缀上下文、AllDefs 枚举+单点 upcast、拆除面隔离；EV-13）；真 Verse XML 加载/交叉引用解析**待维护者实机**（OQ-8）。**动作门 US 落地**（DEC-4 承接）：`SqueakActionDef`/`SqueakActionRegistry`/`SqueakCompat` 新增；`SqueakVoicePackAction.action` 枚举→string（节点名不变、存量零迁移、`voicePackSchemaVersion` 递增）；validator 改键解析（内置 known/外部未注册 dormant/非法 error）；作者指南动作注册章节随 US 0.4 公开；黄金语料同提交重建、内置 17 键矩阵回放零 delta=验收门。**迁移验证**（SR 1.0.0 切换前置时执行）：US/SR 设置、保存、Workshop 迁移演练（真实 save modlist）；六项拆分门逐条可重复证据（本体→[xref: PKG-3]）；`ProductDomainFilter` 在 US 中按通用域机制移除试验名单；`Kernel/`+纯枚举文件原样搬入 US csproj、SR 收缩为 VoicePack 依赖 US；**禁止在 US 首版前发布空壳前置或保留双实现**。

### DEC-14 迁移与兼容政策（Scribe 事务性/fixture 策略/srdiag v1→v2）
设计规范现行有效（0.3.0 未迁移、0.3.1 批次执行；08-18 随决策文档）；事务性形态继承方案 B 吸收项（DEC-1）。**Scribe schema（事务性）**：`VoicePackSelectionRecord`=`scope, raceDefName, xenotypeDefName, enabledPackKeys`；`XenotypePresetRecord`=`raceDefName, xenotypeDefName, field-presence deltas`；旧 XML `targetDefName` 只作 load-only 读取源、不留运行时 alias。映射：旧 Race selection→`(Ratkin, "")`；旧 Xenotype selection/preset→`(Ratkin, <旧精确 defName>)`；last-wins 顺序保留；未知 PackKey 保留 orphan；不可用目标保留 dormant；**不静默删除**。事务：临时 clone 上规范化/去重/校验→全部成功才替换正式列表并 bump schema→只置 `migrationPersistencePending`、主线程 startup 才 `QueueSettingsSave`→实际写入唯一走 `base.WriteSettings()`；失败=原列表与旧 schema 原样保留+迁移失败日志；重启以旧 schema 幂等重跑不半迁移；卸载安全不变（存档零写入、迁移只动 Config/设置）。**Fixture 策略**：实现前保存真实 0.2.4 ModSettings XML fixture（不手写理想样本）：无 schema 节点新装/显式 Off/Fallback+内置种子/多个旧 Race+Xenotype selection/重复 last-wins/消失 PackKey/Biotech inactive/损坏缺字段；每 fixture 跑 load→migrate→publish→serialize→reload→migrate 断言第二次无变化、异常路径断言原列表/旧 schema 保留；profile 另有：缺失/过旧/损坏/玩家 override/恢复 source 版本；0.3.0 阶段另有「0.2.4 fixture 读→写→diff 字节一致」证明未迁移前无 ABI 变化。**srdiag v1→v2**：0.3.0/0.3.1 不改 v1 registry/28 事件/字段序/once key/human sentence；v2=协议头 `fmt=2`：`SettingsOrigin`（FreshCreated/LoadedFromFile）；固定 v2 字段序加入精确 `race`（有异种加 `xenotype`）；route/profile 事件记 domain/tier；v1 解析器继续接受旧记录；**race/xenotype 只写 DefName，绝不写 label/HAR 包名/玩家可变文案**；异常/脱敏/百分号编码/DevOnly 门控沿用；once key 前缀 `log-v2`；不临时塞 v1 字段；**v2 `action` 字段定型=字符串动作键**（内置=枚举名与 v1 字节一致；外部=`packageId.defName`，百分号编码白名单含 `.` 免编码直写）——0.3.1 与 v2 同批定型，一次决策避免未来外部动作再开 v3 协议。

### DEC-15 风险登记册（八项，随 DEC-1 建立，常设）
08-18；#1/#3/#7 已有实机或门证据、#5 部分（合成外来 race 隔离测试列为 0.3.1 验证门）、#8 面向未来 US。风险→缓解：1 0.3.0 行为等价破坏（链重写+枚举提取）→黄金语料断言+`SR_*` 种子（音底逐字节等价）+双 flavor characterization+实机 fixture 快照对比；2 内核纯度纪律松弛（单 csproj 无编译器强制）→KernelCharacterization 链接编译即纯度门（引用 Verse 即失败）+codemap 红线+可选 stage-package grep 断言；3 Scribe 迁移破坏旧选择→fixture 先行+事务性提交+幂等+fail-closed（失败保留旧记录）；4 双表示漂移（kernel string-key↔adapter SoundDef）→单一转换点在 catalog 快照构建+黄金语料回放兜底+IRollSource/ISoundGate 共享同一实例；5 跨 race 池泄漏（旧全局池复活）→`AudioDomain` 不可变池键+assembler 单入口+合成外来 race 隔离测试+catalog 非装配域断言；6 0.3.1 定型失误（动作/年龄/链末端语义）→ageTag field-presence（第三方包零迁移）+定型前 XML 评审+单测锁优先级语义+append-only 序数（定型后至 US 发布前仍可修正，代价=内部返工非契约违约，§1.1 窗口定位）；7 UI 泄漏非装配域（canonical/har hint）→`GetTargetCandidates` assembled-only 投影+每阶段 UI 无泄漏断言（行集合快照对比）；8 0.4.x 动作门落地回归（键 string 化/注册校验/玩家总闸）→内置 17 键语料回放零 delta 为验收门+注册/总闸/卸载全走既有 orphan/dormant 与 policy 模式+放弃信号见 DEC-4（机制可删，OQ-6）。INC-1 属风险 #1 家族残余（语料全绿未覆盖 BuildFallback 路径）。

### DEC-16 实施纪律（常设规范）
§8 六条（08-18）：每阶段只引入一条活运行时路径、旧路径在迁移完成的同一变更中删除、不用长期 shim；每引入一个类型/接口必须能指出 0.3.x 内真实消费者、不为 0.4.x 预建任何机制（程序集边界/独立版本/US 数据政策留到 0.4.x）；阶段门证据必须机器可复现（harness/语料/断言）、「编译通过」与「实机听了一下」不构成完成；实现前将受影响结论提升进 `project-architecture-contract.md`（0.3.1：域键与 selection 语义、17 动作、选择链末端、fallback 工件写通道）与设置合同，作者指南/SKILL 在 US 拆分前不新增 0.4.x 外部动作注册 schema 章节；0.x 窗口利用（结构改动优先本窗口完成；1.0.0 起任何公开面〔Scribe 格式、srdiag 协议、XML 包 schema、枚举序数、`SR_*` 键〕变更必须版本化兼容；玩家设置/存档与已发布协议即使 0.x 也走迁移兼容不做裸破坏）；新增决策逻辑遵循 §2.3（出生即纯、场景配通路或声明 out-of-scope、五处同步断言；「路由是核心、其余是叠叠乐」公理约束一切分层决策）。**红线**（交接 §7，08-19，下会话不得违反）：`SqueakAction` append-only 序数稳定（0.3.1 末尾 append Crying/Giggling）；srdiag v1 协议冻结；`SR_` 前缀 defName 契约；存档零写入；`base.WriteSettings()` 单通道；HAR 反射非依赖（缺失静默降级）；No-DLC 基线；已发布功能=玩家必用→设置全项验证（唯一豁免 dev 隐藏功能/七次点击解锁面）；语料场景构造冻结（`Scenarios.cs`）；任何 Kernel 改动必须过 KernelCharacterization 全绿；fixture 生成器链接的 0.2.4 文件移动时同步更新 csproj 链接；**提交/推送/发布需显式授权；推送前隐私审查完整可达范围**。

## 4. 假设及其演化（ASM-1..15）
| id | 假设 | 提出 | 仍成立？ | 变更/验证 |
| --- | --- | --- | --- | --- |
| ASM-1 | 阶段门证据质量是真实瓶颈；不拆内核技术上也做得了 | 08-18 | 成立 [I]（0.3.0 按语料+harness 完成） | DEC-7 |
| ASM-2 | 约 500 行内核 + 200 行 harness 足以抽取 | 08-18 | 未知 [U]（实际行数不在语料，U-5） | — |
| ASM-3 | 抽取代价 = 纪律负担 + 机械 churn + 双份表示 | 08-18 | 部分受挫 [I]：INC-1 全绿仍漏回归 | 08-20 后置修复 |
| ASM-4 | 独立程序集在 0.3.x 无消费者 | 08-18 | 成立至 0.4.x（届时搬 US csproj） | DEC-13 |
| ASM-5 | harness 链接编译足以强制零 Verse 纯度 | 08-18 | 成立（无违规记录） | KernelCharacterization |
| ASM-6 | 存档按枚举名序列化、namespace 不变即零 churn | 08-18 | 成立 [I]（C 面字节一致） | fixture 9 场景 |
| ASM-7 | 等权=均匀、同 roll 同结果可测 | 08-18 | 成立；分布等价≠逐次等价 | DEC-7 诚实① |
| ASM-8 | 0.x 除玩家数据与已发布面外可大胆改 | 08-18 | 08-22 收窄：作者 XML 面提前公开稳定 | DEC-3 |
| ASM-9 | 恶意 mod 无解（可直调 `SoundDef.PlayOneShot`）→诚实安全模型 | 08-18 | 成立 | DEC-4 共识 |
| ASM-10 | 第三方动作扩展需求可能不出现（放弃信号①） | 08-18 | 悬置 [U] | OQ-6 |
| ASM-11 | 0.3.0 注入面=旧选择面 → DomainPool 全排序无行为差异 | 08-18/20 | 成立（0.3.0 范围）；0.3.1 多 race 后失效边界 | 等价评审 §4.2 |
| ASM-12 | `Rand.Range` 与 `floor(Next01()*N)` 分布等价（逐次不同可接受） | 08-18/20 | 成立（玩家承诺=同池同分布） | 等价评审 §4.1 |
| ASM-13 | G 面性能可由分配论证替代实测 | 08-18/20 | 被接受但未实测闭合 [C]（「绿（论证）」） | 检查表 G |
| ASM-14 | 1.6 无 Toddler 原生 → 直映 life-stage、不按年龄阈值重算 | 08-18 | 成立 | DEC-10 |
| ASM-15 | `XmlToObjectUtils.SearchTypeHierarchy` 可填充继承字段 | 08-22 | 离线已证；真 Verse XML 加载待实机 [U] | OQ-8 |

C-1 维护者 08-20 确认路由公理（文档记载，无第二佐证）。C-2 08-23 身份门控关键场景+egg 5/11、`pawn_faction`/`pawn_ctrl` player 10/nonplayer 26 [C]。C-3 B 面 6 条 `audio.dispatch.no_sound`「未复现→按瞬态关闭」。C-4 A 面听感抽样通过。C-5 交接「15 项 13 done/2 blocked」（已被后续层取代）。C-6 H 面 No-DLC「维护者独立验证（D1）」、旧日志被覆盖。C-7 等价评审对照基准=v0.2.4 tag 零差异（agent 核验+diff 部分支撑）。
I-1 C11 缺号最可能=C10/C12 之间两个未编号提交（`5b51c6a`/`67028b8`）并号或弃号。I-2 静默回归根因=`BuildFallback` 未带内置表种子+原 41 断言未覆盖失败面。I-3 交接后来以 `5b51c6a` 提交并推送。I-4 决策文档为多时间层累写（08-18/20/21/22/23）。I-5 派发「C1–C19」后续链若存在最可能在 PKG-6——**编排方实测：C17–C19/C34–C40 在 PKG-4，C15–C16/C20–C33 语料缺席**。
U-1 彩蛋去运算符前版条文缺。U-2 热修方案 b/c 未记。U-3 0.3.0 发布执行最终结果（本包止于「仍阻塞」；PKG-4 已闭合发布事实）。U-4 0.3.1/0.3.2 计划过门记录不在本包。U-5 内核/harness 实际行数。U-6 fixture 9 场景 vs `S1-S5`+`F03-F07`（10 编号）。U-7 身份门控实机矩阵全文在 TODO.md（语料外）。

## 5. 重大失败/成功与支撑证据（INC-1..4、EV-1..16）
| id | 现象 | 根因 | 结果 | 强度 |
| --- | --- | --- | --- | --- |
| INC-1 | 换链后静音回归：`BuildFallback` 丢失内置表种子 | [I] I-2：未带种子+原 41 断言未覆盖失败面 | C12 `0b0fa48` 恢复种子；语料 3782 零 delta、双 flavor 0 warning | 实测+根因 [I] |
| INC-2 | `CollectKnownSounds` 曾失去 `_Preview` 排除 | 未明写 | 恢复排除；E 面「绿（审计）」 | 实测 |
| INC-3 | B 面 6 条 `audio.dispatch.no_sound`（每动作 1 次） | 未知；维护者未复现 | 「按瞬态关闭」→OQ-7 | [C] |
| INC-4 | 同批卫生：`KernelGate` 取 `ctx.Production`、删 `TotalWeight`、断言 41→43 | — | 已提交 C12 | 实测 |
| EV-1 | C4 `fb9ab50`：41 断言+语料 1622 零 delta | — | 语料第一层入库 | 机器 |
| EV-2 | C5 `ea5fd2c`：14 行全等价+诚实声明 | — | 0.3.1 切核基线 | 论证+单测 |
| EV-3 | C7 `7e2c3f5`：语料扩至 3782（S1-S5+F03-F07）零 delta | — | A 面主机器证据 | 机器 |
| EV-4 | A 面 08-20：dev 包 `SqueakyRatkin-dev-v0.3.0-67028b8-dirty.zip`；87 次 `audio.dispatch.ok`；`mod.start.ready` count=37；失败事件全零（BuildFallback 全程未触发；该次在修复后包上） | — | A 绿 | 实测 [C] |
| EV-5 | B 面：`trigger.outcome.summary` 13 条（峰值 dispatched=52）、0 `trigger.attempt.failed`；6 条 no_sound→INC-3 | — | B 绿 | 实测+[C] |
| EV-6 | C 面：四页/保存/flush 正常；fixture 9 场景 load→save 字节稳定；schema 未 bump（3/1） | — | C 绿 | 实测+机器 |
| EV-7 | D 面日志文件未动+双 flavor characterization；E 面行为文件零改动（仅枚举移动） | — | D 绿、E 绿（审计） | 机器+diff |
| EV-8 | F 面：`1.6/` 零文件改动；`About.xml` 仅 `<modVersion>` 0.2.4→0.3.0；stage-package SHA256 镜像通过（dev 包 116 文件） | — | F 绿 | 机器 |
| EV-9 | H 面：No-DLC 0 error（后置后 0 warning）；Biotech 门保留（F07 dormant）；HAR 缺失=硬依赖预期；Harmony 引用未动 | — | H 绿 | 实测+[C]（C-6） |
| EV-10 | G 面：分配论证（新 SelectTier 少于旧 ChoosePack）；dev 计数对比未做 | — | G 绿（论证） | 论证非实测 |
| EV-11 | BabyFits 1.6 源码一手确认；`Selector.Select(playSound:false)` 全部调用点=程序性后续选择→定案过滤 | — | 两处窄 patch 锁定 | 源码核验 |
| EV-12 | 08-23 egg=true 5 / egg=false 11 零红字；`pawn_faction`/`pawn_ctrl` player 10 / nonplayer 26 | — | 0.3.2 验证门两项过 | [C] |
| EV-13 | legacy 桥离线原型（`LegacyBridgePrototype`/`LegacyBridgeHarness`）完成；真 Verse XML 待实机 | — | 离线可行 | 机器（离线） |
| EV-14 | v0.2.4→HEAD 行为「仅枚举移动」；等价基准=v0.2.4 tag 零差异 | — | 不换面承诺证据 | diff |
| EV-15 | 测试包 `dist/dev/SqueakyRatkin-dev-v0.3.0-e6a1ed7.zip`（zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`） | — | 08-19 可分发产物 | 机器 |
| EV-16 | `origin/0.3.x` 已推送；`0.2.4-FINAL` 留档（dev@`8bd383d`）；08-20 工作树干净 | — | 提交面收口 | 文档记载 |

## 6. 矛盾与未决问题（CNF-1..8、OQ-1..11；零丢弃）
| id | 双方 | 时间层 | 点 | 解释 [I] / 回查 |
| --- | --- | --- | --- | --- |
| CNF-1 | 派发「C1–C19」vs 本包语料止于 C14 | 派发 09-17 vs 语料 08-20 | 派发超主源可见面 | 编排方实测：可见 C1–C10、C12–C14（PKG-2）+C17–C19+C34–C40（PKG-4）；**缺席 C11、C15、C16、C20–C33**。禁止写成连续 C1–C40 |
| CNF-2 | C1–C10、C12–C14 有号 vs **C11 全文无号**（`5b51c6a`/`67028b8` 未编号） | 08-20 | 编号断档 | I-1 两解均存 |
| CNF-3 | 语料 1622 例（C4/等价评审）vs 3782 例（C7/检查表） | 08-18~19 vs 08-19~20 | 同一 `corpus-0.3.0.txt` 两例数 | 非错误：C4→C7 扩容；勿合并成单数 |
| CNF-4 | 交接「GitHub prerelease → Steam」vs 08-20「**不设 GitHub prerelease tag**、本地 dev 包」 | 08-19 vs 08-20 | 双轨载体 | 交接预写未决；08-20 否定。发布事实以 PKG-4 为准 |
| CNF-5 | 动作门落地窗口：08-18「0.4.x US 拆分发布」vs 08-22「US 仓库 0.3.x 并行窗口、0.4 随 US 首版」 | 08-18 vs 08-22 | 开发窗口不同、发布时点同为 0.4 | 修订明言「原表述按新路线理解」=口径演化 |
| CNF-6 | IsEgg：原「0.3.x 内部标签、0.4.x 才开放」vs「自 0.3.2 起纳入公开作者 ABI」 | 08-20 vs 08-22 | 内部 vs 公开 | **有记录的废止** |
| CNF-7 | 交接「可能未提交/未推送」vs 检查表「`origin/0.3.x` 已推送、`5b51c6a` handoff」 | 08-19 vs 08-20 | 仓库同步相反 | I-3 时间层；并存 |
| CNF-8 | 交接「0.3.0.**x** 热修（未决）」vs 定案 `vX.Y.Z-hotfixN`（不 bump z） | 08-19 vs 08-20 | 四段号口语 vs SemVer 后缀 | 定案版为准 |

| OQ | 问题 | 类型 | 状态 |
| --- | --- | --- | --- |
| OQ-1 | merge `0.3.x→dev` + 渠道发布授权（检查表时点仍阻塞） | 需授权 | 本包未解；**PKG-4 已闭合 0.3.0 已发布**（also: PKG-4#DEC-14） |
| OQ-2 | US 仓库尚未创建（08-22 暂不建仓） | 需授权 | 语料内未建仓 |
| OQ-3 | US Workshop 显示名与许可待最终确认 | 待裁决 | 未决 |
| OQ-4 | US 在 SR 1.0.0 时点版本号待定 | 待裁决 | 未决 |
| OQ-5 | 0.3.x/0.3.1/0.3.2 与 US 发布顺序暂不决定；ABI 用「首个携带 0.3.1 XML ABI 的发布版本」浮动表述 | 待裁决 | 未决；PKG-4：0.3.1 无正式版、0.3.2 仅 `v0.3.2-pre1` |
| OQ-6 | 动作门三条放弃信号（无需求→封闭；标准键诉求→纯约定；语料非零 delta→撤销键迁移） | 常设监控 | 窗口观察 |
| OQ-7 | 6 条 no_sound 无根因即关闭 | 未验证 | 已关闭（本包质疑保留） |
| OQ-8 | legacy 桥真 Verse XML 加载待实机 | 未验证 | 离线完成、实机未做 |
| OQ-9 | 双开共存验证尚未到执行窗口 | 未验证 | 计划 |
| OQ-10 | 身份门控矩阵余项「自然覆盖」；全文在 TODO | 未验证/[C] | 存疑保留 |
| OQ-11 | C15–C19 与 C11 缺号延伸未知 | 编排 | 见 CNF-1 编排方实测；C15–C16/C20–C33 仍缺席 |

## 7. 候选教训（LES-1..6）
| id | 教训 | 强度 | 边界 |
| --- | --- | --- | --- |
| LES-1 | 「全部门槛绿」≠无回归：换链后同日仍需后置修复；失败路径必须与主链路同期配断言 | 强 | 实现替换型变更验收 |
| LES-2 | 必须把「分布等价≠逐次对拍」写成诚实声明并给出玩家承诺（同池同分布） | 强 | 以随机源为输入的重构 |
| LES-3 | 新机制先取最小面（彩蛋去运算符；动作门 0.3.x 只做两件零成本事） | 中 | 面向未来生态的机制预建 |
| LES-4 | 门槛按证据强度分级（绿/绿（审计）/绿（论证）），未做的可选实测显式写「未做」 | 中 | 门槛检查表记法 |
| LES-5 | 交接自带失效声明仍会在一天内过期——冷启动应把门槛/状态文档置于叙述性交接之前 | tentative（n=1） | handoff 体裁 |
| LES-6 | 落选方案的「被吸收硬细节」单列是高价值资产（迁移事务性/assembled-only/hook 核验/黄金语料） | 中 | 多方案比拼记录 |

## 8. 证据缺口（GAP-1..5）
| id | 问题 | 为何答不了 | 需要什么 |
| --- | --- | --- | --- |
| GAP-1 | 四份并行架构草稿全文？ | 只留比拼表 | 草稿存档（语料外） |
| GAP-2 | 0.3.1/0.3.2 是否按计划过门？ | 本包止于计划+0.3.0 门槛 | PKG-6 接续 + PKG-4 发布评审 |
| GAP-3 | C15–C19 是否存在？C11 并号还是弃号？ | 本包止于 C14 | 编排方：C17–C19/C34–C40 在 PKG-4；C15–C16/C20–C33 缺席 |
| GAP-4 | fixture 9 场景 vs `S1-S5`+`F03-F07` 如何对齐？ | 无对照表 | Scenarios.cs（代码面语料外） |
| GAP-5 | INC-1 引入提交与暴露面（哪些玩家路径会静音）？ | 只记修复不记引入点 | git log（本包禁止） |

## 9. 锚点（ANCH-1..10：逐字保留）
- **ANCH-1 前缀/协议**：`SR_*`、`US_`、`srdiag`/`usdiag`、`fmt=2`、once key `log-v2`、`coexistence=*_active`；repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker` 与 `coahuilite.squeakyratkin`、namespace `UniversalSqueaker.*`。
- **ANCH-2 内核类型**：`RaceKey`/`XenotypeKey`/`AudioDomain`/`AudioDomainStatus{Available,Dormant,TargetUnavailable,Orphan}`/`AudioDomainStatuses.Classify`；`VoicePackEntry`/`ActionSoundSet`/`DomainPool`/`SqueakPoolRegistry`/`SelectionContext{Domain,ActionKey,Age,Production,AllowEggs}`/`SelectionMode{Off,Fallback,Remix}`/`ChainResult{SoundKey,Tier,PoolStableKey,IsEgg}`/`ChainTier{XenotypePack,RacePack,PackFallback,BuiltInFallback}`/`ISoundGate`/`IRollSource`；`ModulationAxis`/`Modulation.ComposeModulation`；`FallbackProfile`/`FallbackDelta`/`CopyDisposition{KeepCopy,RebuildFromSource,MergeDelta}`/`FallbackProfileOperations{DecideCopy,Merge}`/`BuiltInFallbackCatalog`/`BuiltInFallbackTable`；`DomainFilter`（0.4.x 删）/`DomainFilter.Everything`；`ActionKey.For(SqueakAction)`。
- **ANCH-3 适配层**：`SqueakFallbackProfileStore.cs`、`SqueakLifeStageResolver.cs`、`SqueakRuntimeResolver`、`SqueakXenotypeCatalog`（类名保留）、`SqueakKernelAdapter`（唯一接缝）、`GetTargetCandidates`、`NotifyExternal`、`SqueakCompat.NotifyAction(Pawn, string actionKey)`、`SqueakActionDef`/`SqueakActionRegistry`、`SqueakGlobalActionPolicy`、`RecordOutcome`/`ExternalBlocked`、`allowExternalActions`（默认开）、`minIntervalTicks`≥60、`GetScope(string key)`、`SqueakSoundAvailabilityCache`、`SqueakTriggerInvocation`/`TriggerInvocationRules`/`SqueakActionPlan`、`PlayerSelection`/`ActiveCommand`/`IsPlayerInitiated`/`RequiresResponsivePawn`、`Pawn.IsPlayerControlled`、`!Downed && Awake()`、`Selector.Select(playSound:false)`（`__1` 位置注入）、`MentalStateHandler.TryStartMentalState`（成功 postfix）。
- **ANCH-4 事件名**：`audio.dispatch.ok`、`audio.route.selected`、`trigger.outcome.summary`、`trigger.attempt.failed`、`audio.dispatch.no_sound`、`pack.race_defaulted`、`mod.start.identity`、`mod.start.ready`、`rebuild_failed`、`refresh_failed`、`PackRejected`、`TargetRejected`、`hook.*unavailable`、`devtools.camera_indicator.changed`；v2 字段序 `sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`；人类句 `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`。
- **ANCH-5 工具/文件**：`Source/SqueakyRatkin/Kernel/`（`Domain.cs`/`Pool.cs`/`Modulation.cs`/`FallbackProfile.cs`/`DomainFilter.cs`）、`tools/KernelCharacterization/`（`Scenarios.cs` **冻结不得改动**）、`tools/SqueakLogCharacterization/`、`tools/SettingsFixtureGenerator/`、`fixtures/corpus/corpus-0.3.0.txt`、`tools/LegacyBridgePrototype/`、`tools/LegacyBridgeHarness/`、`dist/SqueakyRatkinEggTestVoices/`（gitignored）、`build-dev.ps1`、`release.yml`、`pack-github`/`pack-steam`/`stage-package`、`verify-local` 第 10 项 XML ABI 锁、`.github/skills/squeaky-voicepack-authoring/SKILL.md`、`scripts/new-voicepack.ps1`。
- **ANCH-6 ABI/协议计数**：`SqueakAction` 17 值（0–16；Crying=15、Giggling=16）；内置 fallback Ratkin 15 键（Call…MentalBreak，不列 Crying/Giggling）；srdiag v1=28 事件冻结；`settingsSchemaVersion` 3→4、`voicePackSchemaVersion` 1→2（0.3.0 不 bump：现值 3/1）；五处同步；八道闸门；四层链；诊断折叠 PackFallback→RacePack、BuiltInFallback→Vanilla；4 处 `Ratkin` 字面量临时层；UI partial ×3；整文件删除 0、符号内嵌删除约 150 行；`SqueakActionModel.cs` Count 15→17。
- **ANCH-7 证据数字**：语料 1622（C4）→3782（C7）；断言 41→43；fixture 9 场景（编号面 `S1-S5`+`F03-F07`）；14 行对照；换面 4 / 不换面 9+1 / dev 豁免 3 + 七次点击；观察窗 2 周起；350 ms；dev 包 116 文件；实机 87 派发、count=37、summary 13、dispatched 峰值 52、no_sound 6、egg 5/11、player 10/nonplayer 26；约 500+200 行；约 +1500/−600（方案 C）。
- **ANCH-8 提交链（逐字；禁止写成连续 C1–C40）**：C1 `41ca953`；C2 `efc08a0`；C3 `e48d38f`；C4 `fb9ab50`；C5 `ea5fd2c`；C6 `0893cd0`；C7 `7e2c3f5`；C8 `5c86315`；C9 `1b42a04`；C10 `e6a1ed7`；未编号 `5b51c6a` handoff、`67028b8` MEMORY/TODO；**C11 缺号**；C12 `0b0fa48`；C13 `402d4be`；C14 `2e5fc74`；留档 dev@`8bd383d`（`0.2.4-FINAL`）；dev 包串 `SqueakyRatkin-dev-v0.3.0-67028b8-dirty.zip`；zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`；热修 `vX.Y.Z-hotfixN`。C17–C19/C34–C40 见 PKG-4。
- **ANCH-9 授权**：提交/推送/发布需显式授权；US 仓库创建需另行授权；Kiiro「实验分支不发布、未经许可不宣传」；措辞不提 US 名称/多种族/Kiiro/通用化。
- **ANCH-10 外部状态**：Steam 自动更新=发布即全员无灰度；HAR 反射非依赖；No-DLC 基线；`ProductDomainFilter` 试验名单 `{Ratkin, Kiiro, Miho}`（默认 `{Ratkin}`、隐藏、替换非叠加、UI 不渲染、release 默认 off、0.4.x 整类删除）；卸载安全=存档零写入；`base.WriteSettings()` 单通道只约束 ModSettings。术语：「定型」=0.x 内部冻结；「冻结」=已发布契约。ABI 五类：①Scribe ②srdiag ③VoicePack XML ④`SqueakAction` 序数 ⑤`SR_*`/PackKey。

## 10. 来源回引
| 源文件 | 主题 | 载体 |
| --- | --- | --- |
| `docs/0.3x-refactor-architecture-decision-zh.md` | 四方案/动作门/公理/彩蛋/阶段映射/迁移/风险/纪律 | PKG-2 |
| `docs/0.3x-equivalence-review-zh.md` | 14 行+诚实边界 | PKG-2 |
| `docs/0.3x-release-gate-checklist-zh.md` | A–H、C1–C14、后置修复 | PKG-2 |
| `docs/handoff-0.3.0-zh.md` | 08-19 交接时间层、红线 | PKG-2 |

## 11. 供下游独立挑战的质疑钩子
- G1「八面全绿」：可能错在把 G「绿（论证）」与 INC-1 后置修复读成发布即无回归。回查检查表 L3/L14/L25–L27。推翻影响 DEC-8 证据质量与 LES-1。
- G2 C 链：可能错在把派发「C1–C19」当连续账本。回查检查表 L29–L31 + PKG-4 C17–C19/C34–C40。推翻影响任何「证据链完整」叙述。
- G3 0.3.0 发布执行：本包 OQ-1 写「仍阻塞」，PKG-4 已记录 0.3.0 GitHub+Workshop 页面级发布。回查 PKG-4#DEC-14/EV-11。推翻则本包「发布待授权」只是 08-20 时间层。
- G4 US 并行：建仓/显示名/许可/版本号/发布顺序全部未决（OQ-2..5）；可能错在把 08-22 重定当已执行。回查决策文档 §5 L373–L387。

## 12. 压缩损失与删减账本
| 丢弃/降级 | 原 id | 类型 | 理由 | 可回查 |
| --- | --- | --- | --- | --- |
| mermaid 图体、C# 签名块原文、措辞草案四段 | v1 §12 | P2 | 禁大段复制；标识符在 §9 | 决策文档 §3/§4.1/§5 |
| 冷启动读序 | 交接 §5 | P2 导航 | 无决策 | 交接 §5 |
| DEC-4/10/13 部分机制展开句被截断后由本填补恢复 | DEC-4/10/13 | 压表达→已补 | FILL 补全 | v1 §3 |
| **未删** 任何 DEC/ALT/CNF/OQ/GAP/ASM/ANCH/INC/EV/LES | 全部 P0 | — | 零丢弃 | — |
