# MEMORY

## 当前工作锚

- 待发布版本 **0.3.4**；文档、记忆、脚本整理及正式本地提交已完成，三渠道本地候选和发布门已通过，见 [本地准备记录](docs/release_review/release-0.3.4-review-zh.md)。远端尚未发布；运行时行为不变，RimWorld **1.6 / 1.7** 共用 `1.6/` 内容。
- 版本源为 `Source/SqueakyRatkin/SqueakyRatkin.csproj` 的 `<Version>`，About 跟随；当前使用 net472 与 1.6 引用程序集，没有单独的 1.7 DLL。
- 1.7 的运行证据来自 Steam **0.3.3**；维护者允许版本号维护的 0.3.4 沿用既有验收，不重复实机测试。更新日志记录版本号提升、1.7 支持与指定庆祝句；证据来源、渠道与未决项统一见 [维护状态](docs/maintenance-status-zh.md)。
- Workshop 正文维护于 [页面草稿](docs/steam-workshop-page-copy-draft.md)；更新说明 / Change Notes 仅维护于 [英文 CHANGELOG](docs/CHANGELOG.md) 与 [中文 CHANGELOG](docs/CHANGELOG.zh-CN.md)，不在页面草稿重复。0.3.4 GitHub 推送、PR/merge、tag 与 Release 已获授权，正在执行；Steam 仅由我们准备包，上传由维护者本人完成。历史证据在 `docs/release_review/`。

## 当前约定与维护状态

- 路由只认 VoicePack 的精确 `raceDefName`；HAR 仅反射增强，失败不得崩溃。产品依赖与发现资格是两回事。
- 卸载安全：不向存档写永久状态；设置/profile 只存 Config。旧版本证据不扩张为未经测试的新版本结论。
- 作者 XML ABI：字段 add-only、17 动作键 append-only、fail-closed。Eat 默认 job 级派发；具体行为见 [架构合同](docs/project-architecture-contract.md)。
- 日志 `srdiag`：v1 28 事件字节冻结，v2 4 事件；协议调整联动 `tools/SqueakLogCharacterization`，隐私冲突保留待裁决。
- 每个 minor 从 `main` 建版本分支，经 PR 入受保护 `main`；不使用 `dev`。直接使用 `.csproj` 与脚本，已删除的 `SqueakyRatkin.slnx` 不恢复。
- 本地校验/发布/隐私的三命令分工以 [runbook](docs/release-runbook-zh.md) 为准，不另造人工仪式。安装上传工具不自动授权外部发布。
- SR 1.0 的 US 硬依赖、Def 归属、不自动迁移设置与共存双响裁定继续有效；完整决定及未决项集中在维护状态。
- 隐私历史沿用 A（不重写 + 台账）；B 仅为 [方案](docs/privacy-history-rewrite-plan-zh.md)，执行须单独授权。个人路径、原始日志、凭据与 Workshop item ID 不入库。
- 本轮脚本以已审阅的 US scripts 为基线升级；SR 台账只接受精确 blob/path/pattern。新增 44 项非个人路径示例/合成测试按维护者裁定保留历史，合计 76 项例外；维护者已确认既有 25 项含用户名排障目录继续按方案 A 保留，不重写历史。当前文件不使用历史例外。
- 0.3.3 的三个 `dist` 旧目录已归档在 `dist/archive-0.3.3-20261013T191632Z/`；不是 0.3.4 发布包。当前候选身份以各渠道 `version.txt` 为准，收尾提交后同步刷新。额外历史 PNG 为已停用宣传图，不随当前包发布；压缩像素流中的路径字节命中是误报。
- SteamCMD 已安装并完成首次更新；截至本记录尚未登录或上传。Steam 包由我们准备，上传由维护者本人完成；已授权的 GitHub 发布流程正在执行。

## 读取入口

`AGENTS.md` 管长期原则与读取入口；`TODO.md` 只列未完成动作；三份行为合同及 runbook 管实现与流程。未安排实施的 US 迁移、退役条件与技术裁定见维护状态，不作为本轮 TODO。`OBLIVIONIS.md` 为冷归档，不能覆盖当前裁定。
