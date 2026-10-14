# Release 0.3.4 准备与发布记录

观察日期：2026-10-14（UTC+8）。本记录分开保留本地准备证据与远端发布证据；Steam 上传和公开页面字段已按证据范围核验。

## 范围与验收依据

- 产品版本提升为 0.3.4，About 与 csproj 同步；主包和示例音源声明支持 RimWorld 1.6 / 1.7，并显式共用 `1.6/` 内容。
- 运行时 C# 行为不变。维护者于 2026-10-14 在 RimWorld 1.6.4871 使用 Steam 0.3.4 测试，未发现问题；最新只读 `Player.log` 无 SR 失败，另有两条原版 `PrefsData` XML 未知窗口位置字段错误，来源未确认。
- 1.7 验收依据仍是 Steam 0.3.3 在 RimWorld 1.7.4977 的既有试玩与日志证据；0.3.4 没有在 1.7 新测。游戏目前已切回 1.6；覆盖限制见 [维护状态](../maintenance-status-zh.md)。
- 日志 characterization 只拆分合成绝对路径的源码字面量，运行时测试输入不变；Release/Dev 冻结字节检查通过。
- 英文与简中 Workshop 正文均已公开核验；更新说明仅在两份 CHANGELOG，保持历史已发布正文。
- 文档收尾按维护者确认删除隐私历史重写方案草案与三份旧流程复盘；现行合同和版本发布记录保留。旧文档可从 Git 历史查阅，本次不重写 Git 历史。

## 已执行检查

| 项 | 观察结果 |
| --- | --- |
| 本地套件 | `verify-local.ps1 -NoRestore` 12 项通过；原 11 项与新增回归子进程接线均通过 |
| 发布脚本回归 | `verify-release-gates.ps1` 最终 16 项通过：PNG 元数据位置/解压/超限/坏压缩、像素排除、OGG 音频排除、改后缀的 PNG、UNC 与已删除历史 PNG |
| 构建 | Dev / Steam / GitHub 的 Release 构建通过，`--no-restore`、warnings-as-errors；0 warnings / 0 errors；DLL FileVersion 均为 0.3.4.0 |
| 候选生产 | 准备提交 `90573b2b6290` 上的三个包均为 116 个文件，版本标签 0.3.4、渠道标签正确、提交标签不带 dirty；各自指定 StageFlavor 的发布 readiness 通过 |
| 拒绝旧包 | 旧 Steam 0.3.3 包返回失败；归档后缺失 Steam 候选也返回失败，避免没有候选包仍显示发布通过 |
| 隐私 | 准备提交上 `privacy-audit.ps1 -FullHistory -PrePush` 退出 0；266 个可达 revision、62 个历史二进制 blob、构建 DLL 与当前树已检查；76 项 known-debt 单列接受，不代表消除 |
| 文档 | XML、相对链接、Git diff whitespace 检查通过；两份 CHANGELOG 的历史已发布正文保留 |
| 本地自动门 | pre-push hook 已安装，本仓 hooksPath 指向 `scripts/githooks`，hook 为 LF / 100755；CI 和 Release CI 接上完整历史门 |
| GitHub 公开资产下载核验 | ZIP 与 Release API asset id `639973649` 相符：1,590,775 bytes、SHA256 与 API digest 相同；`ZipArchive` 实读 116 个文件，0 个 PDB、0 个 `PublishedFileId.txt`，仅含 1 个 SR DLL |

上表的本地准备项以准备提交 `90573b2b6290` 为源码锚；发布后的 GitHub tag、公开资产与 Steam 候选身份单独记录如下。

## GitHub 与 Steam 发布观察

- GitHub PR [#40](https://github.com/Coahuilite/SqueakyRatkin/pull/40) 已 merge。`v0.3.4` 指向 main squash 提交 `bda3cf83b1b13dd3237aa6bf73e2816369cd08cd`。
- Branch push CI `38545407657`、PR CI `38545429256`、main CI `38545788524`、Release CI `38546828361` 均成功。
- GitHub [v0.3.4 Release](https://github.com/Coahuilite/SqueakyRatkin/releases/tag/v0.3.4) 于 `2026-10-14T10:04:50Z`（UTC+8 18:04:50）发布。资产 `SqueakyRatkin-v0.3.4.zip` 为 1,590,775 bytes；Release API asset id 为 `639973649`，实际下载 ZIP 的 SHA256 `c0b5c6f2e34b81b1df996a4a018b93a1c35e7b2ad2bf05428ccbc720279610c9` 与 API digest 相符。ZIP 核验副本存于 `dist/verification/github-v0.3.4`。
- ZIP 实读含 116 个文件、无 PDB、无 `PublishedFileId.txt`，仅有 1 个 SR DLL。`version.txt` 标记 `0.3.4` / `build=github` / commit `bda3cf8`；About 中 `packageId` 正确；包含 `v1.6`、`v1.7` 加载根并共用唯一的 `1.6` 内容目录。DLL FileVersion 为 `0.3.4.0`、ProductVersion 为 `v0.3.4+bda3cf83b1b1`。DLL 仅读取身份信息，未执行程序集。
- GitHub Release notes 由 GitHub 根据 PR 自动生成；CHANGELOG 保持项目更新记录，不接入该自动生成流程。
- 上传使用已通过 readiness 的 tag 固定候选包，未重建。SteamCMD 上传进程退出码为 0；原生 depot 构建日志于 2026-10-14 18:40:41（UTC+8）报告新 manifest 创建成功，其 manifest 与本次上传一致。
- 本次上传记录 6 个文件变更、0 个新增，并从旧线上包移除 `About/PublishedFileId.txt`。
- 无登录公开 Steam API 返回 `result=1`、`visibility=0`、`banned=0`，更新时间 `2026-10-14T10:40:50Z`（UTC+8 18:40:50），文件大小 1,910,256 bytes。Change Notes 最新为 0.3.4，中英的版本提升、1.7 支持和两句庆祝句均可见。
- 公开英文描述与[页面维护源的英文 BBCode 正文](../steam-workshop-page-copy-draft.md)在 LF/trim 后逐字符一致，共 4,505 字符。本次 VDF 不含 description 字段，因此不推断该正文由谁或何时编辑/同步。直接公开读取 `?l=schinese` 页面确认简中正文已上线：可见中文开头、模组版本、0.3.4、1.7 与中文结尾；未做简中全文逐字符比对。
- 初次页面观察的 tag 为 `Mod` 与 `1.6`；维护者随后明确自行处理 `1.7` 标签，不安排 agent 复核。页面预览存在但本轮未做视觉比对；也未下载 Steam 玩家侧二进制。
- Steam 文件信息：`dist/steam/SqueakyRatkin` 共 116 files，DLL FileVersion `0.3.4.0`、ProductVersion `0.3.4+bda3cf83b1b13dd3237aa6bf73e2816369cd08cd`，DLL SHA256 `cdbd0ba1c7699f807935e9addd30ad4d11d855f25e393e3752df313c6786c891`，readiness 已通过。

## 旧产物与历史债务

- 三个 0.3.3 目录保留在 `dist/archive-0.3.3-20261013T191632Z/`；原渠道内旧 ZIP 保留，不作为 0.3.4 上传候选。
- 额外历史 PNG 经目检及维护者确认，是已停用的宣传图，不在当前跟踪树或发布包中。当前树只有 `About/ModIcon.png` 与 `About/Preview.png`。压缩像素中的疑似路径字节是误报。
- 新规则的 44 项非个人命中已按维护者裁定登记精确例外；既有债务的 25 项用户名排障目录由维护者确认继续方案 A（不重写 + known-debt 台账）。原路径不复制进本记录。

## 外部状态与限制

GitHub：v0.3.4 已发布，CI 与公开资产下载核验均成功。Steam：0.3.4 上传、公开元数据、英文及简中正文和双语 Change Notes 已核验；未下载 Steam 玩家侧二进制，预览未做视觉比对。`1.7` tag 由维护者自行处理，不安排 agent 复核。

历史重写未执行。当前扫描覆盖指定文本/元数据规则，不识别任意私人叙述；图片视觉内容需要人工检查。0.3.4 沿用运行验收不扩张为逐动作、逐 DLC 或逐模组组合的 1.7 全覆盖。
