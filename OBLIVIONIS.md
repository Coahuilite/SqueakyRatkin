# OBLIVIONIS

冷归档：不要在会话开始时读取；仅在历史冲突、重新启用旧议题或明确请求时读取。

---

## 2026-07-28：治理层收敛前的文档草案（归档）
- 旧 planning、engineering review、repository status、text review 与 settings responsive draft 已改为 `DEPRECATED_` 前缀。
- 它们由项目架构合同、设置 UI 产品合同、日志协议与 VoicePack 作者指南取代；只保留为冷证据，不重复其全文，也不能作为现行权威。
- **状态**：superseded historical documentation。

---

## 2026-07-27：0.2.0 接受 campaign（归档）
- **内容**：响应式设置 UI/layout、立即运行时设置与合并保存、Periodic 启动相位/可听人口缩放、诊断、No-DLC 与 VoicePack 接受工作，以用户接受结束。
- **取代**：旧的 pending、static-only、review-round 与 Gate A–E 标记不再是当前阻塞项。
- **边界**：用户接受不证明每一个历史测试矩阵行均已单独运行；它也不等同版本更新、推送、合并、tag 或发布。
- **状态**：accepted；当前交接见 `MEMORY.md` / `TODO.md`。

## 0.1.0 / 0.1.1 发布谱系（归并归档）
- `v0.1.0-rc1`、`v0.1.0` 与修复版 `v0.1.1` 是已完成的历史发布链；`v0.1.1` tag/main commit 为 `1b1fe9e`。
- 旧 rc 调参、Steam 包、dirty artifact、局部发布卫生与当时的待推送描述不再指导当前工作。
- **状态**：historical-at-the-time；不要称 0.1.1 为当前开发状态，也不要把 0.2.0 描述为“当前 live validation 基线”。

## 2026-07-28：发布卫生前的本地状态（归档）
- `dev`/远端 ahead 数量、dirty Dev artifact 标签、旧独立 VoicePack staging 与“保留 VoicePacks 待迁移核对”的待办均为当时状态。
- 旧 `VoicePacks/` 已删除；现行发布基建与待办以 `MEMORY.md`、`TODO.md`、`AGENTS.md` 为准。
- **状态**：superseded historical state；不可作为当前 Git、artifact 或目录结论。

## 2026-07：64 SoundDef 矩阵 → 运行时调制
- 初版 8 动作 × 4 心情 × 2 套矩阵造成数据膨胀。
- 现由中性 SoundDef 加 `SoundInfo.pitchFactor` / `volumeFactor` 在运行时调制替代。
- **状态**：superseded，禁止回退到心情×动作矩阵。

## 2026-07：Move/Sleep Sustainer → 一次性 RandomOneShot
- 持续音方案不符合 QOL 的偶发反馈定位。
- 已由 XML `RandomOneShot` 替代；sustainer 逻辑已移除。
- **状态**：superseded。

## 2026-07：旧 VoicePack 资格模型 → unified VoicePack
- PoolDef、Official Default 特权、HAR whitelist/source 资格门曾使选择与回退边界不一致。
- 已由单一 `SqueakVoicePackDef`、精确大小写 `XenotypeDef.defName` 与公平 Off/Fallback/Remix 策略替代；HAR/source 只可作 UI hint/诊断。
- **状态**：superseded，禁止恢复资格过滤门。

## 2026-07：旧实现与测试事故（压缩归档）
- `SR_Mote_TextBg` 曾缺少原版 Mote 所需结构，已修复；早期 `Attack PatchAll` 曾包含无 body 方法，已改为有界筛选。
- `Silence.wav`、`MANUAL_STEPS.txt` 与旧 OneShot 测试包已删除；历史运行结果不可作为当前包验收。
- 旧 settings UI 布局、GUIClip/ScrollView incident、review session、artifact hashes（含 CADAB）均仅为历史当时证据，不能称为 current。

## 2026-07：dev orphan 历史修复
- dev 曾因 orphan 重建与 main 无共同祖先，后以合并建立共同祖先。
- **教训**：重构分支应从 main 建立；当前仍遵循 `dev` → protected-main PR/squash → tag main → CI release。

## 2026-07-27：ExamplePack 外部独立分发/拆仓方案 → 主包内置休眠模板
- **旧方案**：把 `SqueakyRatkinExampleVoices` 提取为外部独立项目/仓库并另行分发。
- **放弃原因**：新目标是让每位玩家随主模组取得可复制、修改的示例，同时保持其独立 package 身份和非自动加载边界。
- **中间方案**：曾建议主包 `Extras/` 携带休眠 ZIP；随后用户明确要求内嵌内容由主模组直接加载并通过创意工坊分发，因此 ZIP 方案也被放弃。
- **当时替代方案**：`Extras/` 携带完整独立目录；父 `LoadFolders.xml` 有条件加载其 Race 内容，canonical 顶层包启用时跳过内嵌版。该中间方案后来再次被淘汰。
- **状态**：superseded；当前决策见 `MEMORY.md`。

## 2026-07-27：canonical override / Patches + Sounds 方案（归档）
- **旧方案**：主模拥有唯一 Example PackDef/SoundDef；`Extras` 顶层启用后只用 15 个 Patch 替换主模 SoundDef grains，以保持同一 PackKey，并把 Extras 称为 canonical override add-on。
- **放弃原因**：它曲解了既定产品边界。`Extras` 从始至终是供创作者使用、拥有自身 Def/PackKey/Catalog 行且可直接启用的完整独立 Template VoicePack；官方 Example 则有且只有主模内置的一份。两者不应通过 Patch 绑定身份。
- **连带废止**：override XPath、部分 Patch 混音、多个 override 顺序、外部音频仍显示主模 Def owner、禁用 override 后恢复 grains 等问题不再属于当前设计。
- **状态**：superseded；禁止恢复。外部 VoicePack 自行 Patch 其他 VoicePack 不属于本模组防御范围。

## 2026-07-27：Extras 本地音频懒加载适配器（归档）
- **旧方案**：主模从嵌套 Extras 按需读取 WAV/OGG，并自行维护异步 AudioClip 生命周期与 fallback。
- **放弃原因**：需要第二套音频后端并穿透 Catalog/resolver/preview/diagnostics，复杂度与兼容风险高于原生 Def + staging 镜像；且会破坏内置官方 Example 与独立 Template 的清晰边界。
- **状态**：superseded；除非未来明确改变产品目标，不重新评估。

---

## 2026-09-19：0.2.x–0.3.x 实施与流程记忆压缩（归档）

- **0.2.1–0.2.4**：悬浮诊断重构（单字符标记 + 可拖动面板）、七击计数修复、`LoadFolders` 去硬门控；`SqueakLog` facade/protocol 拆分 + srdiag v1 28 事件 characterization；`About <modVersion>` 落地；0.2.3 默认音源策略改 Fallback + 内置 Example 种子（schema 2→3）；0.2.4 精神崩溃 hook 收窄到 `MentalBreakWorker.TryStart`、日志 append `pawn`/`pawn_id`。
- **0.3.0（2026-08-21 发布）**：零 Verse `Kernel/` 编译集（域键/池/链/fallback/调制）+ `tools/KernelCharacterization`（纯度门、43 断言、3782 例语料）+ resolver 接入；发布门槛 A–H 八面全绿；双轨发布与热修方案 a（`vX.Y.Z-hotfixN`）定案；打包纪律 = stage 断言 About==csproj、包内 `version.txt`。
- **0.3.1（工作并入 0.3.2 发布）**：race-aware（`raceDefName` 必填、域闸、事务性 Scribe 4/2）、年龄（ageTag exact→all-age、直映 `CurLifeStage.developmentalStage`、1.6 无 Toddler）、fallback 末端（`BuiltInFallbackCatalog` 15 键 + `SqueakFallbackProfileStore` Config 副本单写者）、彩蛋 `IsEgg`（默认关、加性池成员）、Crying/Giggling append 15/16 + `TryStartMentalState` 窄 hook；漏斗纯文件提取（`SqueakActionPlan`/`SqueakTimingModel`）+ 双语料回放零 delta。
- **0.3.2（GitHub prerelease `v0.3.2-pre1`）**：玩家触发身份门控（`IsPlayerControlled`、`PlayerSelection` 另需 `!Downed && Awake()`、`playSound:false` 过滤）、XML ABI 固化（作者面冻结、`IsEgg` 入 ABI）、发配日志重排（只发一条 v2 `audio.route.selected`，含 `pawn_faction`/`pawn_ctrl`）、作者指南统一为 SKILL 正本 + 脚手架。
- **Eat 两级开关（2026-08-23）**：Steam 评论正反馈驱动的实现；默认 job 级派发，父「仅真正进食」= `GainingNutritionNow`，子「使用成瘾品」= `ChewIngestible` toil（未确认时回落完整 job）；纯规则 `SqueakEatOccurrence` + 单测。规格与 vanilla 基线见当时的交接文档（已随 archive 删除）。
- **流程简化 V1–V3（2026-09-13）**：V1 本地 commit 免授权；V2 隐私历史重写只出方案待授权；V3 CI 加门 + SDK 10.0.x；三命令契约（verify-local / check-pack-readiness / privacy-audit）与最小发布仪式取代人工清单。
- **文档收敛与清理（2026-09-19）**：外部 agent 终审并重建 10 份现行 docs（合同 3 + runbook + 维护状态 + Steam 草稿 + 隐私方案 + 双语 CHANGELOG + 索引），旧文档与收敛管道先入 archive 后被整体删除；14 份 `codemap.md` 全部删除；证据（7 份 Claim Pack + 2 份流程复盘 + 终审报告）保留在 `docs/release_review/`。删除内容可从提交 `8481cbe` 取回。
- **分支模型（2026-09-19 裁定）**：`dev` 分支取消，每个 minor 在自己的分支开发（如 `0.3.x`），发布时提交并 merge 到 `main`；`archive/` 分支只在最终版本建立。
- **RimWorld 1.6 年龄体系事实基线**：`lifeStageAges` → `CurLifeStage` → `LifeStageDef.developmentalStage`（Baby/Child/Adult，无 Toddler）；Human 五段 0-3/3-9/9-13/13-18/18+；voxPitch 1.6/1.2/1.0。SR 一律直映，不自算年龄阈值。
- **Kiiro 实验线**：`kiiro-experiment` 分支（薄装配 adapter + `SQUEAKY_EXPERIMENTAL` 门 + MeowingKiiro-EXP 试验包）已于 2026-09-13 之后删除（本地 + 远端），不发布、不 merge。
- **发布历史台账**：0.1.0–0.2.4、0.3.0、0.3.2-pre1 的 tag/CI/资产核验记录见各自 Claim Pack；0.2.3 Workshop 未上传、0.2.2 页面观察缺口、0.2.4 三项流程自评等旧口径不再指导当前工作。
- **`.slim` 隐私债务**：历史提交/tag 仍可达 `.slim/codemap.json`（含本机路径），HEAD 已净化；维护 A（不重写 + 台账），重写需单独授权。
- **状态**：completed / superseded；现行权威见 `MEMORY.md`、`TODO.md` 与 `docs/` 合同。

---

## 2026-09-19：0.3.3 撤回重发、R6 修正与实机验收（归档）

- **R6 缺陷**：0.3.0 起的零 Verse 内核把三层 Remix 折叠写成"按原始位序取 index"，缺层时命中空位（该次事件静默）且后继层（内置音源）永不可达；0.2.4 的等价实现是"仅非 None 层进入候选表"，属 0.3.0 重写引入的回归。默认 Fallback 不受影响，但出厂播种的 `SR_OfficialExample_Race` 声明空 fallbacks，任何开启 Remix 的默认装配都命中。内核复现：shape A `none=105/200`、内置 `0/200`；shape B `none=115/200`、内置 `0/200`。
- **撤回与重发**：首发 tag `v0.3.3`（→ `c7fb868`，资产 1,590,750 B / SHA256 `b7eb7dac…`，downloadCount 1）发布当日撤回（Release + tag 删除，Latest 一度回退 v0.3.0）；同日同版本重发 tag `v0.3.3` → `cd90a9e`，资产 1,590,744 B / `fced520a…`（下载级核验 = API digest），Release 页加更正说明。
- **修正与守卫**：`SelectRemixThree` 改为只在非 None 层等权折叠（与四层同规则；分布等价 0.2.4，非 RNG 流一致）。守卫 = shape A/B 断言 + `corpus-0.3.1.txt` 重建 + 新增 `corpus-0.3.0-r6.txt` 字节回放 + 历史 `corpus-0.3.0.txt` 540 行差异断言。独立只读复核五项全 CONFIRMED。
- **实机验收（流程特例）**：以 GitHub release 资产替代发布前 dev 包前置测试（维护者裁定，仅本次）；日志身份 `build=github` / `v0.3.3+cd90a9e55d44` 自证对象为发布资产；SR 相关 error/no_sound 全 0、26 次派发、同一动作同时出现包层与内置层；维护者听感确认无问题。
- **1.0 形态裁定**：对 US 硬依赖（FL 传递）；Def 归属按 US 约定；设置不自动导入、只提醒玩家自留；Steam 3A 声明下方加无具体时间的退役公告，但 US 未上工坊前不随 0.3.3 上传。
- **Steam 上传（2026-09-19 完成）**：维护者人工更新同一 Workshop item；agent 只读核验公开页面——`Mod version: 0.3.3`、英文描述块与文案源一致、Change Notes 为仓库 CHANGELOG 0.3.3 英文段（约 1,993 字符）、退役公告未出现（符合裁定）。页面**未含中文描述块与 R6 条目**；Workshop 二进制与订阅后实机未核验（页面级证据）。线上 0.3.0（含 R6 缺陷）已替换。
- **状态**：completed；完整证据见 `docs/release_review/release-0.3.3-review-zh.md`，现行开放项见 `TODO.md` 与维护状态。
