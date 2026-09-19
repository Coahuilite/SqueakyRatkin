# 文档收敛交付核查

日期：2026-09-19。基线 HEAD：`547e2f2539dec5bc50324433db3fc951097b7f10`，本轮未提交。

- 先完成终审 R1–R10，再按内容重构。原 26 份正文 → 9 份现行 Markdown；完整历史与管道仍在 archive，磁盘总量不作为精简指标。
- 度量为 UTF-8 解码后的 UTF-16 单元，含实际换行：194394 → 48032，日常阅读面减少 75.3%；排除两份不变 CHANGELOG 后 177238 → 30876，减少 82.6%。
- 原文件指纹核验：56/56 通过，逐文件检查字节数与 SHA-256。双语 CHANGELOG 均仍在原位、字节不变；其余原文件在对应 archive 路径，包含 ignored packages。
- 有效 Markdown 本地目标：35 文件检查，0 断链；新导航标题锚点检查通过。历史 archive 正文、旧提示词、TASK 与 OBLIVIONIS 的历史引用不批量重写。
- 热入口的三份合同、runbook 和 Workshop 链接仍指向 docs 根重建文件。根 AGENTS 仅改发布证据目录；MEMORY/TODO/codemap 修正必要引用，TODO 登记新增发现，未全面压缩记忆。
- 日志表：v1 28 行、v2 4 行，与源码版本映射核对；v1 字段补列 pawn/pawn_id。未改协议实现。
- Workshop 双语 BBCode 标签配对通过；LF 规范化后的 Unicode code point 数：中文 1936、英文 4440。两版目标均为 0.3.3，未上传/未验证线上。
- 终审原编号全集：CNF 51、OQ 96、GAP 51；原陈述全文另保留，编号计数不是语义或实机验证通过数。
- `git diff --check` 通过。现有 `pwsh scripts/privacy-audit.ps1` 默认模式 CLEAN；另以现有凭据/路径/ID 模式对含 untracked 的 14 个本轮输出/编辑文件做一次性扫描，0 命中。该扫描不是通用隐私证明；未运行 FullHistory/PrePush，因为本轮不推送。
- Remix：内存编译当前 Kernel，公开 Select 入口用 Ratkin/Call/Race 包+正式内置表复现 None/Race 结果；Off/Fallback 对照正常。只读追踪 adapter/PlayOneShot 未见兜底。未跑 RimWorld 实机。
- Source/scripts/tools/fixtures/.github/About/1.6 零 diff；未改运行时代码或追加测试/CI 门；没有构建、游戏验证、版本变动、提交、push、tag 或发布。

## 剩余项

R6 运行时缺陷未修；R5 日志隐私冲突与 R3 ABI 锚点未裁；US 当前态、迁移导入、发布与历史重写仍需各自证据/授权。见现行 maintenance-status-zh.md。原 packages 的 gitignore 状态保留，归档存在不代表已跟踪。
