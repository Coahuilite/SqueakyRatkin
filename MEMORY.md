# MEMORY

## Current Project State
- RimWorld 1.6 mod「鼠辈啁啾 / Squeaky Ratkin」,packageId `coahuilite.squeakyratkin`(**发布后定死,不改**——改了旧存档失联)。
- 仓库 https://github.com/Coahuilite/SqueakyRatkin。分支模型:`main`(受保护,PR squash 合并,稳定发版)+ `dev`(原子提交开发,所有 PR 提到 dev)。
- junction:`I:\SteamLibrary\steamapps\common\RimWorld\Mods\SqueakyRatkin` → 工作区根,编译即加载。
- 构建:`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj` → `1.6/Assemblies/SqueakyRatkin.dll`,要求 0 错误。
- BuildFlavor:`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` → `SQUEAKY_*` 常量,仅影响启动日志 banner,运行时功能三态相同。
- 版本:0.1.0(SemVer,内测期 0.x.y,正式 1.0.0)。源头 csproj `<Version>`/`<AssemblyVersion>`。
- CI:`.github/workflows/ci.yml`(push/PR main+dev 构建验证;push 额外产出 `dev-<sha>` artifact 供内测)+ `release.yml`(tag `v*` 发 GitHub Release)。
- git identity:`19252128+Coahuilite@users.noreply.github.com`(GitHub noreply 带数字 ID,`--local` 不碰全局)。
- 许可:代码 MPL-2.0,音频 All Rights Reserved,原版资产只引用不分发。
- 本地化:全 Keyed(EN + SC),`SqueakLabels` helper 封装动作/心情名翻译。

## Completed (立项→上云全流程)
- **立项评估**:核实原版机制——类人种族**无原版发声钩子**(`Pawn_CallTracker` 只为 `intelligence<=1` 动物创建,`PawnComponentsUtility.cs:183`;`Toils_Ingest` 显式跳过 Humanlike 的 `race.soundEating`)。故"纯 XML 补 race 声音字段"对鼠族无效,必须自驱动 C#。
- **核心 CompSqueaker**:数据驱动(`actions` + `moodMods` 在 `1.6/Patches/Ratkin_AddSqueakComp.xml`,C# 通用适配);**运行时调制**(`SoundInfo.pitchFactor/volumeFactor`,非 SoundDef 矩阵);三层配置(CompProperties 默认 ← ModSettings.moodOverrides ← useCustomOnly);复用 `Pawn_CallTracker` 摄像机视野剔除(`CurrentViewRect.ExpandedBy(10).Contains()`,性能优化)+ **distRange 距离衰减**(`SoundInfo.InMap(TargetInfo(Pawn))` 让引擎按相机-pawn 距离线性衰减,15~70 格,超 70 无声;2026-07 删除原 `CurrentZoom<=Close` zoom gating,因它挡掉 distRange 自然衰减,只有最大缩放才响)+ `TickRateMultiplier` 时间倍速音量;default 原版豚鼠(GuineaPig)。
- **触发模式**:Eat=EachTime;Call/Move/Sleep/Social/Joy=RandomOneShot(概率+间隔);Wounded/Select/Death=External(patch `Pawn.PostApplyDamage` / `Selector.Select`(postfix,位置注入 `__0`)/ `Pawn.Kill`(prefix,被击杀+流血而亡统一入口),带最短间隔节流防高攻速刷屏)。Select 节流 15 ticks(0.25 秒,2026-07 从 60 调低)。
- **ModSettings 调制工作台**:下拉选心情/动作 + slider+TextFieldFloat 联动 + 4 预设(尖锐/中性/低沉/混乱)+ 预览试听(`SoundDef.PlayOneShot(SoundInfo.OnCamera())`)。
- **Dev 调试**:`[DebugAction]` 菜单(悬浮字开关 + 音频浏览器)+ 描边悬浮字(`MoteTextWithBackground`)+ DevMode 日志。
- **分发**:LICENSE(MPL-2.0)+ 双语 `README.md`/`README.zh-CN.md` + AGENTS.md + CONTRIBUTING.md(含术语通俗解释)+ `1.6/Sounds/Squeak/AUDIO_GUIDE.txt`(ogg/22050/16bit/mono);`scripts/`(validate-junction + pack-steam/pack-github);CI/CD。
- **上云准备**:脱敏(README 本地路径占位、AGENTS 兄弟项目名泛化)+ `.gitignore` 完善 + packageId 定死 + author 统一 Coahuilite + commit 规范(`.gitmessage`)。
- **分支保护**:main `require PR` + `enforce_admins` + 禁 force push;dev 原子提交链(9 阶段:骨架→核心→patches→工作台→调试→本地化→分发→文档→音频)。
- **红字修复**(2026-07):`SR_Mote_TextBg` 缺 `ParentName="MoteBase"` + graphicData + drawGUIOverlay + mote.realTime → Def ConfigErrors 红字;仿原版 `Mote_Text` 修复(commit `80a5ab5`)。
- **启动崩溃修复**(2026-07,commit `982331f`,实测通过):(1) `Patch_Selector_Select` postfix 声明 `Thing t`,但 `Selector.Select` 实参为 `(object obj,bool,bool)`,Harmony 按参数名注入找不到 `t` → Mod 类构造崩;改位置注入 `object __0`。(2) XML FloatRange 写 `(a,b)`,但 vanilla `FloatRange.FromString` 用 `~` 分隔(`Split('~')`)→ 16 SoundDef + moodMod 全部 FormatException;全项目 `(a,b)`→`a~b`。
- **功能调优**(2026-07,本地待推送):声音浏览器 `SoundInfo.OnCamera()`→`InMap(镜头中心)`(onCamera 需 subSound.onCamera=true 未配);删镜头 zoom gating;Select 节流 60→15;mote 位置可配置(`modExtensions/SqueakMoteOffset`,offsetY 默认 1.2);Death 死亡音效(`Pawn.Kill` prefix + `SR_Death`,GuineaPig/Pain grain,pitch 0.85~0.95)。

## Active Constraints
- **Def 前缀 `SR_`**(Def 数据库全局防撞);C# 类不加前缀(namespace 隔离)。
- **packageId `coahuilite.squeakyratkin` 定死**(改了旧存档找不到本 mod)。
- **main 受保护**:只能 PR squash 合并,禁直接 push/force push;所有开发进 dev。
- **原版资产只引用 defName/clipFolderPath,不分发原版文件**(Ludeon 政策)。
- **用户可见文本走 Keyed**(EN + SC),无硬编码。
- **agent 禁扫项目根外路径**(grep/glob/read 触及 `C:\Program Files`/Steam/兄弟目录 → 触发提权失败,连续 3 次空跑才定位;所需 API 由调用方提供)。
- **commit 规范**:Conventional Commits(`<type>: <desc>`,feat/fix/docs/chore/refactor),原子提交进 dev,模板见 `.gitmessage`。
- **日志**:`Log.Message`/`Warning`/`Error` 硬编码英文,`[SqueakyRatkin]` 前缀(开发者面向,便于全局搜索)。
- **文案面向所有玩家**:发布包内文本(Sounds/README.txt、Keyed、SoundDefs 注释、README/AUDIO_GUIDE)用中性措辞,禁私人化(朋友/好友/friend);commit message 同样不指名特定人。2026-07 已清 20 处。

## Architecture Decisions
- **数据驱动**:触发模式/间隔/概率/心情调制在 patch XML(`actions`/`moodMods`),改行为不重编译。
- **运行时调制**(纠正过过度设计):心情靠 `SoundInfo.pitchFactor`(×pitchJitter)/`volumeFactor` 运行时设,每动作 1 个 SoundDef(16 个,非 64 矩阵);SoundDef 用中性 `pitchRange`(0.95,1.05)容许调制叠加。
- **三层配置**:CompProperties(XML 默认)← ModSettings.moodOverrides(玩家 override)← useCustomOnly(纯自定义 vs 混合原版)。
- **default 原版豚鼠**:SoundDef grain 指向 `Pawn/Animal/GuineaPig/{Call,Pain,Angry}`;自定义音频(ogg)就位后取消 grain 注释生效;多 grain 随机(原版机制)。
- **Move/Sleep 一次性**:不持续音,RandomOneShot 偶发(用户决定简化,弃 sustainer)。
- **mote 仿原版**:`SR_Mote_TextBg` 用 `ParentName="MoteBase"` + graphicData(`Things/Mote/Transparent`)+ drawGUIOverlay + MetaOverlays + realTime(仿 `Mote_Text`)。
- **mote 位置可配置**(2026-07):`SR_Mote_TextBg` 加 `modExtensions/SqueakMoteOffset(offsetX/offsetY)`,`MoteTextWithBackground.DrawGUIOverlay` 读取并叠加到 worldPos;默认 offsetY=1.2(头顶)。改 XML 调位置不重编译。
- **Death 死亡音效**(2026-07):`Pawn.Kill` Prefix patch(在 DeSpawnOrDeselect/SetDead 前触发,pawn 仍 spawned,position 有效);`SR_Death`/`SR_Death_Pure` 复用 GuineaPig/Pain grain,pitchRange 0.85~0.95 低沉;被击杀与流血而亡统一走 Kill,一 hook 覆盖。

## Evidence Pointers
- 仓库规则 + 记忆协议:`AGENTS.md`
- 当前任务:`TODO.md`
- 冷归档(失效决策):`OBLIVIONIS.md`
- 用户/贡献者文档:`README.md`(EN)/ `README.zh-CN.md`(中)/ `CONTRIBUTING.md`
- 音频指引:`1.6/Sounds/Squeak/AUDIO_GUIDE.txt`
- 上云流程留档(工作区共享):`../modding_documents/RimWorld_Mod_CleanCloudRelease_Workflow_zh.md`
