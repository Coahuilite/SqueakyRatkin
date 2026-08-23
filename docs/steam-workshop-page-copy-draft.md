# Steam Workshop 页面文案维护源

## 用途、来源与维护规则

- 页面：<https://steamcommunity.com/sharedfiles/filedetails/?id=3758115669>。
- 本文是 Workshop 页面中英文描述的维护源，不是发布记录，不包含 change note。Steam 页面实际是否已经更新，不能从仓库状态推断。
- 技术事实以当前架构合同、发布产物与已批准的玩家文档为准；页面更新后必须在 Steam 编辑器及实际页面中分别人工预览、核对。
- BBCode 仅使用保守子集：`[h1]`、`[h2]`、`[h3]`、`[b]`、`[i]`、`[list]`、`[*]`、`[olist]`、`[url]`、`[hr][/hr]`、`[code]`。中英文是两份可独立粘贴的完整描述，不混排。
- 不在页面展示 `packageId`，不重复 Steam 依赖栏已经列出的依赖模组，不把页面文案当作更新说明。
- 两份描述均应尽量保持在 Steam 常见的 8000 字符以内；字符数在代码块外维护，内容变更后重新统计。

## 术语与文风

- 正式或技术英文使用 `Ratkin`；明确的玩笑处只使用 `adorable little mousie` / `mousies`，禁止 `rat-rats`、`mousefolk`。
- 中文中“鼠鼠”“鼠辈”仅用于模组名或明确玩笑；普通正式说明使用“鼠族”。
- 中文“3A”固定指 AI 规划、AI 编程、AI 维护；英文使用严肃标题 `AI-Generated Work Disclosure`，不得使用 `Masterpiece`。
- 开头的披露后直接进入标题与正文；内置 Example 默认启用事实由正文「内置 Race Example」小节承载（0.3.0 起置顶公告已如期删除），披露与标题之间不得插入其他内容。
- 重点说明 15 个动作的设计；只自然说明未覆盖动作会回退，不在 Workshop 描述中展开三种声音模式。
- 页面专注模组本身，不展示：音频统计数字（OGG 总数、每动作数量、SoundDef 数）、开发者排障（七击解锁的开发与诊断页）、版本迁移说明（旧版 audio-selection）、VoicePack 制作步骤（作者内容路由到作者指南，页面只留链接）。

## Example 与兼容事实

- 内置普通 Race Example 覆盖全部 15 个动作，音频为 public-domain；页面不写音频统计数字。
- Example 无特殊权重；0.2.3 起新装默认启用内置 Race Example（从未调整过音源策略的旧配置自动迁移一次），显式关闭或选择其它包的配置不受影响。
- 内置 Example 默认启用置顶公告已于 0.3.0 如期删除；默认音源策略 0.3.0 未再调整（本次为维护性更新）。若未来再次调整默认策略，须重新评估置顶公告。
- 0.2.4 起婴幼儿咯咯笑/哭闹不再触发精神崩溃语音（Biotech BabyFits 是正常精神状态发作，非崩溃）；该行为说明已并入兼容性小节。
- 基础运行不需要任何官方 DLC。Biotech 仅为精确 Xenotype `defName` 匹配提供可选增强；没有 Biotech 时，Race 与 Vanilla 仍正常工作。
- 第三方声音必须作为独立 VoicePack 发布和安装，不能装入主模组目录；0.2.0 不会自动迁移旧版 audio-selection（该迁移事实页面不展示，属维护参考）。

## 中文 BBCode

```bbcode
[h2]⚠ 3A 大作声明[/h2]
本模组是一款由 AI 规划、AI 编程、AI 维护的模组。
这里的“3A”与开发预算、团队规模和显卡性能无关。实际版本仍由人类维护者执行审查、测试、打包和发布。

[h1]鼠辈啁啾[/h1]

[b]模组版本：[/b]0.3.0
[b]适用版本：[/b]RimWorld 1.6

鼠族当然不是不会说话。

鼠辈啁啾为 NewRatkinPlus 鼠族 加入基于动作触发的可选声音反馈。呼唤、吃饭、移动、工作、战斗或休息时，符合条件的鼠族会在合适的时机发出短促啁啾；模组不会改变其行为、数值或战斗逻辑。

它不只照顾殖民地成员。访客、友方、敌对与地图上其他符合条件的鼠族也走同一套触发与声音选择通路。

这里还有一只可爱的鼠鼠……或者不可爱，但谁不喜欢鼠鼠呢？

[h2]15 个动作，各自有自己的节奏[/h2]

这 15 个动作不是一份声音随机套在所有场景上，而是分别识别游戏中的不同事件：

[list]
[*][b]日常与状态：[/b]Call、Eat、Sleep、Wounded。
[*][b]玩家操作：[/b]Select、Move、Draft、Undraft、Equip。
[*][b]生活与关系：[/b]Social、Joy、Work。
[*][b]危险与转折：[/b]Attack、MentalBreak、Death。
[/list]

每个动作都有独立的触发概率、冷却和距离设置，因此选择反馈可以清楚直接，移动与工作则可以保持克制，不必让整张地图持续吵闹。距离控制哪些声音值得传到镜头附近；冷却避免同类事件在短时间内挤在一起。

当前心情还会在运行时调制音高和音量。同一套声音会随状态产生轻微差别，但不会要求 VoicePack 作者为每种心情制作一整套音频。设置分为 3 个普通页面，修改后立即生效。

VoicePack 可以只覆盖其中一部分动作。某个动作没有合适的自定义声音时，会自然回退到仍可播放的层级，而不是因为缺一条音频就静音。

[h2]原版回退声音[/h2]

主模组按动作引用 RimWorld Core 中不同动物音效组成的混合池。它不是纯 Boomrat，也不是纯 GuineaPig；原版资产只通过游戏的 Def 与资源路径机制引用，不会随本模组重新分发。

[i]这次只有一点豚鼠，我保留了一点，就一点。[/i]

[h2]内置 Race Example[/h2]

内置 Example 是一个覆盖全部 15 个动作的完整 Race VoicePack。它没有特殊优先级或额外权重；0.2.3 起新装默认启用，可随时在设置中关闭。

这些 Example 音频是公共领域素材，可用来试听，也可作为制作独立 VoicePack 的起点。你可以使用、复制、修改和再分发这些音频；完整的权利状态、来源与法域免责声明见 GitHub 仓库中的权利说明。

[h2]兼容性[/h2]

[list]
[*]适用于 RimWorld 1.6 与 NewRatkinPlus；所需依赖请查看本页 Steam 依赖栏。
[*]所有官方 DLC 都是可选内容。只使用 Core 与依赖模组时，Race VoicePack、原版回退、15 个动作、心情调制和设置仍可工作。
[*]Biotech 启用时，可按精确且区分大小写的 Xenotype defName 匹配声音；这是可选增强，不是基础功能的前提。
[*]0.2.4 起，婴幼儿的咯咯笑/哭闹（Biotech BabyFits 正常精神状态）不再触发精神崩溃语音。
[*]不会重新分发 RimWorld 原版音频，也不会把第三方 VoicePack 装入主模组目录。
[/list]

[h2]下载、指南与反馈[/h2]

[list]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/releases/tag/v0.3.0]GitHub Release v0.3.0[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/blob/main/.github/skills/squeaky-voicepack-authoring/SKILL.md]VoicePack 作者指南（中文）[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/issues]GitHub Issues：问题反馈[/url]
[/list]

[h2]许可与音频权利[/h2]

模组代码采用 MPL-2.0。内置 Example 的音频是代码许可之外的公共领域素材；项目与贡献者不对这些音频主张版权或相关权利。RimWorld 原版资产仅按 Def 或资源路径引用，不会重新分发。

第三方 VoicePack 作者应为自己的音频、文本与其他内容选择并声明适当许可，只发布自己有权分发的素材。

[i]图书馆大堂的奇怪柱子是什么？那是仇恨吱书！[/i]
```

维护字符数（中文 BBCode 代码块内，包含标签与换行）：2101。

## English BBCode

```bbcode
[h2]⚠ AI-Generated Work Disclosure[/h2]
This mod was planned, programmed, and maintained with AI assistance.
A human maintainer reviews, tests, packages, and publishes each release.

[h1]Squeaky Ratkin[/h1]

[b]Mod version:[/b] 0.3.0
[b]Game version:[/b] RimWorld 1.6

It is not that Ratkin cannot speak.

Squeaky Ratkin adds optional, action-based sound feedback to NewRatkinPlus Ratkin. At suitable moments, eligible Ratkin may make a short squeak while calling, eating, moving, working, fighting, or resting. The mod does not alter their behavior, stats, or combat rules.

It is not limited to colony members. Visitors, allies, hostile Ratkin, and other eligible Ratkin on the map use the same trigger and sound-selection path.

And here is another adorable little mousie... or perhaps not adorable, but who does not like mousies?

[h2]15 actions, each with its own rhythm[/h2]

The design does not apply one random sound to every situation. It recognizes 15 distinct kinds of game events:

[list]
[*][b]Daily life and condition:[/b] Call, Eat, Sleep, and Wounded.
[*][b]Player commands:[/b] Select, Move, Draft, Undraft, and Equip.
[*][b]Life and relationships:[/b] Social, Joy, and Work.
[*][b]Danger and turning points:[/b] Attack, MentalBreak, and Death.
[/list]

Each action has independent trigger chance, cooldown, and distance settings. Selection feedback can remain clear and immediate, while movement and work can be kept restrained instead of filling the whole map with constant noise. Distance determines which sounds are worth carrying toward the camera; cooldowns keep repeated events from piling up.

Current mood also adjusts pitch and volume at runtime. One set of clips can gain subtle variation with a Ratkin's condition without asking VoicePack authors to record a full mood matrix. Settings are arranged across three regular pages, and changes take effect immediately.

A VoicePack may cover only some actions. When no suitable custom sound exists for an action, selection falls back naturally to an available tier instead of turning that action silent.

[h2]Vanilla fallback sounds[/h2]

The main mod references action-specific pools assembled from several RimWorld Core animal sounds. The pool is neither purely Boomrat nor purely Guinea Pig. Vanilla assets are referenced only through the game's Def and resource-path systems and are not redistributed with this mod.

[i]There is only a little guinea pig left in the mix this time. I kept a bit. Just a bit.[/i]

[h2]The built-in Race Example[/h2]

The built-in Example is a complete Race VoicePack covering all 15 actions. It has no special priority or extra weight. Since 0.2.3 it is enabled by default on fresh installs and can be disabled in settings at any time.

The Example clips are public-domain material. They can be used for listening or as the starting point for an independent VoicePack. You may use, copy, modify, and redistribute them; see the rights notice in the GitHub repository for the full status, provenance, and jurisdiction disclaimer.

[h2]Compatibility[/h2]

[list]
[*]Made for RimWorld 1.6 and NewRatkinPlus. See the Steam dependency panel on this page for required mods.
[*]All official DLC are optional. With Core and the required mods, Race VoicePacks, Vanilla fallback, all 15 actions, mood modulation, and settings continue to work.
[*]When Biotech is active, sounds may target an exact, case-sensitive Xenotype defName. This is an optional enhancement, not a requirement for the base feature.
[*]Since 0.2.4, Biotech baby fits (giggling/crying) no longer trigger the mental-break sound.
[*]The mod does not redistribute RimWorld audio, and third-party VoicePacks must not be installed inside the main mod folder.
[/list]

[h2]Downloads, guide, and feedback[/h2]

[list]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/releases/tag/v0.3.0]GitHub Release v0.3.0[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/blob/main/.github/skills/squeaky-voicepack-authoring/SKILL.md]VoicePack Author Guide (Chinese)[/url]
[*][url=https://github.com/Coahuilite/SqueakyRatkin/issues]GitHub Issues[/url]
[/list]

[h2]License and audio rights[/h2]

The mod's code is licensed under MPL-2.0. The built-in Example clips are public-domain material outside the code license; the project and its contributors claim no copyright or related rights in those clips. RimWorld assets are referenced only by Def or resource path and are not redistributed.

Third-party VoicePack authors should choose and state an appropriate license for their own audio, text, and other content, and distribute only material they have the right to share.

[i]What is that strange pillar in the library hall? That is The Book of Squeakudges — it has grown as tall as a pillar.[/i]
```

维护字符数（English BBCode 代码块内，包含标签与换行）：4741。

## 发布前核对

- [ ] 版本事实与实际已发布产物一致；Steam 页面实际更新状态不得从仓库、提交或文案文件推断。
- [ ] 15 个动作固定；页面不含音频统计数字；Example 默认启用（0.2.3 起）的事实正确。
- [ ] 新装默认启用内置 Example；显式关闭不受影响；Example 无特殊权重。
- [ ] No-DLC 基线与精确 Xenotype 匹配的可选 Biotech 边界正确。
- [ ] 页面 URL 与正文链接有效，且正文没有重复 Steam 依赖栏内容。
- [ ] 不显示 `packageId`，不包含 change note，两种语言没有混排。
- [ ] 两份 BBCode 分别在 Steam 编辑器与实际页面人工预览；无错误标签、截断或超长显示问题。
- [ ] 两份描述字符数均低于 8000，且维护元信息中的统计已经刷新。
- [ ] 原版资产不再分发、Example 公共领域权利与第三方作者责任表述正确。
- [ ] 第三方 VoicePack 独立安装边界明确；页面不展开迁移说明、开发者排障与 VoicePack 制作步骤。
- [ ] 中文正式说明使用“鼠族”；英文正式说明使用 Ratkin，玩笑术语仅为 `adorable little mousie` / `mousies`。
- [ ] 访客、友方、敌对与其他符合条件的非殖民者鼠族/Ratkin 已明确纳入同一通路。
- [ ] 3A 披露标题与正文逐字正确；内置 Example 默认启用事实在「内置 Race Example」小节、婴幼儿 fits 行为说明在兼容性小节；0.3.0 起无置顶公告。
- [ ] 每种语言只有一个指定的末尾俏皮句，未混入另一语言的书名或其他结尾梗。
