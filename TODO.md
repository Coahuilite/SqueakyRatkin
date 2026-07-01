# TODO

## Current Goal
游戏内验证本轮调优/新增(mote 位置、声音浏览器、镜头距离衰减、死亡音效)+ 文案去私人化,确认后统一推送 dev。

## In Progress
- [ ] 等用户重启 RimWorld 一次性验证(本地 5 个 commit 未推送):
  - mote 文字位置(offsetY=1.2 是否到头顶,截图微调)
  - 声音浏览器 Play 能响(InMap 修复)
  - 镜头:中近距有声 → 远距线性衰减 → 超 70 格无声(删 zoom gating 后 distRange 生效)
  - 死亡音效:被击杀 + 流血而亡都响(Pawn.Kill prefix)
  - Select 连点 ~4/秒(minIntervalTicks=15)

## Pending
- [ ] 验证通过后 `git push origin dev`(本地待推:3ffb72b/3d5e97c/22a5365/37181eb/4090b4e)
- [ ] PR #1(dev→main squash)merge(用户在 GitHub 网页操作)
- [ ] 收集自定义音频(ogg/22050Hz/16-bit/mono,放 `1.6/Sounds/Squeak/<Action>/SR_<Action>_<n>.ogg`,取消 `SqueakyRatkin_SoundDefs.xml` 对应 grain 注释)
- [ ] Steam Workshop 上传(手动:`pwsh scripts/pack-steam.ps1` → RimWorld 开发者模式"上传模组")
- [ ] 首个 release:`git tag v0.1.0 && git push origin v0.1.0`(CI 自动打包发 GitHub Release)
- [ ] 游戏内调参:间隔/概率/心情阈值(改 `Ratkin_AddSqueakComp.xml`,不重编译)

## Completed
- [x] 立项评估(原版机制核实:类人无发声钩子)
- [x] 核心实现(数据驱动 Comp + 运行时调制 + 三层配置)
- [x] ModSettings 调制工作台(slider+输入+预设+预览)
- [x] Dev 调试系统(DebugAction 菜单 + 悬浮字 + 试听浏览器)
- [x] 中英文本地化(全 Keyed)
- [x] 分发(MPL-2.0 + BuildFlavor + junction + pack + CI/CD)
- [x] 上云(脱敏 + packageId 定死 + 双语文档 + CONTRIBUTING + commit 规范)
- [x] 分支保护(main require PR + enforce_admins)
- [x] dev 原子提交链重构(9 阶段)+ merge main 建立共同祖先
- [x] SR_Mote 红字修复(commit 80a5ab5)
- [x] 清理玩家包(删 Silence/.gitkeep/MANUAL_STEPS,pack 排除 .gitkeep)
- [x] 启动崩溃修复实测通过(Selector 参数名 + FloatRange 分隔符,commit 982331f,已 push dev)
- [x] Select 节流 60→15 ticks(commit 3ffb72b,本地待推)
- [x] mote 位置可配置化 modExtensions offset(commit 3ffb72b + 3d5e97c offsetY 1.2)
- [x] 声音浏览器 InMap 播放修复(commit 22a5365)
- [x] 删镜头 zoom gating,改 distRange 距离衰减(commit 22a5365,架构红线变更)
- [x] Death 死亡音效全链路(commit 37181eb:Pawn.Kill prefix + SR_Death + 枚举 + Keyed)
- [x] 文案去私人化,面向所有玩家(commit 4090b4e,清 20 处)

## Blocked
- 无(等用户重启验证本轮改动)

## Known (not blocking)
- CI Node 20 deprecation 警告:来自 GitHub 官方 actions,功能不受影响,无需操作。
- dev 与 main 历史独立:已 merge 建立共同祖先 afa00cb,PR 可比较;未来 dev→main squash 正常。
