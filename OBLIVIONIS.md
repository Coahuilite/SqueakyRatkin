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
