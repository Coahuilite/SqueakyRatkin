# MEMORY

## 当前工作锚

- **0.3.4 已在 GitHub 和 Steam 正式发布**；Steam 公开元数据、双语页面正文与 Change Notes 已核验；详见 [发布记录](docs/release_review/release-0.3.4-review-zh.md)。运行时行为不变，RimWorld **1.6 / 1.7** 共用 `1.6/` 内容。
- 版本源为 `Source/SqueakyRatkin/SqueakyRatkin.csproj` 的 `<Version>`，About 跟随；当前使用 net472 与 1.6 引用程序集，没有单独的 1.7 DLL。
- RimWorld **1.6** 下的 SR 0.3.4 Steam 实测通过；1.7 仍沿用 Steam 0.3.3 的既有验收，未新增 0.3.4 的 1.7 实测。游戏现已切回 1.6；日志归因与证据见 [发布记录](docs/release_review/release-0.3.4-review-zh.md)。
- Workshop 正文维护于 [页面草稿](docs/steam-workshop-page-copy-draft.md)；更新说明维护于双语 CHANGELOG，不在页面草稿重复。GitHub Release 正文按 PR 自动生成，不要求接入 CHANGELOG。发布渠道观察和未决动作见 0.3.4 记录；历史证据在 `docs/release_review/`。

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
- SteamCMD 已用于更新现有工坊条目，工具配置留在仓库之外，流程见 runbook。工坊 `1.7` 标签由维护者自行处理，不安排 agent 复核；渠道观察边界见 0.3.4 发布记录。

## 读取入口

`AGENTS.md` 管长期原则与读取入口；`TODO.md` 只列未完成动作；三份行为合同及 runbook 管实现与流程。未安排实施的 US 迁移、退役条件与技术裁定见维护状态，不作为本轮 TODO。`OBLIVIONIS.md` 为冷归档，不能覆盖当前裁定。
