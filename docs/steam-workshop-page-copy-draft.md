# Workshop 页面文案维护源

> **目标：0.3.3。GitHub `v0.3.3` 首发当日因 R6 Remix 缺陷撤回，同日同版本重发（main `cd90a9e`，2026-09-19）；Steam 待维护者上传**，线上页面仍是 0.3.0 口径（未核验）。发布时须让版本、下载入口与批准产物一致。当前状态与已知问题见 [维护状态](maintenance-status-zh.md)，流程见 [runbook](release-runbook-zh.md)。

本文是中英描述唯一维护源，不是 change note 或发布记录。页面编辑、双预览由维护者执行；agent 仅公开页面只读核验。下载使用 releases 总入口，因此不依赖具体 tag 文案。

## 编辑约定

- 中文正式称“鼠族”，英文 Ratkin；品牌 `鼠辈啁啾` / `Squeaky Ratkin` 不改。玩笑才用“鼠鼠”或 `adorable little mousie` / `mousies`。
- 披露置顶，中文“3A”指 AI 规划/编程/维护，英文标题 `AI-Generated Work Disclosure`；不使用 `Masterpiece`、`rat-rats`、`mousefolk`。
- 两份 BBCode 独立粘贴，仅用保守标签；不写 packageId、重复依赖栏、音频文件统计、开发者入口、迁移操作或制作步骤。作者内容只链接指南。
- 区分 **17 个动作键** 与 **15 个内置音频动作**；BabyFits 不误触真崩溃，专用动作需包提供声音。非玩家鼠族可参与常规触发，玩家主动动作仍有控制权/可响应限制。
- 8000 字符只作编辑目标，不声明为已核实的平台硬上限。改文案后重算字符数；发布前随一次核对确认中英对称、版本/链接、权利措辞及双预览。旧公告是否删除须看实际页面，不能由本文件推断。

## 中文 BBCode

```bbcode
[h2]⚠ 3A 大作声明[/h2]
本模组由 AI 规划、AI 编程、AI 维护。这里的“3A”与开发预算、团队规模和显卡性能无关；实际版本由人类维护者审查、测试、打包和发布。

[h1]鼠辈啁啾[/h1]
[b]模组版本：[/b]0.3.3
[b]适用版本：[/b]RimWorld 1.6

鼠族当然不是不会说话。

鼠辈啁啾为 NewRatkinPlus 鼠族加入基于动作触发的可选声音反馈。呼唤、吃饭、移动、工作、战斗或休息时，符合条件的鼠族会发出短促啁啾；模组不会改变其行为、数值或战斗逻辑。

常规动作也适用于访客、友方、敌对及其他符合条件的鼠族；玩家主动选择和命令的声音反馈要求角色可由玩家控制，选择反馈还要求角色清醒且未倒地。

这里还有一只可爱的鼠鼠……或者不可爱，但谁不喜欢鼠鼠呢？

[h2]不同动作，各有节奏[/h2]
[list]
[*][b]日常与状态：[/b]Call、Eat、Sleep、Wounded。
[*][b]操作与移动：[/b]Select、Move、Draft、Undraft、Equip。
[*][b]生活与关系：[/b]Social、Joy、Work。
[*][b]危险与转折：[/b]Attack、MentalBreak、Death。
[*][b]Biotech 婴幼儿：[/b]Crying、Giggling；需要 VoicePack 提供相应声音，没有可用音频时静默。
[/list]

声音遵守各动作的触发条件、概率、冷却和距离设置。心情在运行时调制音高与音量，让同一套声音有轻微变化。设置分为三个普通页面，修改后立即生效。

Eat 默认覆盖整个进食任务，包括端食物走向餐桌。你可以开启“仅在真正进食（正在摄入营养）时触发 Eat 叫声”，再按需开启“使用成瘾品”以覆盖零营养摄入物。啤酒、仙馔等带营养成瘾品在前一个选项下已经计入；无法识别咀嚼阶段时会回到完整进食任务的判定。

[h2]声音来源[/h2]
内置 Race Example 为前 15 个动作提供声音，自 0.2.3 起新装默认启用，可随时关闭；它没有特殊优先级或额外权重。VoicePack 可以只覆盖部分动作，未覆盖部分按设置尝试其他可用音源；所有层均无可用声音时静默。

内置回退引用 RimWorld Core 中多种动物的声音。它不是纯 Boomrat，也不是纯 GuineaPig；原版资产只通过 Def 与资源路径引用，不随模组重新分发。

[i]这次只有一点豚鼠，我保留了一点，就一点。[/i]

[h2]兼容性[/h2]
[list]
[*]适用于 RimWorld 1.6 与 NewRatkinPlus；依赖见本页 Steam 依赖栏。
[*]官方 DLC 均可选。Core 与所需依赖下，基础动作、Race VoicePack、内置回退、心情和设置仍可用。
[*]Biotech 可按精确且区分大小写的 Xenotype defName 匹配音源，并提供对应婴幼儿动作；无需 Biotech 使用基础功能。
[*]婴幼儿正常的哭闹/咯咯笑不会被当成精神崩溃；Crying/Giggling 与 MentalBreak 是独立动作。
[*]第三方声音包应独立安装，不放入主模组目录。
[/list]

[h2]下载、指南与反馈[/h2]
[list]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/releases]GitHub Releases[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/blob/main/.github/skills/squeaky-voicepack-authoring/SKILL.md]VoicePack 作者指南（中文）[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/issues]GitHub Issues：问题反馈[/url]
[/list]

[h2]许可与音频权利[/h2]
模组代码采用 MPL-2.0。Example 音频是代码许可之外的公共领域素材，项目与贡献者不对其主张版权或相关权利；可用于试听或独立 VoicePack 的起点，完整来源、权利状态及法域说明见仓库权利文件。RimWorld 原版资产不重新分发。

第三方 VoicePack 作者应为自己的音频、文本及其他内容声明适当许可，只发布有权分发的素材。

[i]图书馆大堂的奇怪柱子是什么？那是仇恨吱书！[/i]
```

## English BBCode

```bbcode
[h2]⚠ AI-Generated Work Disclosure[/h2]
This mod was planned, programmed, and maintained with AI assistance. A human maintainer reviews, tests, packages, and publishes each release.

[h1]Squeaky Ratkin[/h1]
[b]Mod version:[/b] 0.3.3
[b]Game version:[/b] RimWorld 1.6

It is not that Ratkin cannot speak.

Squeaky Ratkin adds optional, action-based sound feedback to NewRatkinPlus Ratkin. Eligible Ratkin may make short squeaks while calling, eating, moving, working, fighting, or resting. The mod does not alter their behavior, stats, or combat rules.

Ordinary triggers also apply to eligible visitors, allies, hostile Ratkin, and other Ratkin. Player-initiated selection and command feedback requires a player-controlled pawn; selection feedback additionally requires the pawn to be awake and not downed.

And here is another adorable little mousie... or perhaps not adorable, but who does not like mousies?

[h2]Different actions, different rhythms[/h2]
[list]
[*][b]Daily life and condition:[/b] Call, Eat, Sleep, and Wounded.
[*][b]Commands and movement:[/b] Select, Move, Draft, Undraft, and Equip.
[*][b]Life and relationships:[/b] Social, Joy, and Work.
[*][b]Danger and turning points:[/b] Attack, MentalBreak, and Death.
[*][b]Biotech baby fits:[/b] Crying and Giggling require matching VoicePack audio and remain silent when none is available.
[/list]

Sounds follow each action's trigger conditions, chance, cooldown, and distance settings. Mood adjusts pitch and volume at runtime, adding subtle variation to the same clips. Settings use three regular pages, and changes take effect immediately.

By default, Eat covers the whole ingest job, including carrying food to a table. An optional setting restricts it to gaining nutrition; a dependent option also includes zero-nutrition ingestibles such as drugs. Nutrition-bearing drugs such as beer and ambrosia already count under the first option. If the chewing stage cannot be identified, detection falls back to the whole ingest job.

[h2]Sound sources[/h2]
The built-in Race Example provides audio for the first 15 actions. It has been enabled by default on fresh installs since 0.2.3, can be disabled at any time, and has no special priority or extra weight. VoicePacks may cover only some actions. Uncovered actions try other available sources according to your settings; if no layer has playable audio, they remain silent.

Built-in fallback references several RimWorld Core animal sounds. The pool is neither purely Boomrat nor purely Guinea Pig. Vanilla assets are referenced through Defs and resource paths and are not redistributed.

[i]There is only a little guinea pig left in the mix this time. I kept a bit. Just a bit.[/i]

[h2]Compatibility[/h2]
[list]
[*]Made for RimWorld 1.6 and NewRatkinPlus. Required dependencies are listed in the Steam dependency panel.
[*]All official DLC are optional. Core and the required mods support the base actions, Race VoicePacks, built-in fallback, mood modulation, and settings.
[*]Biotech enables exact, case-sensitive Xenotype defName matching and the corresponding baby-fit actions. It is not required for the base features.
[*]Normal baby crying and giggling do not count as mental breaks. Crying/Giggling and MentalBreak are separate actions.
[*]Install third-party VoicePacks independently, never inside the main mod folder.
[/list]

[h2]Downloads, guide, and feedback[/h2]
[list]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/releases]GitHub Releases[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/blob/main/.github/skills/squeaky-voicepack-authoring/SKILL.md]VoicePack Author Guide (Chinese)[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/issues]GitHub Issues[/url]
[/list]

[h2]License and audio rights[/h2]
The code is licensed under MPL-2.0. Example clips are public-domain material outside the code license; the project and contributors claim no copyright or related rights in them. They may be used for listening or as a starting point for an independent VoicePack. See the repository's rights notices for provenance, full status, and jurisdiction details. RimWorld assets are not redistributed.

Third-party VoicePack authors should state appropriate licenses for their audio, text, and other content and distribute only material they have the right to share.

[i]What is that strange pillar in the library hall? That is The Book of Squeakudges — it has grown as tall as a pillar.[/i]
```

## Steam 更新说明（Change Notes，中英各一段，粘贴用）

```text
[b]0.3.3[/b]
- 新增 Eat 两级粒度开关：默认仍是整段进食任务（含端食物走向餐桌）；可改为只在正在摄入营养时触发，「使用成瘾品」子项覆盖烟卷、薄片等零营养摄入物。啤酒、仙馔等本身带营养的成瘾品在第一个开关下已经计入；无法识别咀嚼阶段时回落到完整进食任务。
- 语音包改为声明所服务的精确种族，并支持按年龄（Baby/Child/Adult）的变体与逐动作包回退。
- 玩家主动发声（选中、命令）仅对玩家可控、清醒、未倒地的角色触发。
- 作者 XML 合同公开冻结：字段只增不改、17 个动作键 append-only、非法包 fail-closed；随附脚手架与自包含作者指南。
- Biotech 婴幼儿的 Crying/Giggling 需要语音包提供音频，未提供时静默；正常的哭闹/咯咯笑不再被当作精神崩溃。
- 诊断日志每个动作窗口合并为一行路由记录，含音源层级、彩蛋标记、派系与玩家控制标记。
- 修复语音源 Remix（混合）在缺少某一层音源时可能整次无声、且内置音源永不入选的缺陷；现在只在可用层之间等权抽取。
```

```text
[b]0.3.3[/b]
- New two-level Eat granularity: the whole ingest job still counts by default (including carrying food to a table). Optionally restrict it to gaining nutrition, with a dependent option that also covers zero-nutrition ingestibles such as drugs. Nutrition-bearing drugs such as beer and ambrosia already count under the first option; if the chewing stage cannot be identified, detection falls back to the whole ingest job.
- VoicePacks now declare the exact race they serve, and support per-age variants (Baby/Child/Adult) plus per-action pack fallbacks.
- Player-initiated selection and command feedback now requires a player-controlled pawn that is awake and not downed.
- The author XML contract is publicly frozen: fields are add-only, the 17 action keys are append-only, and invalid packs fail closed. A scaffold and a self-contained author guide ship alongside.
- Biotech baby Crying/Giggling need matching VoicePack audio and stay silent otherwise; normal crying and giggling are no longer treated as a mental break.
- Diagnostics now write one consolidated route line per action window with sound tier, egg flag, faction, and player-control flag.
- Fixed Voice source Remix when a tier is missing: some events stayed silent and the built-in tier could never be drawn. Remix now draws evenly among available tiers only.
```

## 文案状态

字符数（Unicode code point，LF 换行，不含 fence 与尾换行）：中文正文 1936、英文正文 4440；中文 Change Notes 471、英文 Change Notes 1337。两段正文与 Change Notes 均面向 0.3.3，正文自 2026-09-19 版本起未再改动；Change Notes 增加 R6 Remix 修复条目（0.3.3 撤回重发）。发布前用一次核对确认中英对称、版本、下载入口、权利措辞与双预览；页面编辑与 change notes 粘贴均由维护者执行，agent 只做公开页面只读核验。旧置顶公告是否删除须看实际页面，不能由本文件推断。本文件不替代 Claim Pack。
