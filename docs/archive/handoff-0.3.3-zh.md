# 交接：Squeaky Ratkin 0.3.3（更新与发布在后续会话推进）

> 用途：把本轮会话的产出、已定案、未提交状态与下一步分派完整交给后续会话。阅读顺序：`AGENTS.md` → `MEMORY.md` → `TODO.md` → 本文 → 按分派读对应专项文档。
> 日期：2026-08-23。分支：`dev`。**工作树未提交、未推送。**
> 本文不含本机绝对路径、日志摘录或凭据。

## 1. 本轮产出（四块 + 一次版本推进）

| 块 | 内容 | 主要产物 |
| --- | --- | --- |
| A | **Eat 触发粒度两级开关**（玩家可见功能：父「仅真正进食」+ 子「使用成瘾品」，默认双关，子项父关时禁用且强制 false，toil 名未确认时回落完整 job 级） | `Source/SqueakyRatkin/SqueakEatOccurrence.cs`、`CompSqueaker.cs`、设置/UI/Scribe/本地化/单测/合同同步 |
| B | **Eat 语义交接文档**（vanilla Ingest/Eat 事实基线 + 按钮需求规格 + 方案 A/B/C/D） | [`handoff-eat-occurrence-granularity-zh.md`](./handoff-eat-occurrence-granularity-zh.md) |
| C | **SR × US 兼容性检查**（F1–F8、组合矩阵、U1–U4、双开验收矩阵） | [`us-sr-compatibility-check-zh.md`](./us-sr-compatibility-check-zh.md) |
| D | **SR → US 迁移/退役方案**（五条事实基线、P0–P4、身份归属 §8、Q1–Q10） | [`us-sr-migration-plan-zh.md`](./us-sr-migration-plan-zh.md) |
| E | **版本推进 0.3.3**（csproj + About 同步；双语 CHANGELOG 顶端改为 `未发布 — 0.3.3`，注明 0.3.2 仅 GitHub prerelease） | `Source/SqueakyRatkin/SqueakyRatkin.csproj`、`About/About.xml`、`docs/CHANGELOG{,.zh-CN}.md` |

## 2. 当前仓库状态（交付给发布会话的事实）

- 分支 `dev`，与 `origin/dev` 同步，**本轮 0 提交**。
- **19 个已跟踪文件改动**（+161 / −17）**+ 4 个新文件（untracked）**：

| 类别 | 文件 |
| --- | --- |
| 功能代码 | `Source/SqueakyRatkin/CompSqueaker.cs`、`SqueakyRatkinSettings.cs`、`SqueakyRatkinSettings.ExposeData.cs`、**新增** `SqueakEatOccurrence.cs` |
| 本地化 | `1.6/Languages/English/Keyed/SqueakyRatkin.xml`、`1.6/Languages/ChineseSimplified/Keyed/SqueakyRatkin.xml` |
| 验证/夹具 | `tools/KernelCharacterization/{KernelCharacterization.csproj,UnitTests.cs}`、`tools/SettingsFixtureGenerator/Contract/SettingsContract.cs` |
| 版本 | `Source/SqueakyRatkin/SqueakyRatkin.csproj`、`About/About.xml` |
| 合同/指南/codemap | `docs/project-architecture-contract.md`、`docs/settings-ui-product-contract-zh.md`、`.github/skills/squeaky-voicepack-authoring/SKILL.md`、`Source/SqueakyRatkin/codemap.md`、`Source/SqueakyRatkin/UI/codemap.md` |
| CHANGELOG | `docs/CHANGELOG.md`、`docs/CHANGELOG.zh-CN.md` |
| 记忆 | `MEMORY.md`、`TODO.md` |
| 新文档（untracked） | `docs/handoff-0.3.3-zh.md`（本文）、`docs/handoff-eat-occurrence-granularity-zh.md`、`docs/us-sr-compatibility-check-zh.md`、`docs/us-sr-migration-plan-zh.md` |

- **验证状态**：`pwsh -NoProfile -File scripts/verify-local.ps1 -NoRestore` **11/11 全绿**；最后一次运行发生在全部代码与版本写入之后，其后只改文档与 MEMORY/TODO（不涉及编译面）。单检查重跑命令见脚本输出。主模组构建 **0 warning / 0 error**（Dev + Steam 双 flavor）。
- **未做**：commit / push / tag / GitHub Release / Steam 上传 / 打包产物分发——全部属外部有效操作，需维护者显式授权（`AGENTS.md`）。

## 3. 版本与发布面

- 产品版本 **0.3.3**；`csproj <Version>` 与 `About.xml <modVersion>` 已一致（`stage-package.ps1` 会硬断言；release tag 基版本必须等于 csproj 版本）。
- 双语 CHANGELOG 顶端为 `未发布 — 0.3.3` / `Unreleased — 0.3.3`，发布时替换为实际 UTC+8 时间。
- Steam **仍阻断**（0.3.2 从未上 Steam）；GitHub 侧上一产物是 prerelease `v0.3.2-pre1`。
- 发布唯一入口：[`release-runbook-zh.md`](./release-runbook-zh.md)（阶段 0–4、双轨、`merge -s ours` 条件、Claim Pack 模板在文末）。发布执行时新增 `docs/release_review/release-0.3.3-review-zh.md`。
- 建议发布会话第一步：读 runbook → `verify-local`（可加 `-PackDev` 顺带出 dev 包）→ 授权后按阶段推进。

## 4. 已定案（后续会话不要重开）

1. **Eat 默认保持 job 级派发**（整个 `JobDefOf.Ingest` job，含端食物赶路）＝招牌手感，默认不得收窄（0.4/US 同样适用）。
2. 父开关 `eatOnlyDuringChewing` → vanilla `IEatingDriver.GainingNutritionNow`（默认 `false`）。
3. 子开关 `eatIncludeDrugs`（UI「使用成瘾品」）→ vanilla `ChewIngestible` toil（默认 `false`；父关时禁用且强制 `false`）。
4. toil 名在本进程未确认时**回落完整 job 级**（fail-open，绝不静默）。
5. **版本 = 0.3.3**；0.3.2 正式版按"跳过"口径（其工作并入 0.3.3）。
6. **US 兼容修复归属 = US 侧**（U1 跨程序集让位检测）；SR 本窗口**零运行时改动**；顺序硬门 = `U1 → US 型 Ratkin 包/桥 → SR 1.0 内容化`。
7. **身份归属推荐**：packageId 跟内容走（pack-only 继承人就地继承 `coahuilite.squeakyratkin`；legacy 拿新 id 冻结）——推荐未裁决。

## 5. 开放裁决（按会话归类）

| 类别 | 项 | 位置 |
| --- | --- | --- |
| 发布 | 0.3.2 正式版是否确认跳过；Steam 是否恢复；0.3.3 走 GitHub full 还是 prerelease 先行 | 本文 §3、`TODO.md` 0.3.2 列表 |
| 迁移/身份 | Q1–Q10（过渡版、legacy 条目、设置导入、公告窗口、前置硬/软、跨版本维护、id 归属、音源重置、legacy 上架、新线 def 类型） | 迁移方案 §7/§8.5 |
| US 兼容 | Q1–Q4（US 是否服务 Ratkin；检测方式；桥重叠期；Domain 级跳过） | 兼容性检查 §6 |
| 实机 | Eat 三态矩阵；US 双开矩阵 | `TODO.md`、兼容性检查 §5 |

## 6. 分派给其他会话（可直接转述）

### 6.1 发布会话（SR 0.3.3 更新与发布）
- 读：`AGENTS.md` → `MEMORY.md` → `TODO.md` → 本文 → `docs/release-runbook-zh.md`。
- 做：`verify-local`（全绿）→ 授权后 commit（建议把功能块与文档块分开）→ 按 runbook 阶段推进 → 写 `docs/release_review/release-0.3.3-review-zh.md` → 渠道核验。
- 边界：**不要改语义/默认值**；Steam 涉及人工编辑；外部操作需授权；隐私审查覆盖完整可达范围。

### 6.2 US 会话（兼容改动，SR 侧不改行为）
- U1：`VoicePackCompAttach` 逃逸门改**跨程序集**检测（"该种族已有任意 squeak comp 即跳过"）+ skip 原因日志（如 `foreign_squeak_comp`）。
- U2：按 Q1 裁决落实"US 是否服务 Ratkin"；若服务，U1 是其**发布硬前置**。
- U3：legacy 桥（薄空 `SqueakyRatkin.SqueakVoicePackDef`）仅在 **SR 程序集不再加载**的窗口启用；重叠期谁拥有该类型名先约定。
- U4：双开矩阵（兼容性检查 §5）纳入 US 发布门。
- 验收信号：SR+US（±US 型 Ratkin 包 ±桥）四种组合下 Ratkin **每次事件恰好一条声音**；US 日志出现 Ratkin 跳过记录。

### 6.3 决策会话（迁移/身份）
- 只做裁决与排期：Q1–Q10；产出可写回迁移方案（把"待裁决"改为"已定案"）与 `TODO.md`。

## 7. 证据索引

- Eat 语义/vanilla 基线/按钮规格：[`handoff-eat-occurrence-granularity-zh.md`](./handoff-eat-occurrence-granularity-zh.md)
- 兼容性（F1–F8、U1–U4、矩阵）：[`us-sr-compatibility-check-zh.md`](./us-sr-compatibility-check-zh.md)
- 迁移/退役（B1–B5、P0–P4、§8 身份、Q1–Q10）：[`us-sr-migration-plan-zh.md`](./us-sr-migration-plan-zh.md)
- 关键代码：`Source/SqueakyRatkin/SqueakEatOccurrence.cs`（纯规则）、`Source/SqueakyRatkin/CompSqueaker.cs`（`IsEating`/`SampleChewingToil`）、`1.6/Patches/Ratkin_AddSqueakComp.xml`（SR 装配点）
- 校验：`scripts/verify-local.ps1`；设置夹具 `fixtures/expected/01-new-install-first-save.xml`（两个新字段默认 false ⇒ 零 delta）
- 耐久记录：`MEMORY.md`（Eat 两级 + US 兼容/迁移指针）、`TODO.md`（US 兼容硬前置、迁移裁决待办）

## 8. 隐私与授权边界

- 本轮未做任何 commit/push/tag/release/上传；工作树即全部交付物。
- 三份新文档与 MEMORY/TODO 均无本机绝对路径、凭据、`PublishedFileId`、日志摘录；US 仓库为只读参考（维护者授权路径），未做任何写入。
- 发布/推送/上架前按 `AGENTS.md` 做完整可达范围的隐私审查。

## 9. 下一会话开工前先确认的三件事

1. 是否 commit 本轮改动，以及拆分方式（建议：功能块一个提交、文档/记忆块一个提交）。
2. 0.3.2 正式版是否确认跳过（当前口径：跳过，工作并入 0.3.3）。
3. Steam 是否恢复发布；若不恢复，0.3.3 是否只走 GitHub。

## 10. 接续记录：流程简化落地 + 本地发布推进（2026-09-13 会话，停在远端推送前）

### 10.1 本轮做了什么

1. **流程冗余评估**（对齐 UniversalSqueaker 的三命令 + 最小仪式）：产出 [`process-redundancy-review-zh.md`](./release_review/process-redundancy-review-zh.md)——逐条裁决 R1–R12、实测证据、新发现、待裁决 V1–V3、明确不做 N1–N3。核心结论：门禁覆盖不缺，冗余在「谁来做」；同一包内容清单曾在 runbook 出现 3 次、隐私审查 4 处人工表述 0 脚本。
2. **简化落地**：新增 `scripts/privacy-audit.ps1`（三向量 + 身份 + `-FullHistory`/`-PrePush` + `$knownHistoryDebt` 台账）与 `scripts/check-pack-readiness.ps1`（版本轴/红线/暂存包/DLL 身份/`[claim]` 快照）；`docs/release-runbook-zh.md` 重写为「入口契约 + 最小发布仪式 + 阶段 0–4」；`AGENTS.md` 增「External-state boundaries」（本地 commit 允许、远端操作需授权、禁止重新加回人工仪式）；`ci.yml`/`release.yml` 接线（11 项门 + 隐私门；release 再加发布面门；SDK 对齐 10.0.x）；`scripts/codemap.md` 与 README 双语版本锚点同步。
3. **发布推进（本地）**：0.3.2 正式版跳过口径不变；0.3.3 工作树已提交（功能块 / 文档记忆块 / 流程块分离），dev 包已产出。

### 10.2 证据（可复跑）

- `pwsh scripts/verify-local.ps1 -NoRestore` → 11/11 全绿（EXIT 0）。
- `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata` → all checks passed。
- `pwsh scripts/privacy-audit.ps1 -FullHistory` → 227 revision 全扫，未接受命中 0；5 条 `[known-debt]`。
- 负向自测（隔离一次性仓库）→ 三向量 + 身份全部触发、EXIT 1。

### 10.3 推送会话入口（需授权后才动远端）

1. 读 `AGENTS.md` → `MEMORY.md` → `TODO.md` → 本文件 → `docs/release-runbook-zh.md`。
2. 推送前仪式（三步）：`check-pack-readiness -RequireReleaseMetadata` → `privacy-audit -FullHistory -PrePush` → 发布裁决（V1–V3 与 Steam 是否恢复）。
3. 授权后：push dev → PR dev→main → CI 绿 → squash merge → tag `v0.3.3` → release CI → 资产核验 → CHANGELOG 换时间 → Claim Pack `release-0.3.3-review-zh.md` → 渠道核验。

### 10.4 边界与未验证面

- 未做：push / PR / merge / tag / GitHub Release / Steam 上传或编辑。
- CI 改动要推送后才实际运行（本机无法实跑 runner）；YAML 只做人工校对。
- 历史/tag 重写未执行：门以 `[known-debt]` 列出 5 个文件（范围比先前记录更大，需按 5 文件重估）。
- Steam 文案/包核验随解除阻断执行，本轮未做。
