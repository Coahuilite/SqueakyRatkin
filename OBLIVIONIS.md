# OBLIVIONIS

Cold archive for downgraded memory. Do NOT read at session start. Read only for historical conflict, resurrected topic, or explicit request.

---

## 2026-07: 64 SoundDef 矩阵 → 运行时调制(16)
- **What**:初版为 8 动作 × 4 心情 × 2 套 = 64 个 SoundDef,每个心情档独立 pitchRange/volumeRange。
- **Reason deprecated**:把"运行时该做的调制"前移成编译期数据膨胀;`SoundInfo` 支持 `pitchFactor`/`volumeFactor` 运行时调制。
- **Replaced by**:每动作 1 个 SoundDef(中性 pitchRange)+ Comp 播放时按心情设 pitchFactor/volumeFactor。SoundDef 64→16。
- **Status**: superseded(过度设计纠正)。

## 2026-07: Move/Sleep 持续音 sustainer → 一次性 RandomOneShot
- **What**:初版 Move/Sleep 用 Sustainer(`Maintenance.PerTick` + 每 tick Maintain + 动作结束 End)。
- **Reason**:用户决定改一次性偶发,简化(QOL 定位,保持简单)。
- **Replaced by**:`actions` XML `mode=RandomOneShot`,砍掉整个 sustainer 逻辑。
- **Status**: superseded。

## 2026-07: Silence.wav 早期占位 → 删除
- **What**:`1.6/Sounds/Squeak/Silence.wav` 早期静音占位音频。
- **Reason**:SoundDef 改用豚鼠 default 后 Silence 不被引用;且会随 pack 进玩家分发包(玩家下到无用 wav)。
- **Status**: deleted。

## 2026-07: MANUAL_STEPS.txt 内部 todo → 删除
- **What**:根目录 `MANUAL_STEPS.txt` 作者发布前 todo 清单。
- **Reason**:内部 todo 不该入公开仓库;内容已由 README/CONTRIBUTING 覆盖。
- **Status**: deleted。

## 2026-07: dev orphan 重建致与 main 无共同祖先 → merge 修复
- **What**:dev 原子链用 `git checkout --orphan` 重建(无 parent),与 main 的 `afa00cb` 无共同祖先,GitHub PR 报 "entirely different commit histories" 无法比较。
- **Reason**:重构 dev 时未基于 main。
- **Replaced by**:`git merge main --allow-unrelated-histories -X ours` 建立共同祖先 afa00cb;merge 带回的废弃文件(MANUAL_STeps/Silence/Squeak.gitkeep)重新 git rm。
- **Status**: resolved。**教训**:重构分支应基于 main(`git rebase --root --onto main` 或重建时 parent=main),或事后 merge 建立祖先。

## 2026-07: SR_Mote 缺 ParentName/graphicData → 红字 → 已修复
- **What**:`SR_Mote_TextBg` 早期 Def 缺 `ParentName="MoteBase"` + `<graphicData>` + `drawGUIOverlay` + `mote.realTime`,触发 Def ConfigErrors 红字(首次启动报错)。
- **Reason**:fixer 生成 SR_Mote.xml 时未仿原版 `Mote_Text` 结构。
- **Replaced by**:仿原版 Mote_Text(ParentName MoteBase + graphicData `Things/Mote/Transparent` + drawGUIOverlay + altitudeLayer MetaOverlays + mote.realTime),commit `80a5ab5`。
- **Status**: fixed。

## 2026-07: fixer 连续 3 次空跑(扫盘触发提权)
- **What**:派 fixer 实现 Dev 调试系统,连续 3 次返回空 task_result(fix-2/fix-3/fix-4)。
- **Reason**:fixer 内部用 grep/glob 搜原版 API 实例,扫到 `C:\Program Files`/Steam 目录,触发系统提权弹窗被用户 reject,任务空跑。
- **Replaced by**:fixer prompt 明确"禁止扫描项目根外路径"+ 所需 API 由 orchestrator 提前核实写进 prompt。fix-5 起成功。
- **Status**: resolved。**教训**:派 fixer/agent 时禁扫外部,API 由调用方提供。

## 2026-07: v0.1.0-rc1 发布过程细节 → 0.1.1 后冷归档
- **What**:`v0.1.0-rc1` 作为内测候选发布,包含 Death 音效、工作台 editBuffer、mood/action 数据驱动、试听迁移、SC 本地化、启动日志增强与一组早期调参。
- **Reason downgraded**:正式 `v0.1.0` 与修复版 `v0.1.1` 已发布,rc1 过程细节不再指导当前开发;保留在热记忆中会干扰版本判断。
- **Current replacement**:`MEMORY.md` 只保留当前版本 `0.1.1`、release flow、架构红线与玩家排障 DebugAction 约束。
- **Status**: archived。

## 2026-07: rc1 高倍速音量反馈细节 → 已由 0.1.0/0.1.1 吸收
- **What**:rc1 反馈中确认高倍速声音显著变小,主因是旧代码按 `TickRateMultiplier` 降低单次 `volumeFactor`;暂停缩放后第一声音量不一致曾列为待复现。
- **Reason downgraded**:当前架构已移除高倍速单次音量压低,改由冷却补偿控制触发密度;距离衰减改用 vanilla spatial `distRange` 与玩家可调 `Distance volume fade`。
- **Current replacement**:`MEMORY.md` 中的 distRange/Distance volume fade、cooldown scaling 与 camera indicator 约束。
- **Status**: superseded。

## 2026-07: 功能调优/默认频率“本地待推送”状态 → 已发布
- **What**:曾记录声音浏览器 InMap、删 zoom gating、Select 15 ticks、mote offset、Death 音效、默认频率下调等为“本地待推送”。
- **Reason downgraded**:这些内容已进入正式发布链,不再是当前 pending 状态。
- **Current replacement**:`MEMORY.md` 的核心 CompSqueaker/触发模式/ModSettings/Death/Distance volume fade 事实。
- **Status**: completed and archived。
