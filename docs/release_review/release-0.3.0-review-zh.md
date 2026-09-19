# 0.3.0 Release Claim Pack（GitHub 完整 + Workshop 页面级核验）

## GitHub Release Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.3.0 |
| 标签 | v0.3.0（严格 SemVer，基版本 = csproj `<Version>`） |
| 源码提交 | main squash `c06a90b360dab8824248e805b64bf4e9730c0007`（PR #23，dev = `a3e26e8`） |
| 发布时间 | 2026-08-21T01:35:38Z（UTC+8 2026-08-21 09:35） |
| 构建 flavor | GitHub（CI tag 触发） |
| DLL 身份 | FileVersion 0.3.0.0；Informational v0.3.0+c06a90b360da |
| CI | Release workflow run `32436779849` success |
| 资产 | SqueakyRatkin-v0.3.0.zip（1,578,195 B） |
| 资产 SHA256 | 3C0AD055E9DDBADAEC53CF2F55BF5B8B38D37D921A1FD2D5EA1C0ECEB54DAB97 |
| DLL SHA256 | 17B9A2660D26BA05B9136D4800CEAF69F014A4F5EB61496714577E7C073DD3CF |
| 包内容 | 116 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；OGG 镜像 SHA256 校验通过；包内 version.txt = `SqueakyRatkin 0.3.0 / build=github / commit=c06a90b` |
| 隐私审计 | v0.2.4..main 完整树扫描 0 真实命中（2 处为审查纪律文案自述误报）；dev↔main 树一致 |

## Steam staging / 发布观察

- staging 包：`dist/steam/SqueakyRatkin` 116 文件、0 pdb、PublishedFileId=0、包内 version.txt = `SqueakyRatkin 0.3.0 / build=steam / commit=c06a90b`；上传人工（无 SteamCMD）。
- 页面观察（2026-08-21，公开 API 只读、无登录态）：result=1、同一 item `3758115669`、title=鼠辈啁啾、time_updated=2026-08-21 10:02 UTC+8、描述含 0.3.0；file_size=1,879,864 B（Steam 自有打包格式，不与本地包直比）。**旧「内置 Example 默认启用」公告仍在线——待维护者在 Steam 编辑器删除后复核。**
- 二进制下载级验证边界如实记录：页面级核验 ≠ 玩家下载内容已验证。

## 渠道状态

- GitHub：**完整**。
- Workshop：**页面级已核验**（版本 0.3.0 已上页面；旧公告删除待维护者编辑后复核）。

## 附：0.3.0 关键事实

- 分类：维护性更新（内部内核重构，对等实现替换）；玩家可见行为与 0.2.4 一致（八面门槛全绿 + 维护者实测 + Player.log 核验）。
- 发布决策：双轨 = 本地 dev 包试用（观察期由维护者实测替代）；热修 = `vX.Y.Z-hotfixN`（下个 z 合并）；措辞 = 内核重构 + 拆分预告不提名（决策文档 §5）。
- 后置修复与工具：BuildFallback 兜底恢复 + `_Preview` 过滤 + 卫生（C12）；Steam 打包纪律（C13）；一键校验 `verify-local.ps1`（C17）；changelog 0.3.0 条目（C18/C19）。
