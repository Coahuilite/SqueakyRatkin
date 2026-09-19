# 设置 UI 产品合同

> 2026-09-19 重构。本文规定设置体验与验收边界，自足描述现状；运行时见 [架构合同](project-architecture-contract.md)。

## 页面与能力

常显三页：**发声规则、心情音色、语音来源与异种**。稳定版本入口连续点击七次解锁第四页 **开发与诊断**；未解锁时无占位、锁图标或暗示。第四页集中诊断、日志、音频浏览/路径和版本信息，不改变 Dev Mode、地图及 Pawn 门控。

主菜单、无地图、无选择 Pawn 都是受支持状态。普通页安全可用；依赖地图/Pawn 的命令说明不可用原因。无 Biotech 时不访问 Xenotype DefDatabase/pawn genes，Race/内置回退、全局动作、心情与保存照常可用。

Biotech 下按精确 `XenotypeDef.defName` 建 preset，缺字段继承全局；本地化名称、图标与发现信息只用于显示。已存 preset、音源选择、包声明目标可投影候选；HAR-only 发现不直接生成行，尤其不能仅凭 Core/Ludeon 提示造行。orphan/dormant 保留；确认“忘记此目标”才同时删除行为 preset 和异种音源选择，普通刷新不得删除。

## 输入与保存

- 设置 **immediate**：接受的语义输入立即发布到运行时，约 350 ms 合并保存，关闭 flush；没有 Apply/Revert、草稿交易或 dirty-close guard。
- footer/status/version 是稳定区域。Saving/Saved/Failed 使用固定预留槽，不改变导航或高度；失败如实显示并保留待保存代次，不无限自动重试。
- Remix 两步确认；任意中间步骤、取消或关闭不得改变 canonical 模式或点击穿透。
- Eat 位于发声规则·频率组，是 SR 0.3.x UI 冻结口径的明确控件例外：父“仅在真正进食（正在摄入营养）时触发 Eat 叫声”，子“使用成瘾品”。两者默认 false；父关则子项可见、禁用并清零，PostLoadInit 归一。三模式、营养药物和 toil 未确认回落见架构合同。此次仅 Scribe add-only 增字段、不 bump schema；不代表承诺 schema 永不变。

## 排障与布局

排障工具编译进 Dev/GitHub/Steam 全部 flavor。Developer menu DebugAction 仍要求 Dev Mode 与活动地图；成功派发悬浮字只受记录开关及 Pawn 在地图内控制。其他 live overlay 与相机指示器保留自身 Dev Mode/地图门；指示器读取真实 `Find.Camera.transform.position.y` 与 `orthographicSize`，不得从 RootSize 推算。ModSettings 音频 preview 无需 Dev Mode。详细日志开关独立，见 [日志协议](logging-protocol.md)。

布局按实际宽高选择 normal/narrow/low-height，先 **Measure → Arrange → Draw**；测量无写设置等副作用。窄宽表单堆叠，低高度缩 viewport；前两页各一个主 scroll，第三页宽屏 master/detail 分别拥有明确滚动边界，窄屏列表/编辑分步且每步一个主 scroll。内容/折叠变化后夹紧位置。

固定 footer 从 viewport 扣除，预留 scrollbar gutter。短提示用 tooltip；回退顺序、No-DLC、Remix、技术身份等关键长说明须有可见 `?` 页内入口，不能仅藏在 tooltip，不能遮挡或共用命令 hit rect。

不借 UI 改动更改资格、resolver、回退、DLC 产品语义；不增加跨模组 UI 框架/共享 DLL/router/store/command bus/布局 DSL，不 patch 游戏或他人 UI。除已批准 Eat 例外，不扩展 Scribe 面。

## 维护与验收

实现归 `SqueakyRatkinSettings` 及其 partial/helper，游戏上下文归 `SqueakSettingsGameContext`，保存协调器负责合并与 flush；绘制 helper 不承担 Catalog/resolver/持久化。

| 维度 | 检查范围（要求，不是已通过记录） |
| --- | --- |
| 显示 | EN/简中，100%/125%/150%；normal/narrow/低高度，无重叠、负 Rect、不可达控件或滚动串层 |
| 能力 | 主菜单/地图/无选择/无 Biotech；可用、空、orphan、dormant、失败 Catalog |
| 输入 | slider/文本/列表/问号/折叠；Remix 每层取消关闭；Eat 父子联动，无穿透 |
| 保存 | 即时生效、合并保存、close flush、重开/重启保留，失败诚实 |
| 运行时 | 无 GUIClip/ScrollView 配对错误、NRE、主线程错误；No-DLC 不访问 Biotech 路径 |
