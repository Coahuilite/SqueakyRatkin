# TODO

## Current Goal
v0.1.0-rc1 已发(tag `v0.1.0-rc1`,CI release)。收集内测反馈,精调后发正式 0.1.0。

## In Progress
- [ ] CI release.yml 完成 → GitHub Release v0.1.0-rc1 zip 上线
- [ ] rc1 内测反馈收集(玩家听感、触发频率、心情调制、bug)

## Pending
- [ ] 正式 0.1.0:rc1 反馈精调后,PR dev→main squash merge + tag `v0.1.0` + push(CI release)
- [ ] 收集自定义音频(ogg/22050Hz/16-bit/mono,放 `1.6/Sounds/Squeak/<Action>/SR_<Action>_<n>.ogg`,取消 grain 注释)
- [ ] Steam Workshop 上传(pack-steam.ps1 → RimWorld 开发者模式上传)
- [ ] rc1 反馈驱动调参(改 Ratkin_AddSqueakComp.xml,不重编译)

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
