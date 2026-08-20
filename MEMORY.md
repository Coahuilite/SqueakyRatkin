# MEMORY

## 当前耐久状态
- 项目为 RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；永久 `packageId` 为 `coahuilite.squeakyratkin`，产品版本 `0.2.4`。
- `0.2.4`（最新）：GitHub 已发布核验（tag `v0.2.4`、Release CI `32044477840` success、资产 115 文件逐项核验）；Workshop 已上传且页面级核验完成（同一 item 3758115669、Updated 17 Aug 09:22、中英文案同步草稿）。Claim Pack：[`docs/release_review/release-0.2.4-review-zh.md`](./docs/release_review/release-0.2.4-review-zh.md)（含三项流程自我批评）。
- `0.2.3`：GitHub 已发布核验；Workshop 未上传。`0.2.1`/`0.2.2`：已发布，细节见各自 Claim Pack（`docs/release_review/`）。
- **包版本标识**：0.2.5 起三渠道（dev/steam/github）包内 `version.txt`（`SqueakyRatkin <版本>`/`build=<flavor>`/`commit=<sha>`，dev 带 `-dirty`），由 `stage-package.ps1` 统一写入；已发布的 0.2.4 各渠道包不含，重发与否由维护者定。
- 流程综合复盘（跨 0.2.x，含新增四项与通用原则）：[`docs/release_review/process-review-zh.md`](./docs/release_review/process-review-zh.md)。

## 权威入口
- 仓库导航：根 [`codemap.md`](./codemap.md)（纯汇总），具体目录读其 `codemap.md`；地图可能过期，源码与 `docs/` 现行合同权威。
- 架构/运行时/动作/发布边界：[`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md)；设置 UI：[`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md)；日志协议：[`docs/logging-protocol.md`](./docs/logging-protocol.md)。
- **0.3.x 实现架构（已接受，实施唯一入口）**：[`docs/0.3x-refactor-architecture-decision-zh.md`](./docs/0.3x-refactor-architecture-decision-zh.md)；规划输入（不覆盖合同）：[`docs/internal-universalization-design-note-zh.md`](./docs/internal-universalization-design-note-zh.md)。实现前将受影响结论提升进合同。
- VoicePack 作者约定：[`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md)；Workshop 页面维护源：[`docs/steam-workshop-page-copy-draft.md`](./docs/steam-workshop-page-copy-draft.md)（含版本一致性/只读核验维护规则）；发布流程唯一入口：[`docs/release-runbook-zh.md`](./docs/release-runbook-zh.md)。

## 工程决定与交接
- `0.2.3`（已实现发布；分类=**优化/行为变更**）：默认音源策略改为 Fallback + 内置 Race Example 种子（纯 Vanilla 回退豚鼠 clip 占比高，婴幼儿动作集全部命中——"婴幼儿路由豚鼠叫"根因是默认态落回退层，非年龄路由）。迁移规则：`voicePackMode` 节点缺失（Scribe 省略默认值节点）→ Fallback + 启动链 `EnsureBuiltInRaceDefault()` 种子一次（`voicePackDefaultSeeded` 幂等）；显式模式与已有选择永不被覆盖。schema 2→3。
- `0.2.4`（已实现发布）：精神崩溃音 hook 收窄至 `MentalBreakWorker.TryStart`（MentalBreakDef 驱动唯一通道）；Biotech BabyFits（Giggling/Crying）属正常精神状态发作，不派发崩溃音。日志协议 append-only 扩展（`pawn=`/`pawn_id=` 尾随字段），由 `SqueakLogCharacterization` 锁定；后续 0.3.x 触发基线须锁定此 hook 语义，Crying/Giggling 动作兼容为 0.3.x 年龄域规划。
- `0.3.x` 架构决策（2026-08-18，分支 `0.3.x`；实施唯一入口 [`docs/0.3x-refactor-architecture-decision-zh.md`](./docs/0.3x-refactor-architecture-decision-zh.md)）：① 重构采纳「内核但薄」——决策逻辑（域键/池/链/fallback/年龄/Filter）进零 Verse `Kernel/` 编译集 + `tools/KernelCharacterization`（纯度门+单测+黄金语料），不拆程序集；吸收迁移事务性、`GetTargetCandidates` assembled-only 投影、BabyFits hook（`TryStartMentalState` 成功 postfix + `MentalFitDef` 反向 map）、黄金语料升级。② 动作门三方向比拼后采纳**受控开放**——`SqueakActionDef` 注册 + `SqueakCompat` 门面 + 全闸门复用 = 八道闸门链（含玩家总闸 `allowExternalActions` 默认开），0.4.x US 拆分与消费者同窗口落地；0.3.x 仅两件零成本前置：内核动作键出生即字符串（`ActionKey`，内置=枚举名，枚举留适配层设置/存档面）、srdiag v2 `action` 字段定型字符串键（0.3.1）。③ 0.3.2 定型 17 动作 ABI（Crying/Giggling append 15/16，内置表不列条目）与 Fallback 末端→profile→无声；内置表封闭于内置键；放弃信号（无第三方需求→退回封闭）。**0.3.0 等价验收第一原则：已发布功能=玩家必用，设置全项验证（唯一豁免=dev 隐藏功能）；发布门槛八面（A–H）+ 双轨发布 + 0.3.0.x 热修策略（决策文档 §5，三项待确认）**。
- **0.3.0 实施状态（2026-08-19/20）**：换链完成——resolver 经 `Kernel/SqueakPoolRegistry.Select`（适配层 `SqueakKernelAdapter` 注入 `(Ratkin,*)`），旧 `ChoosePack`/vanilla 字典/`Or`/`ResolvedAudioPack` 同变更删除；证据链 C1-C11（fixture 9 场景、枚举提取+ActionKey、Kernel 骨架、KernelCharacterization 41 断言+3782 例语料回放零 delta、等价评审、resolver 接入、fixture 驱动语料、检查表、版本 bump 0.3.0、zip 脚本化、handoff）；**发布门槛八面全绿**（实机：A 池归属/B 时机计数/C UI 确认/D 日志 v1/H No-DLC 基线；离线 E/F/G + C fixture）；八面证据已落检查表（2026-08-20 维护者二次实测 + Player.log 87 派发核验；no_sound 6 条未复现按瞬态关闭）；**发布流程暂缓**（三项决策待定）；**merge 0.3.x → dev 择日**（预期 fast-forward；merge 前跑三 harness——2026-08-20 已复跑全绿）；本会话修复/卫生/Steam 打包纪律/决策文档已提交（C12–C14）并推送 `origin/0.3.x`；交接见 `docs/handoff-0.3.0-zh.md`。
- **0.3.0 错误路径修复 + 卫生梳理（2026-08-20，已提交 C12）**：`BuildFallback` 恢复种子内置表（重建失败仍放 SR_* 兜底）；`CollectKnownSounds` 恢复 `_Preview` 排除；KernelGate 统一取 `ctx.Production`；删 `TotalWeight` 死计算；KernelCharacterization 41→43 断言（失败路径 2 条）；主模组双 flavor 0 warning/0 error；三 harness + 语料 3782 回放复验全绿。
- **0.3.1 = 半分离→全分离收口（2026-08-20 维护者确认）**：内核自足判据 = 删除 SR 兼容层后内核编译集直接迁移可用；收口清单见 TODO 0.3.1（FallbackProfile 字符串键、内核内置键清单、Select 内核自有 mode、ActionKey 入核、SqueakActionModel 出 harness、迁移清单 = Kernel/ + 0 根文件、Ratkin 字面量拆除清单）。
- **路由核心公理 + 机械验证约束（2026-08-20 维护者确认，决策文档 §2.3）**：路由=链唯一事实源；执行层单向依赖路由、只经输入/`ISoundGate` 注入面影响候选；新决策逻辑出生即纯、场景配通路或声明 out-of-scope、五处同步机械断言。分阶段：同步断言 0.3.1、漏斗纯逻辑提取 0.3.2、动作门纯决策 0.4.x。
- **彩蛋语音裁决（2026-08-20 定案，同日 YAGNI 修订，决策文档 §2.4）**：仅 `IsEgg` 标签、无运算符（种族定位由 pack `raceDefName` 承担）；加性池成员、开关默认关、0.3.2 XML ABI 同批落地、作者指南 0.4.x 同窗口；Kiiro 许可门不变。
- **归档分支 `0.2.4-FINAL` = dev@`8bd383d`（csproj 0.2.4）**：2026-08-20 建立，仅本地，未推送；dev 线 0.2.4 状态的只读指针。
- **发布流程边界**：runbook 为唯一入口；Steam 页面核验仅**玩家视角只读** filedetails（绝不尝试登录态/编辑界面，编辑=维护者人工）；workshop 草稿变更后必须核对版本号/下载链接/字符数/检查清单；包内容核验含 `version.txt`。维护者发布不使用 SteamCMD；Steam 页面核验 = **本地纪律不进 CI**（公开 API 只读轮询，2026-08-20 确认）。**Steam 打包纪律（2026-08-20 定案）**：Steam = 最终发布步 → `pack-steam` 干净树硬门（脏树 throw）+ `build-steam.ps1` 一步入口（Steam flavor 由构造保证）；三渠道 `stage-package` 断言 About.xml == csproj 版本。0.2.1 教训（LoadFolders 静默回归、codemap 泄漏、tag 重发 changelog、`merge -s ours`）已并入 runbook 与 process-review。
- Kiiro：实验分支不发布不 merge；通用化支持（标准 HAR `defName=Kiiro_Race`），无永久 adapter；Workshop 页面存在衍生作品限制，未经许可不得发布/宣传 Kiiro compat。
- `srdiag v1` characterization 护栏：`tools/SqueakLogCharacterization`（纯 net472 harness，28 事件）；扩展日志协议必须同步 characterization。
- **发布流程边界**：runbook 为唯一入口；Steam 页面核验仅**玩家视角只读** filedetails（绝不尝试登录态/编辑界面，编辑=维护者人工）；workshop 草稿变更后必须核对版本号/下载链接/字符数/检查清单；包内容核验含 `version.txt`。0.2.1 教训（LoadFolders 静默回归、codemap 泄漏、tag 重发 changelog、`merge -s ours`）已并入 runbook 与 process-review。
- 当前目标、开放行动、阻塞与明确延后项见 `TODO.md`。冷证据仅供历史冲突或明确请求，不能覆盖现行合同。
