# TODO

## Current Goal
游戏内验证 SR_Mote 红字修复 + 收集启动日志确认无残留错误。

## In Progress
- [ ] 等用户启动 RimWorld(junction 已建)发 `Player.log`,确认红字消除、mod 正常加载

## Pending
- [ ] 游戏内验证:启动无红字 → 鼠族吱叫各动作触发正常 → 摄像机剔除生效
- [ ] PR #1(dev→main squash)merge(用户在 GitHub 网页操作)
- [ ] 朋友收集自定义音频(ogg/22050Hz/16-bit/mono,放 `1.6/Sounds/Squeak/<Action>/SR_<Action>_<n>.ogg`,取消 `SqueakyRatkin_SoundDefs.xml` 对应 grain 注释)
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

## Blocked
- 无(等用户 Player.log)

## Known (not blocking)
- CI Node 20 deprecation 警告:来自 GitHub 官方 actions(checkout/setup-dotnet/upload-artifact @v4)声明 Node 20 但实际被 force 到 Node 24,功能不受影响,等 GitHub 升级 actions,无需操作。
- dev 与 main 历史独立:已 merge 建立共同祖先 afa00cb,PR 可比较;未来 dev→main squash 正常。
