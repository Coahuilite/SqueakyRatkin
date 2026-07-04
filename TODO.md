# TODO

## Current Goal
正式 0.1.0 发布前收口:完成 rc1 反馈精调、频率控制与发布卫生,准备 PR dev→main + tag `v0.1.0`。

## In Progress
- [ ] rc1 反馈收口与最终调参(玩家听感、触发频率、心情调制、bug)
- [ ] 正式 0.1.0 发版准备:PR dev→main squash merge + tag `v0.1.0` + push(CI release)

## Pending
- [ ] 收集自定义音频(ogg/22050Hz/16-bit/mono,放 `1.6/Sounds/Squeak/<Action>/SR_<Action>_<n>.ogg`,取消 grain 注释)
- [ ] rc1 反馈驱动调参(改 Ratkin_AddSqueakComp.xml,不重编译)
- [ ] Steam Workshop 上传(pack-steam.ps1 → RimWorld 开发者模式上传)
- [ ] 下一阶段架构设计:组件资格判定
- [ ] 下一阶段架构设计:xenotype/gene 目标配置
- [ ] 下一阶段架构设计:动作元数据拆分
- [ ] 下一阶段架构设计:长音频/配音策略

## Completed
- [x] 打包脚本职责缩减:build 由调用方/workflow 负责,`pack-*` 只 staging/zip,减少 GitHub Actions 重复构建耗时;branch CI dev artifact 改用 Dev flavor。
- [x] ModSettings QoL:新增距离衰减动态图表(衰减开始前全音量→区间内线性衰减→衰减结束后静音)并用 GapLine/短标题分隔音源与触发节奏、距离衰减、心情调制工作台;`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj` 通过。
- [x] rc1 反馈收口:默认 1x 发声节奏保守放慢,仅改 `Ratkin_AddSqueakComp.xml` 数据默认值(无 C# 硬编码)。
- [x] Dev 调试:新增仅 `SQUEAKY_DEV` 编译的右侧摄像机指示器/Camera Indicator(DebugAction 开关 + `GlobalControlsUtility.DoDate` postfix,本地化显示 height/view size,SC:高度/视野)。
- [x] 正式 0.1.0 发布前:per-pawn 全局冷却(`globalMinIntervalTicks`)已落地,玩家可用 `globalCooldownMultiplier` 调整全局触发间隔,Death 绕过全局冷却避免被吞。
- [x] 正式 0.1.0 发布前:新增倍速冷却补偿选项(`scaleCooldownWithTimeSpeed`,默认开启),按 `TickRateMultiplier` 放大全局/动作有效冷却。
- [x] 正式 0.1.0 发布前:修复高倍速声音显著变小:移除按 `TickRateMultiplier` 压低单次 `volumeFactor`,高倍速降噪改由冷却补偿承担。
- [x] 正式 0.1.0 发布前:新增字段驱动冷却时钟(`cooldownClock`),Select 配置 Realtime + ignoreGlobalCooldown,暂停时仍保留玩家反馈。
- [x] 正式 0.1.0 发布前:启动日志加入强区分 build 标识(dev 提交号/GitHub tag/Steam 包版本),减少旧版反馈误判。
- [x] 正式 0.1.0 发布前:新增 `pack-dev.ps1`,明确本地测试只能用 Dev flavor；GitHub flavor 仅 CI/tag release；Steam flavor 仅 Workshop 上传。
- [x] 正式 0.1.0 发布前:新增距离衰减预设/自定义设置,并改为 XML 驱动的玩家可见衰减区间(保守 15~65,适中 15~50,强力 15~40;手动改值自动 Custom,运行时覆盖游戏内 SR_* distRange)。
- [x] 正式 0.1.0 发布前:发布卫生修正(README/README.zh-CN/About/AUDIO_GUIDE 对齐 distRange/工作台预览/Death,补 Death README)。
- [x] 生成 `docs/` 规划与工程审阅文档(仓库状态、架构规划、独立 oracle 工程审阅)
- [x] 生成根目录文案人工审核表 `TEXT_REVIEW.md`(含 About、Keyed、README、CONTRIBUTING、AUDIO_GUIDE、XML 注释、开发者菜单硬编码字符串)
- [x] rc1 反馈排查:Ratkin 派系/pawn 无法生成已解决；原因是玩家未更新模组、仍使用旧版已知 bug 代码，更新后恢复。
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
- 无

## Known (not blocking)
- CI Node 20 deprecation 警告:来自 GitHub 官方 actions,功能不受影响,无需操作。
- dev 与 main 历史独立:已 merge 建立共同祖先 afa00cb,PR 可比较;未来 dev→main squash 正常。
