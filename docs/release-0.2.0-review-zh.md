# 0.2.0 发布后复盘与问题探测

> 本文记录 0.2.0 发布后的事实、已发现的问题类别和后续门禁候选；不是现行架构合同，也不替代 `docs/project-architecture-contract.md`。

## 最终发布事实

- PR #7 已 squash；`main` commit 为 `5818dedc3f22a8e7a4286d2b8b48f57377098b3f`。
- tag `v0.2.0` 指向同一 commit。
- GitHub Release：<https://github.com/Coahuilite/SqueakyRatkin/releases/tag/v0.2.0>。
- 发布资产：`SqueakyRatkin-v0.2.0.zip`，115 个文件，SHA-256 为 `7B532A39BAF64FBCC192694B93271A54362E87429074A4C3D9944DACAF42BAB3`。
- DLL identity：`v0.2.0+5818dedc3f22`。

## Release Claim Pack

### Claim status

- **GitHub：verified。**
- **Steam：unverified/manual。**

### GitHub 证据

- version：`0.2.0`；source commit 与 tag：`5818dedc3f22a8e7a4286d2b8b48f57377098b3f` / `v0.2.0`。
- CI workflow：<https://github.com/Coahuilite/SqueakyRatkin/actions/runs/30478707424>。
- asset：`SqueakyRatkin-v0.2.0.zip`；size 1,569,962 bytes；115 文件；SHA-256 `7B532A39BAF64FBCC192694B93271A54362E87429074A4C3D9944DACAF42BAB3`。
- build flavor/identity：GitHub，`v0.2.0+5818dedc3f22`。artifact manifest 与隐私审计无命中。

### Steam 所需观察与明确不主张事项

- 需人工确认同一 item 的实际二进制/版本、visibility、preview 和中英文 description；并记录观察日期与结果。
- 本文不主张 Steam 页面、Workshop 二进制或页面内容已经更新，也不由 stage、GitHub asset 或仓库 tree 推断这些外部手工状态。

## 已发现并处理的问题类别

- 待推送历史曾含个人本地状态。原始开发历史已保留为仅本地归档、未推送；新的 `dev` 从干净基点重建。远端历史不得含个人用户名、主目录、绝对本地路径、安装/订阅状态或秘密。
- 审计同时覆盖最终 tree 与完整待推送范围，而非只检查 HEAD。若任何真实 secret 命中，必须先撤销或轮换，再清理可达历史；清理历史不等于轮换秘密。
- changelog 曾把新行为误写入旧版本历史，已更正；发布说明只描述对应版本的事实。
- dirty 产物只能作为测试证据，不能作为正式发布物；正式包应从 clean commit 后重新构建。
- README、作者指南曾与产品版本及 Vanilla 音池事实漂移，已作为文档一致性问题处理。
- 内置 Example 不自动启用；新装默认仍为 Off/Vanilla，玩家须主动选择并启用 PackDef。
- `PublishedFileId.txt` 只允许存在于本地 Workshop 上传副本，不进入 Git、stage 或发布包。
- HTTPS 受阻时曾一次性改用 SSH 完成必要操作，但未改动 `origin` 配置；这不是常规流程。
- `main` 与 `dev` 曾分叉，但 PR merge-tree 无冲突。
- CI 的 Node 20 弃用 warning 为非阻断警告，不应描述为已经修复。

## 脚本与 CI 观察

- 四个 PowerShell 打包脚本均完成语法检查与实际运行。
- release workflow 具有 SemVer、项目版本与 `main` 祖先关系门禁。
- 目前 `pack-*` 脚本不自行验证输入 DLL 是否为正确 flavor；本地 `pack-github` 不校验 tag 基点是否等于 csproj 版本；PR CI 不打包。这些是改进候选，不是本版已修复事项。

## Steam 状态边界

- 公开页面：<https://steamcommunity.com/sharedfiles/filedetails/?id=3758115669>。
- Steam stage 曾以 `e3ed623...` 的 dev head 构建验证：115 文件、内置/Template 各 41 条音频镜像、0 隐私命中。
- 页面文字、可见性、预览图或 Workshop 二进制是否已由人工更新，需由维护者在 Steam 后台及公开页面人工核对；仓库证据不能自动证明这些步骤已经完成。

## 下一版经验与门禁

1. 先收口 release claim pack，再从 clean commit 构建发布物。
2. 先完成 `main` squash，再在该 commit 创建 tag。
3. 推送前审计最终 tree 和完整待推送 range。
4. 如有真实 secret，先轮换，再清理可达历史。
5. 更新 Workshop 时确认同一 item、实际二进制/版本、可见性、预览和 description。

事故恢复中的特殊操作不应自动固化为日常发布流程；只有经过单独审阅的通用门禁才应进入长期规范。

## 可跨项目复用的最小原则

1. 渠道状态彼此独立，不以一个渠道证明另一个渠道。
2. version、source、build、artifact 与 release 分别取证。
3. 公开产物必须来自 exact clean release commit。
4. 每次渠道 claim 都建立 Claim Pack。
5. 推送审计覆盖完整新可达 range 与最终 tree。
6. 人工外部状态必须人工观察，未知即 unverified。
7. 事故恢复不是日常流程。
8. 文档事实应以机械检查防漂移。
9. Memory 保留耐久决策，不保留过程流水。

## AGENTS/Memory 系统评价

本次评估：AGENTS 约 **8/10**，理念 **8/10**，发布前执行 **4/10**。优点是合同边界、隐私意识和发布顺序已明确；不足是事实验证、产物身份与外部渠道观察没有在发布前形成可执行 Claim Pack。此次改进将渠道真相、exact artifact identity、完整 range 审计、文档事实检查和去过程化 Memory 明确化。评分仅为本次复盘评价，不是永久项目事实。

changelog 的精确分钟与实际渠道发布时间可能漂移；未来可选择记录发布日期，或明确定义其时间 cutoff。本文不修改现有 changelog。
