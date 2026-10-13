# Release 0.3.4 本地准备记录

观察日期：2026-10-14（UTC+8）。本记录证明本地候选准备，不代表 GitHub 或 Workshop 已上线。

## 范围与验收依据

- 产品版本提升为 0.3.4，About 与 csproj 同步；主包和示例音源声明支持 RimWorld 1.6 / 1.7，并显式共用 `1.6/` 内容。
- 运行时 C# 行为不变。维护者接受 Steam 0.3.3 在 RimWorld 1.7 的既有试玩与日志证据；没有新增 0.3.4 实机运行记录，覆盖限制见 [维护状态](../maintenance-status-zh.md)。
- 日志 characterization 只拆分合成绝对路径的源码字面量，运行时测试输入不变；Release/Dev 冻结字节检查通过。
- Workshop 中英文正文供审阅；更新说明仅在两份 CHANGELOG，保持历史已发布正文。

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

本记录中的源码锚是首次正式候选的准备提交。记忆收尾提交后重建包，最终身份以各包 `version.txt` 和 DLL InformationalVersion 为准；上线后另补批准的 main/tag 与渠道资产证据。

## 旧产物与历史债务

- 三个 0.3.3 目录保留在 `dist/archive-0.3.3-20261013T191632Z/`；原渠道内旧 ZIP 保留，不作为 0.3.4 上传候选。
- 额外历史 PNG 经目检及维护者确认，是已停用的宣传图，不在当前跟踪树或发布包中。当前树只有 `About/ModIcon.png` 与 `About/Preview.png`。压缩像素中的疑似路径字节是误报。
- 新规则的 44 项非个人命中已按维护者裁定登记精确例外；既有债务的 25 项用户名排障目录已只在会话展示，是否继续方案 A 待维护者确认。原路径不复制进本记录。

## 外部状态与限制

GitHub / Workshop：**未发布、未做新版本渠道观察**。SteamCMD 已完成安装及启动更新自检，未登录或上传。Workshop 草稿和实际发布渠道待维护者审阅、授权；未创建新 tag、推送、PR 或 Release。CI 接线仅本地审阅，远端运行结果仍待实际 push。

历史重写未执行。当前扫描覆盖指定文本/元数据规则，不识别任意私人叙述；图片视觉内容需要人工检查。0.3.4 沿用运行验收不扩张为逐动作、逐 DLC 或逐模组组合的 1.7 全覆盖。
