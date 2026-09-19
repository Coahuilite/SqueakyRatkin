# Squeaky Ratkin 0.2.3 发布核验（Release Claim Pack）

> 依据 `docs/release-runbook-zh.md` 阶段 2-3 流程记录。版本 0.2.3 为**行为变更（优化）**：新装与从未显式选择音源策略的旧配置默认启用内置 Race Example，取代纯 Vanilla 回退作为默认听感。

## GitHub Release Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.2.3 |
| 标签 | v0.2.3（严格 SemVer，基版本 = csproj `<Version>` 0.2.3） |
| 源码提交 | `fca5fa6fbcc6bdacf13f5caa9959d6be2cd33ea7`（main squash `feat: enable built-in Race Example by default (#13)`） |
| 发布时间 | 2026-08-17 22:12 UTC+8（14:12 UTC） |
| 构建 flavor | GitHub（CI tag 触发） |
| DLL 身份 | FileVersion `0.2.3.0`；Informational `v0.2.3+fca5fa6fbcc6` |
| CI | Release workflow run `32038162813` success |
| 资产 | `SqueakyRatkin-v0.2.3.zip`（1,574,271 B） |
| DLL SHA256 | `9e57d4b1e44d2d4b2324e0a6d269ec8d42f5582add436239793045fb8b2ba8e8` |
| zip SHA256 | `cc8327ba54dd1453225578df7b798bce9f86d0963ab1b59d599eedbd0b04c268` |
| 包内容 | 115 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；`LoadFolders.xml` 无门控；`modVersion` 0.2.3；OGG 41（Template 镜像 SHA256 校验） |
| 隐私审计 | 完整树扫描 0 命中；dev↔main 树一致（0 差异） |

## Steam staging（0.2.3，未上传）

| 项 | 值 |
|---|---|
| 构建 flavor | Steam（本地 `pack-steam.ps1`） |
| staging 包 | `dist/steam/SqueakyRatkin`：115 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；OGG 41 镜像校验通过 |
| DLL 身份 | FileVersion `0.2.3.0`；SHA256 `40c5c1c68cb0c28e4a3c041e834a29872a7b34b28bfee850769a474d1ef3ad7c` |
| 上传 | 未执行（待维护者：复制 stage 到上传副本、写 `PublishedFileId.txt`、以同一作者 Update、粘贴中英文案） |

## 渠道状态

- GitHub：完整。
- Workshop：unverified（staging 已核验，上传与页面观察待维护者）。

## 备注

- 发布前 PR/CI 链：dev push CI `32037789607` → PR #13 CI `32037960290` → main push CI `32038064450` 均 success。
- 实机验证（0.2.3 行为）：fresh / 显式 Off / 已有选择三态经 GABP 桥运行时对象核验通过。
- 0.3.x 规划输入随本次发布提交：年龄维度（标签路由 + 调制轴）、设置来源事件 SettingsOrigin（srdiag v2 候选）。
