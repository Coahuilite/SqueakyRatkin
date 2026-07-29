# 鼠辈啁啾设置 UI 产品合同

> 本文是当前设置产品合同。通用 IMGUI 建议见 [`rimworld-mod-ui-design-methodology-zh.md`](./rimworld-mod-ui-design-methodology-zh.md)；运行时与兼容边界见 [`project-architecture-contract.md`](./project-architecture-contract.md)。

## 产品表面

普通玩家始终看到三个任务页：**发声规则**、**心情音色**、**语音来源与异种**。连续点击稳定版本入口七次后，才出现第四页 **开发与诊断**；未解锁时没有占位、锁图标或暗示入口。第四页收纳诊断、日志、音频浏览/路径与版本信息，不改变 RimWorld Dev Mode、地图或 Pawn 选择的既有门控。

Biotech 激活时，每个精确 `XenotypeDef.defName` 对应一个 preset；空或缺失字段回退全局默认值。UI 显示本地化名称和 Xenotype 图标，并可提供 `defName` 技术信息；本地化、图标和发现信息不是资格门。仅由 HAR 发现的 Core/Ludeon 原版 Xenotype 不单独创建候选行；已有行为 preset、异种语音包选择或声明 VoicePack 的精确目标仍显示。未激活 Biotech 时不访问 Xenotype DefDatabase 或 pawn genes。

设置是 **immediate**：语义输入一经接受即发布到运行时，约 350 ms 合并保存；关闭窗口必须 flush 最近待保存值。没有 Apply、Revert、dirty-close guard 或草稿交易。失联目标保留以便来源模组恢复，但必须提供明确的“忘记此目标”入口和破坏性确认；确认后同时删除该目标的行为 preset 与异种语音包选择，普通刷新不得静默删档。外壳的 footer/status/version 是稳定区域，保存状态使用永久预留槽位，不因 Saving、Saved、Failed 改变导航或页面高度；失败如实呈现，保留待保存代次但不无限自动重试。

切换至 Remix 走既有两步确认；任何取消、关闭或中间步骤都不得改变 canonical 模式，也不得让点击穿透到后方控件。

玩家排障工具必须编译进所有 Dev、GitHub 与 Steam flavor：Developer menu 的 DebugAction 可用性受 RimWorld Dev Mode 与活动地图门控。成功派发记录开启后，正式派发的短暂结果悬浮字仅受该记录开关与 Pawn 在地图内状态控制；相机指示器及其他 live overlay 维持各自的 RimWorld Dev Mode/活动地图门控。相机指示器读取真实的 `Find.Camera.transform.position.y` 与 `Find.Camera.orthographicSize`，不得由 `RootSize` 反推；声音 preview 位于 ModSettings workbench，无需 DevMode。

## capability 与布局

主菜单、无地图、无选择 Pawn 都是受支持状态：普通页可安全打开，依赖 MapUI/地图/Pawn 的命令明确说明不可用原因。无 Biotech 时，不访问 Xenotype DefDatabase 或 pawn genes；基础设置、保存、动作、mood 与 Race/Vanilla 回退照常可用，异种区安全降级。

响应式是有限模式，不承诺无限伸缩。以实际可用宽高、文本和控件最小尺寸选择 normal、narrow、low-height 等排列；窄宽时表单堆叠，低高度缩小 viewport。先 Measure → Arrange → Draw：测量不写设置或触发副作用，排列决定 Rect、gutter 和滚动边界，绘制只在已分配 Rect 内处理既有语义。滚动有唯一所有者：前两页各有一个主 scroll；第三页宽屏为明确边界的 master/detail 独立 scroll，窄屏改为列表/编辑步骤，每步只有一个主要 scroll。折叠或内容变化后夹紧 scroll position。

固定 footer 从内容 viewport 中扣除；预留 scrollbar gutter，避免滚动条导致列宽跳动。短即时解释用 tooltip；回退顺序、No-DLC、Remix、技术身份等关键长说明由可见 `?` 入口在页内展示，不能遮挡或与相邻命令共享 hit rect。

## 非目标与实现路径

不借 UI 改动改变动作资格、resolver、音频回退、Scribe schema 或 DLC 产品语义；不加入跨模组 UI 框架、共享 DLL、router/store/command bus、布局 DSL；不 patch RimWorld/其他模组 UI；不把 tooltip 当作关键知识唯一载体。

实现位于 `Source/SqueakyRatkin/SqueakyRatkinSettings.cs` 及其设置 UI 相关 partial/helper；游戏上下文由 `SqueakSettingsGameContext` 管理，保存协调器负责 immediate、coalesced save 与 close flush。helper 只处理局部 Rect/绘制/意图，不承担设置写入、Catalog、resolver 或保存。

## 最小验收矩阵

| 维度 | 至少验证 |
| --- | --- |
| 语言与比例 | EN、简体中文；100%、125%、150%。 |
| 空间 | normal、narrow、高度受限；无重叠、负 Rect、不可达控件或滚动串层。 |
| 状态 | 主菜单、游戏内、无选择、无 Biotech、可用/空/orphan/dormant/失败 Catalog。 |
| 输入 | slider、文本、列表、`?`、折叠、Remix 两步确认及每层取消/关闭无穿透。 |
| 持久化 | 立即运行时生效、约 350 ms 合并保存、close flush、重开/重启保留，失败状态诚实。 |
| 运行时 | 页面不引发 GUIClip/ScrollView 配对、NRE 或 Unity 主线程错误；No-DLC 不访问 Biotech 路径。 |
