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
- **What**:`SR_Mote_TextBg` 早期 Def 缺 `ParentName="MoteBase"` + `<graphicData>` + `drawGUIOverlay` + `mote.realTime`,触发 Def ConfigErrors 红字(好友启动报错)。
- **Reason**:fixer 生成 SR_Mote.xml 时未仿原版 `Mote_Text` 结构。
- **Replaced by**:仿原版 Mote_Text(ParentName MoteBase + graphicData `Things/Mote/Transparent` + drawGUIOverlay + altitudeLayer MetaOverlays + mote.realTime),commit `80a5ab5`。
- **Status**: fixed。

## 2026-07: fixer 连续 3 次空跑(扫盘触发提权)
- **What**:派 fixer 实现 Dev 调试系统,连续 3 次返回空 task_result(fix-2/fix-3/fix-4)。
- **Reason**:fixer 内部用 grep/glob 搜原版 API 实例,扫到 `C:\Program Files`/Steam 目录,触发系统提权弹窗被用户 reject,任务空跑。
- **Replaced by**:fixer prompt 明确"禁止扫描项目根外路径"+ 所需 API 由 orchestrator 提前核实写进 prompt。fix-5 起成功。
- **Status**: resolved。**教训**:派 fixer/agent 时禁扫外部,API 由调用方提供。
