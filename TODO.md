# TODO

## 下一会话：0.2.0 发布前收口
- [ ] 先整理 `MEMORY.md` / `TODO.md`：保留当前签署事实和发布阻塞，降级不再指导工作的测试过程；仅在发生历史冲突时读取 `OBLIVIONIS.md`。
- [ ] 审阅当前 dirty 工作树的完整 diff，确认只包含已测试接受的 41 条音频更新、成功派发悬浮字门控修复、双语文案、合同、来源/权利文本与打包断言；不要回退或重复转码音频。
- [ ] 完成发布前文档收口：中英文 README 已与 csproj 0.2.0 对齐；仍须复核 About、CONTRIBUTING、架构/UI/日志合同、VoicePack 指南和 Example README 是否一致。
- [x] 完成 0.2.0 changelog 发布定稿：双语条目已使用实际 UTC+8 时间，覆盖获接受内容、41 条公共领域 Example 音频与诊断修复，且未污染旧版本历史；这不代表 tag、GitHub Release 或 Workshop 更新已经执行。
- [ ] 执行当前树与产物隐私审计：本机绝对路径、用户名、Steam/Workspace 路径、日志片段、密钥/令牌、`PublishedFileId.txt`、PDB、源码和不应分发的本地文件；现有 dirty 测试包证据为 Dev/GitHub 各 115 文件、播放池 41、物理音频 82。
- [ ] 在获得提交授权后，按可审阅边界提交当前未提交改动；提交前检查 `git status`、完整 diff、近期提交，不夹带 dist 或秘密。普通 fast-forward push `dev` 仍需另行明确授权。
- [ ] 提交后从 clean HEAD 重新运行 mandatory build，并按正式用途构建/打包 Dev、GitHub 与 Steam flavor；重新验证 15/41 精确分布、41/41 镜像 SHA-256、Vorbis/22050/mono、完整解码、115 文件白名单、隐私与构建身份。当前 `-dirty` 包只保留为已测试证据，不作为正式发布物。
- [ ] 按逐步授权完成发布链：push `dev` → 创建 `dev`→`main` PR → squash merge → 只在进入 `origin/main` 历史的 squash commit 上创建 `v0.2.0` → push tag → 确认 GitHub CI artifact/release → 构建 Steam flavor 并更新既有 Workshop 条目。禁止直接推送 `main`、在 `dev` 打 tag，或复用 0.1.x 的“首次 Workshop 上传”流程。

## 已完成的 0.2.0 测试证据
- [x] 用户确认当前 0.2.0 测试版本已完成测试并接受进入发布前收口；此前音频听感、UI 可达性、异种行为、日志字段和成功派发悬浮字复测不再作为未完成测试阻塞。
- [x] 41 条 Example 正式源通过 15 Action 精确分布、Vorbis/22050/mono 与完整解码；Dev/GitHub 测试包各 115 文件，Template/内置各 41 条且镜像哈希一致。
- [x] 当前测试产物：Dev DLL SHA-256 `BFD0587F1C0D1D281C402746CA507123AAB7D9724F2380F33E788D1780DEEC1E`；GitHub ZIP SHA-256 `78137213B8CDBDC32ADCBE21F7D4966A0DF4120730272BCCFB5CF5CEDFD75748`。
- [x] 已补充首次上传后更新既有 Steam Workshop item 的说明：`pack-steam.ps1` 仅 stage；ID 只在本机上传副本使用；后续版本不再使用 `Initial Workshop Upload`。
- [x] 已完成当前工作树路径卫生：`MEMORY.md` 与 `AUDIO_PROVENANCE.txt` 不再暴露本机盘符、用户名或 Desktop 位置；41 条权威来源事实未改变。

## 后续版本（不属于 0.2.0，不阻断本次发布）
- [ ] 治理 `SqueakLog`：先建立 srdiag v1 characterization checks，再机械拆分 internal registry/build/once/formatter/sink；不得改变 public typed facade、字段顺序、once key 或事件 ID。
- [ ] 修复退出模组设置后七击开发选项的未完成点击计数未归零：检查原生关闭按钮、Esc 与 WindowStack 移除是否都到达 `EndSettingsSession`，不要改变已解锁状态。
- [ ] 仅在再次复现或伴随功能故障时归因 `Accessing TicksAbs but gameStartAbsTick is not set yet`；届时使用无本模组对照或 Dev-only 一次性堆栈探针，不盲改游戏时钟、不吞掉原版错误。
