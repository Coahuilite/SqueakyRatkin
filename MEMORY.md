# MEMORY

## 当前工作锚

- GitHub 与 Steam 的 **0.3.4 已发布**；当前准备发布 **0.3.4-tagfix1**，仅更新工坊版本标签、无运行时代码改动。基础版本保持 0.3.4；Steam 页面仍记录 0.3.4，待维护者用游戏内 Update 手工发布；详见 [0.3.4-tagfix1 记录](docs/release_review/release-0.3.4-tagfix1-review-zh.md)。
- 版本源为 `Source/SqueakyRatkin/SqueakyRatkin.csproj` 的 `<Version>`，About 跟随；当前使用 net472 与 1.6 引用程序集，没有单独的 1.7 DLL。
- 本次 0.3.4-tagfix1 沿用 RimWorld 1.6 下 Steam 0.3.4 与 1.7 下 Steam 0.3.3 的既有验收；没有本版本新实测，支持仍为 1.6 / 1.7 共用 `1.6/` 内容。
- Workshop 正文维护于 [页面草稿](docs/steam-workshop-page-copy-draft.md)；更新说明维护于双语 CHANGELOG。GitHub Release notes 仍按 PR 自动生成；本次 Steam Change Notes 由维护者手工录入，不做外部同步。发布证据见各版本 review。

## 当前约定与维护状态

- 路由只认 VoicePack 的精确 `raceDefName`；HAR 仅反射增强，失败不得崩溃。产品依赖与发现资格是两回事。
- 卸载安全：不向存档写永久状态；设置/profile 只存 Config。旧版本证据不扩张为未经测试的新版本结论。
- 作者 XML ABI：字段 add-only、17 动作键 append-only、fail-closed。Eat 默认 job 级派发；具体行为见 [架构合同](docs/project-architecture-contract.md)。
- 日志 `srdiag`：v1 28 事件字节冻结，v2 4 事件；协议调整联动 `tools/SqueakLogCharacterization`，隐私冲突保留待裁决。
- 每个 minor 从 `main` 建版本分支，经 PR 入受保护 `main`；不使用 `dev`。直接使用 `.csproj` 与脚本，已删除的 `SqueakyRatkin.slnx` 不恢复。
- 本地校验/发布/隐私的三命令分工以 [runbook](docs/release-runbook-zh.md) 为准，不另造人工仪式。安装上传工具不自动授权外部发布。
- SR 1.0 的 US 硬依赖、Def 归属、不自动迁移设置与共存双响裁定继续有效；完整决定及未决项集中在维护状态。
- 隐私历史沿用已确认的方案 A（不重写 + 台账）；不再维护方案 B 草案。个人路径、原始日志、凭据与 Workshop item ID 不入库。
- 本轮脚本以已审阅的 US scripts 为基线升级；SR 台账只接受精确 blob/path/pattern。新增 44 项非个人路径示例/合成测试按维护者裁定保留历史，合计 76 项例外；维护者已确认既有 25 项含用户名排障目录继续按方案 A 保留，不重写历史。当前文件不使用历史例外。
- 0.3.3 的三个 `dist` 旧目录已归档在 `dist/archive-0.3.3-20261013T191632Z/`；不是 0.3.4 发布包。Steam 0.3.4 候选固定于发布 tag，不随文档收尾提交重建。额外历史 PNG 为已停用宣传图，不随当前包发布；压缩像素流中的路径字节命中是误报。
- SteamCMD 已安装；其 0.3.4 上传没有更新工坊版本标签，因此本次改用维护者游戏内 Update。实际 0.3.4 上传事实见旧版 review；旧推荐流程已归档于 `OBLIVIONIS.md`。

## 读取入口

`AGENTS.md` 管长期原则与读取入口；`TODO.md` 只列未完成动作；三份行为合同及 runbook 管实现与流程。未安排实施的 US 迁移、退役条件与技术裁定见维护状态，不作为本轮 TODO。`OBLIVIONIS.md` 为冷归档，不能覆盖当前裁定。
