## 4. 假设账本（ASM-<n>）

| id | 假设（要旨） | 提出时间 | 是否仍成立 | 变更/失效点 | 验证或反证 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| ASM-1 | 阶段门证据质量是真实瓶颈（游戏内验证慢/抖/不可复现），不拆内核技术上也做得了 | 2026-08-18 | 成立 [I]（0.3.0 验收按语料+harness 完成） | — | DEC-7 三层证据实际执行 | src: 决策文档 §1 L10 |
| ASM-2 | 约 500 行内核 + 200 行 harness 足以完成抽取 | 2026-08-18 | 未知 [U]（实际行数不在语料，U-5） | — | 无实测行数 | src: 决策文档 §1 L10 |
| ASM-3 | 抽取的全部代价 = 内核纪律负担 + 一波机械 churn + 双份表示心智成本 | 2026-08-18 | 部分受挫 [I]：INC-1 显示纪律负担真实存在（全绿仍漏回归） | 2026-08-20 后置修复 | checklist 后置修复段 | src: 决策文档 §2 L30；检查表 L27 |
| ASM-4 | 独立程序集在 0.3.x 无消费者（独立版本/发布/类型隔离均无收益） | 2026-08-18 | 成立至 0.4.x（届时整体搬 US csproj） | 0.4.x 计划 | DEC-13 迁移验证 | src: 决策文档 §3.2 L141–L143 |
| ASM-5 | harness 链接编译足以强制零 Verse 纯度 | 2026-08-18 | 成立（无违规记录） | — | KernelCharacterization 链接编译通过（harness 表） | src: 决策文档 §3.2 L143；交接 0.3.0 §3 L34 |
| ASM-6 | 存档按枚举名序列化，namespace 不变即零 churn 已证安全 | 2026-08-18 | 成立 [I]（0.3.0 C 面字节一致佐证） | — | fixture 9 场景 load→save 字节稳定（EV-6 面 C） | src: 决策文档 §3.1 L123；检查表 L10 |
| ASM-7 | 等权 = 均匀、单次 roll 走累计权重、同 roll 同结果可测 | 2026-08-18 | 成立（语料自洽基线） | 分布等价≠逐次等价（DEC-7 诚实①） | 语料回放零 delta | src: 决策文档 §4.2 L231；等价评审 §4.1 |
| ASM-8 | 0.x 内除玩家数据与已发布面外可大胆改、改错成本=修正本身 | 2026-08-18 | 2026-08-22 起收窄：作者 XML 面提前公开稳定 | §1.1 修订 | DEC-3 | src: 决策文档 §1.1 L16–L24 |
| ASM-9 | 恶意 mod 无解（可直调 `SoundDef.PlayOneShot`）→ 诚实安全模型 | 2026-08-18 | 成立（共识条款沿用） | — | — | src: 决策文档 §2.2 L44 |
| ASM-10 | 第三方表达动作扩展需求可能存在也可能不出现（放弃信号①预置） | 2026-08-18 | 悬置 [U]（需求是否出现不在本包语料） | 放弃信号触发时 | OQ-6 | src: 决策文档 §2.2 L56 |
| ASM-11 | 0.3.0 注入面 = 旧选择面 → DomainPool 全排序无行为差异 | 2026-08-18/20 | 成立（0.3.0 范围内） | 0.3.1 多 race 注入后失效边界 | 等价评审 §4.2 声明 | src: 等价评审 §4 L51 |
| ASM-12 | `Rand.Range` 与 `floor(Next01()*N)` 分布等价（逐次不同可接受） | 2026-08-18/20 | 成立（作为玩家承诺「同池同分布」的基础） | — | 等价评审 §4.1 诚实声明 | src: 等价评审 §4 L50 |
| ASM-13 | G 面性能可由分配论证替代实测（dev 计数对比「可选」） | 2026-08-18/20 | 被接受但未按实测闭合 [C]（检查表如实标「绿（论证）」） | 若 0.3.0 发布后出现性能报告 | 检查表 G 行 + 实机记录 4 | src: 检查表 L14、L22 |
| ASM-14 | RimWorld 1.6 无 Toddler 原生对应 → 直映 life-stage、不按年龄阈值重算 | 2026-08-18 | 成立（0.3.1 计划沿此口径） | — | 决策文档 §3.1/§4.7 两处一致 | src: 决策文档 §3.1 L132、§4.7 L271 |
| ASM-15 | Verse `XmlToObjectUtils.SearchTypeHierarchy` 可填充继承字段（legacy 薄继承可行） | 2026-08-22 | 离线已证（真实 Verse API 编译期 + 桥源 harness）；真 Verse XML 加载待实机 [U] | OQ-8 实机 | `tools/LegacyBridgePrototype/`、`tools/LegacyBridgeHarness/` | src: 决策文档 §5 L385 |

## 5. 认识论分账（C / I / U）

- **参与者声明 C-<n>**：
  - C-1：维护者于 2026-08-20 确认路由核心公理与机械验证约束——文档标题级标注「维护者确认」，是语料内声明，无第二独立佐证文件。src: 决策文档 §2.3 L66 [F]（确认动作已被文档记载）
  - C-2：维护者 2026-08-23 声明实机覆盖身份门控关键场景（睡眠/非玩家 pawn/精神崩溃玩家 pawn 静默）并确认 egg 两态计数（true 5 条 / false 对照 11 条零红字）与 `pawn_faction`/`pawn_ctrl` 字段生效（player 10 / nonplayer 26 无 Select nonplayer 泄漏）——参与者回机声明，语料内无原始日志附件（本包按隐私规则亦不复制日志形态）。src: 决策文档 §5 L371 [C]
  - C-3：维护者对 B 面 6 条 `audio.dispatch.no_sound` 「未复现 → 按瞬态关闭」——关闭决定基于当事人未复现声明，非根因分析。src: 检查表 L9 [C]
  - C-4：维护者声明 A 面听感抽样通过（2026-08-20）。src: 检查表 L8 [C]
  - C-5：交接 0.3.0 自述进度「0.3.0 任务 15 项：13 done，2 blocked」——当时点的参与者状态声明，已被后续时间层取代。src: 交接 0.3.0 §4 L43 [C]
  - C-6：H 面 No-DLC 实机「由维护者独立验证成功（D1）」，且因当前会话日志已覆盖旧记录，检查表明示「以其独立验证为准」——明示的不可复核参与者声明。src: 检查表 L15、L21 [C]
  - C-7：等价评审声明「对照基准 = v0.2.4 tag，本分支与 tag 零差异已核验」——核验由文档作者（agent）执行，检查表 B/E 行的 diff 审计构成部分独立支撑。src: 等价评审 §1 L10 [C]+EV-14
- **本包推断 I-<n>**：
  - I-1：C11 缺号最可能对应 L31 中两个未编号提交（`5b51c6a` handoff、`67028b8` MEMORY/TODO 同步）之一被并入 C10 描述或编号跳过——推断链：L31 顺序枚举恰在 C10 与 C12 之间插入两个无 C 号条目；替代解释：C11 为另一未记录提交，或编号者故意弃用该号。src: 检查表 L31 [U]
  - I-2：静默回归根因 = 换链时 BuildFallback 路径未带内置表种子，且原语料/断言未覆盖 BuildFallback 失败面——推断链：修复文字「`BuildFallback` 恢复种子内置表（修静音回归）」+「KernelCharacterization 41→43 断言（失败路径 2 条）」说明修复同时补了原 41 断言未覆盖的路径；替代解释：回归源于更早的既有缺陷而非换链（但被归入「后置修复」条目下，时序指向换链）。src: 检查表 L27 [I]
  - I-3：交接 0.3.0 自身后来以 `5b51c6a` 提交并推送——推断链：交接 L4「本文件可能未提交」+ 检查表 L31「`5b51c6a` handoff」。src: 交接 0.3.0 L4；检查表 L31 [I]
  - I-4：决策文档为多时间层累写文档（非 2026-08-18 一次写成）——推断链：文内显式标注 08-18（§2/§5 小节）/08-20（§2.3/§2.4/发布决策）/08-21（重排）×3 处/08-22（修订）×6 处/08-23（SKILL/实机）×2 处；影响：任何「前段 vs 后段」差异先按时间层处理，不当笔误。src: 决策文档全篇节题 [I]
  - I-5：派发表「C1–C19」的后续链条目（若存在）最可能记载于 `docs/handoff-0.3.3-zh.md`（含 2026-09-13 接续记录，PKG-6 主源）——推断链：README §3 该行注记 + 本包三文档证据链止于 C14。src: prompts/docs-consolidation/README.md §3 #25（框架层，仅定位用）[I]
- **未决 / 缺失 U-<n>**：
  - U-1：彩蛋 YAGNI 修订前「运算符/条件表达式」方案的具体条文——缺前版草稿（语料只留「去掉运算符」标题级记录）；影响 DEC-6 演化完整性。src: 决策文档 §2.4 L81
  - U-2：热修「方案 a」的对照方案 b/c——语料未记；影响 DEC-9 备选重建。src: 决策文档 §5 L340
  - U-3：0.3.0 发布执行最终结果（merge 授权、渠道发布是否完成）——本包语料终点 = 2026-08-20「仍阻塞」；影响 DEC-8/DEC-9 结论现行性。src: 检查表 L3、L23
  - U-4：0.3.1/0.3.2 计划是否如期实施与过门——实施证据不在本包主源（疑在 PKG-6 主源）；影响 DEC-10/DEC-11 状态。src: 决策文档 §5 L350–L371
  - U-5：内核/harness 实际行数（对照 ASM-2 的「约 500/200」）。src: 决策文档 §1 L10
  - U-6：fixture「九场景」与语料编号 `S1-S5 + F03-F07`（5+5=10 个编号）的映射关系——语料未给出编号→场景对照；影响 A/C 面证据复算。src: 检查表 L8、L10；等价评审 §3 L40
  - U-7：身份门控实机矩阵全文——原文写「见 TODO」，TODO.md 在语料外。src: 决策文档 §5 L365

## 6. 矛盾与反转（CNF-<n>）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
| --- | --- | --- | --- | --- | --- |
| CNF-1 | 派发表要求「C1–C19 证据链」（本包调度消息/README §5 行）vs 冻结语料证据链止于 C14（`src: 检查表 L29–L31`） | 派发=2026-09-17；语料=2026-08-20 落表 | 派发口径含 C15–C19，四份主源无一条目 | I-5：后续链在 PKG-6 主源（handoff-0.3.3）；或派发表数字有误 | [xref: PKG-6（交接与开放裁决）] 是否出现 C15–C19；按源文档为准执行 |
| CNF-2 | 检查表 L31 链中 C1–C10、C12–C14 有号 vs **C11 全文无号**（同节两个未编号提交 `5b51c6a`/`67028b8`） | 2026-08-20 | 编号断档 | I-1（两解均存） | 后续会话/记忆文件中是否曾引用 C11。src: docs/0.3x-release-gate-checklist-zh.md §已锁定证据链 L29–L31 |
| CNF-3 | 语料例数「1622 例」（`src: 等价评审 §1 L12、§5 L56`，随 C4 提交口径）vs「3782 例」（`src: 检查表 L8`、交接 0.3.0 §3 L34/§4 L44，C7 后口径） | 等价评审=08-18~19（C5 时点）；检查表/交接 §3=08-19~20 | 同一文件 `fixtures/corpus/corpus-0.3.0.txt` 两个例数 | 非错误：C4 生成 1622 → C7 fixture 驱动扩至 3782；等价评审 §5 结论未随 C7 改写（文档停留旧层） | 以「C4=1622、C7=3782」双层并记；提炼时勿合并成单一数字 |
| CNF-4 | 交接 0.3.0 §6.2「定案后执行双轨发布（GitHub prerelease → Steam 正式）」vs 决策文档 §5 / 检查表 L23「**不设 GitHub prerelease tag**、本地 dev 包分发」 | 交接=2026-08-19（决策未定时预写）；修订=2026-08-20 同日修订 | 双轨载体是否含 GitHub prerelease | 交接收写「①GitHub prerelease 双轨是否采用」当时确未决；08-20 修订否定 | 发布执行事实以 [xref: PKG-4] 为准。src: docs/handoff-0.3.0-zh.md §6 L59；docs/0.3x-release-gate-checklist-zh.md L23 |
| CNF-5 | 动作门落地窗口：§2.2 主表「机制本体 **0.4.x US 拆分发布时**与消费者同窗口落地」vs L58 修订「机制本体改为在 **US 仓库 0.3.x 并行窗口**开发、0.4 随 US 首版发布」（`src: 决策文档 §2.2 L48–L58`） | 08-18 原文 vs 2026-08-22 修订 | 开发窗口不同（SR 0.4.x vs US 0.3.x），发布时点同为 0.4 | 修订明言「原表述按新路线理解」= 表体未改写、修订附注覆盖；属口径演化非错误 | 与 DEC-12/DEC-13 的 US 落地清单互查 |
| CNF-6 | IsEgg 暴露面：原口径「0.3.x 内为维护者内部标签、第三方作者指南 0.4.x 才开放」vs「自 0.3.2 起纳入公开作者 ABI」（`src: 决策文档 §2.4 L83–L87`） | 08-20 定案 vs 2026-08-22 修订 | 内部标签 vs 公开 ABI | 文档已明文「口径废止」——本条为**有记录的废止**，非未协调矛盾 | 合同现行文本对照 [xref: PKG-1（日志协议/设置合同）] |
| CNF-7 | 交接 0.3.0：L4「本文件**可能未提交**」+ L10「本地领先 `origin/0.3.x` 若干提交**未推送**」vs 检查表 L31「`origin/0.3.x` 已推送同步」「`5b51c6a` handoff」 | 08-19 vs 08-20 | 仓库同步状态相反 | I-3：交接写于推送前，08-20 完成提交+推送；属时间层差异非矛盾，但按规则并存记录 | (冻结动作) 编排方已核 docs 干净（2026-09-17）。src: docs/handoff-0.3.0-zh.md L4、L10；docs/0.3x-release-gate-checklist-zh.md L31 |
| CNF-8 | 热修路线：交接 0.3.0 §6.2「③ 0.3.0.**x** 热修路线确认（未决）」vs 决策文档 §5 L340 定案「`vX.Y.Z-hotfixN`，hotfix **不 bump z**」（`x.y.z` 三节不变） | 08-19 vs 2026-08-20 | 待定项的口头形态「0.3.0.x」（四段号）与最终定案（prerelease 后缀、不动四段号）表述不一致 | [I] 交接口语预写与定案格式差异；定案版为准（同 CNF-4 时间层逻辑） | 下游引用热修格式一律用 `vX.Y.Z-hotfixN`。src: docs/handoff-0.3.0-zh.md §6 L59；docs/0.3x-refactor-architecture-decision-zh.md §5 L340 |

## 7. 事故 / 失败 / 成功与后果（INC-<n> / EV-<n>）

| id | 类别 | 现象 | 根因（若知） | 处置与结果 | 证据强度 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| INC-1 | 事故（回归） | 0.3.0 换链后出现**静音回归**：BuildFallback 路径丢失内置表种子（检查表归入「后置修复」首项） | 语料未明写；[I] I-2：换链时 BuildFallback 未带种子 + 原 41 断言/语料未覆盖该失败面 | 后置修复（2026-08-20，已提交 C12 `0b0fa48`「错误路径修复」）：`BuildFallback` 恢复种子内置表；复验三 harness 全绿（语料 3782 零 delta）、双 flavor 0 warning/0 error、dev 包重打包通过 | 实测（复跑复验）+根因 [I] | src: 检查表 L27、L31 |
| INC-2 | 回归 | `CollectKnownSounds` 曾失去 `_Preview` 排除（与 v0.2.4 语义偏离） | 未明写（后置修复条目） | 恢复 `_Preview` 排除，检查表 E 面定性为**回归恢复**（对齐 v0.2.4 语义），计入 E 面「绿（审计）」 | 实测（diff 审计+修复复验） | src: 检查表 L12、L27 |
| INC-3 | 瞬态事件 | 实机 B 面日志出现 6 条 `audio.dispatch.no_sound`（每动作各 1 次，v1 既有事件） | 未知；维护者未复现 | 「按瞬态关闭」——无根因分析即关闭；本包如实保留（→OQ-7） | 传闻/当事人 [C]（C-3） | src: 检查表 L9 |
| INC-4 | 伴随卫生 | 后置修复同批还含：`KernelGate` 统一取 `ctx.Production`、删 `TotalWeight` 死计算、KernelCharacterization 断言 41→43（新增失败路径 2 条） | — | 已提交 C12；G 面备注「后置卫生再减一次权重求和」 | 实测 | src: 检查表 L14、L27 |
| EV-1 | 验证 | C4 `fb9ab50`：KernelCharacterization 41 断言 + 黄金语料 1622 例生成/回放零 delta | — | 语料第一层入库 | 机器复跑（文档记载） | src: 检查表 L31 |
| EV-2 | 验证 | C5 `ea5fd2c`：等价评审入库（14 行对照全等价 + 换面/不换面审计 + dev 豁免 + 诚实声明三条） | — | 成为 0.3.1 切核对照基线 | 论证+单测 | src: 等价评审 全篇；检查表 L31 |
| EV-3 | 验证 | C7 `7e2c3f5`：fixture 驱动语料扩至 3782 例（S1-S5 + F03-F07）零 delta | — | A 面主机器证据 | 机器复跑 | src: 检查表 L8、L31 |
| EV-4 | 实机成功 | A 面（2026-08-20）：dev 包 `SqueakyRatkin-dev-v0.3.0-67028b8-dirty.zip` 实装，听感抽样通过；日志核验 = 87 次 `audio.dispatch.ok` 覆盖 `SR_OfficialExample_Race_*`（Fallback/Remix tier）与 `SR_*`（Off 内置回退）两面；`mod.start.ready` count=37；`rebuild_failed`/`refresh_failed`/`PackRejected`/`TargetRejected`/`hook.*unavailable` 全零 → BuildFallback 全程未触发（注：与 INC-1 的关系——该次实机在修复后的包上） | — | A 绿 | 实测（当事人）+日志计数 | src: 检查表 L8、L19 |
| EV-5 | 实机 | B 面：触发节奏正常；`trigger.outcome.summary` 13 条（最大单窗 dispatched=52）、0 条 `trigger.attempt.failed`；6 条 no_sound → INC-3 | — | B 绿 | 实测+当事人 | src: 检查表 L9、L19 |
| EV-6 | 实机 | C 面：维护者手测四页/保存/footer/flush 正常、catalog 无刷新失败无 PackRejected；fixture 9 场景（含损坏修复）load→save 字节稳定；schema 未 bump（3/1）零迁移执行 | — | C 绿 | 实测+机器 | src: 检查表 L10、L20 |
| EV-7 | 审计 | D 面：日志文件未动（v0.2.4→HEAD 零差异）+ 双 flavor characterization；E 面：行为文件零改动（仅枚举移动） | — | D 绿、E 绿（审计） | 机器+diff 审计 | src: 检查表 L11、L12 |
| EV-8 | 机器验证 | F 面：`1.6/` 零文件改动；`About.xml` 仅 `<modVersion>` 0.2.4→0.3.0；`SR_*` 键源未动；stage-package Template↔built-in SHA256 镜像校验通过（dev 包 116 文件） | — | F 绿 | 机器 | src: 检查表 L13 |
| EV-9 | 实机 | H 面：No-DLC 编译基线主模组 Dev/Steam flavor 0 error（后置修复后 0 warning）；Biotech 门分支保留（语料 F07 dormant 锁定）；No-DLC 启动与触发维护者独立验证成功（D1）；HAR 缺失 = 硬依赖预期（报缺失不崩溃）；Harmony 引用未动 | — | H 绿 | 实测+当事人 [C]（D1 旧日志已被覆盖，C-6） | src: 检查表 L15、L21 |
| EV-10 | 验证（论证级） | G 面：代码级分配论证——旧 ChoosePack 每调用 `Where().ToList()` ×2 + LINQ 委托；新 SelectTier 手写循环 + 单 valid List + 仅 sound 级一个 playable List，分配更少；实机 dev 计数对比**未做（可选）**，接受论证 | — | G 绿（论证） | 论证（非实测） | src: 检查表 L14、L22 |
| EV-11 | 成功（源码核验） | BabyFits hook 对 RimWorld 1.6 源码一手确认（`TickInterval` 最终成功调用 `TryStartMentalState(..., transitionSilently: true)`）；0.3.2 `Selector.Select(playSound:false)` 全部调用点性质核验 → 定案「过滤」 | — | 两处窄 patch 设计锁定 | 一手源码核验（论证） | src: 决策文档 §2.1 L39、§5 L365 |
| EV-12 | 实机 | 0.3.2 彩蛋日志字段：2026-08-23 维护者实机确认 egg=true 5 条/egg=false 11 条零红字；最新 dev 包确认 `pawn_faction`/`pawn_ctrl`（player 10 / nonplayer 26，无 Select nonplayer 泄漏）（本包按隐私规则转述计数，不复制日志行） | — | 0.3.2 验证门两项过 | 当事人实测 [C]（C-2） | src: 决策文档 §5 L371 |
| EV-13 | 离线原型 | legacy 桥：`tools/LegacyBridgePrototype/`（真实 Verse API 编译期证明）+ `tools/LegacyBridgeHarness/`（运行时语义四项）完成（2026-08-22）；真 Verse XML 加载/交叉引用待实机 | — | 定案的技术可行性成立（离线） | 机器（离线） | src: 决策文档 §5 L385 |
| EV-14 | diff 审计 | v0.2.4→HEAD：行为代码「仅枚举移动」审计通过（B/E 面依据）；等价基准 = v0.2.4 tag 零差异核验 | — | 不换面承诺的证据 | diff 审计 | src: 检查表 L9、L12；等价评审 §1 L10 |
| EV-15 | 交付 | 测试包 `dist/dev/SqueakyRatkin-dev-v0.3.0-e6a1ed7.zip`（zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`；version.txt = 0.3.0/dev/e6a1ed7） | — | 2026-08-19 时点可分发产物 | 机器 | src: 交接 0.3.0 §4 L46 |
| EV-16 | 留档 | 链外状态：`origin/0.3.x` 已推送同步；`0.2.4-FINAL` 留档分支已推送（dev@`8bd383d`）；工作树干净（2026-08-20 检查表落表时点） | — | 提交面收口 | 文档记载 | src: 检查表 L31 |

## 8. 开放问题、阻塞与交接风险（OQ-<n>）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 发布执行两项授权未落：merge `0.3.x → dev` 授权 + 渠道发布授权（检查表落表时点仍阻塞） | 需授权 | 0.3.0 能否发布；DEC-9 双轨流程启动 | 本包语料内未解决（U-3）；[xref: PKG-4（发布执行事实）] | src: 检查表 L3、L23 |
| OQ-2 | US 仓库尚未创建（2026-08-22 明示「暂不建仓」，外部操作需另行授权） | 需授权/依赖外部 | DEC-12/DEC-13 全部 US 侧计划 | 未建仓（语料内）；[xref: PKG-3] | src: 决策文档 §5 L376 |
| OQ-3 | US Workshop 显示名与许可「待最终确认」 | 待裁决 | 双开命名面、外部渠道 | 未决 | src: 决策文档 §5 L376 |
| OQ-4 | US 在 SR 1.0.0 时点的版本号待定 | 待裁决 | 前置切换与依赖声明 | 未决 | src: 决策文档 §5 L384 |
| OQ-5 | 0.3.x/0.3.1/0.3.2 与 US 的发布顺序暂不决定（2026-08-22），ABI 用「首个携带 0.3.1 XML ABI 的发布版本」浮动表述 | 待裁决 | 冻结承诺的实际起算版本 | 未决 | src: 决策文档 §5 L369 |
| OQ-6 | 动作门三条放弃信号是持续监控条件（无需求→退回封闭；标准键诉求→转纯约定/封闭；语料非零 delta→撤销键迁移） | 依赖外部/未验证 | DEC-4 路线存活性 | 常设；0.3.x→0.4.x 窗口观察 | src: 决策文档 §2.2 L56 |
| OQ-7 | 6 条 `audio.dispatch.no_sound` 无根因，按「未复现」当事人声明关闭 | 未验证 | B 面绿灯成色；若发布后复现即成 A 面玩家可见缺陷 | 已关闭（本包质疑保留） | src: 检查表 L9 |
| OQ-8 | legacy 桥真 Verse XML 加载/交叉引用解析待维护者实机（US 程序集 + legacy XML 包 + 日志零红字） | 未验证 | SR 1.0.0 前置切换可行性 | 离线完成、实机未做 | src: 决策文档 §5 L385 |
| OQ-9 | 双开共存验证（打包门断言 US 0.4 无 Ratkin 条目 + 实机双开矩阵「同一种族只经一方单响」）尚未到执行窗口 | 未验证（未来门） | 0.4 共存策略成立性 | 计划 | src: 决策文档 §5 L382–L383 |
| OQ-10 | 身份门控实机矩阵余项由维护者判「自然覆盖」；全量矩阵在语料外 TODO.md | 未验证/[C] | 0.3.2 验证门完整性 | 判已覆盖（存疑保留） | src: 决策文档 §5 L365、L371 |
| OQ-11 | C15–C19（若存在）与 C11 缺号：证据链在本包语料外的延伸情况未知 | 待裁决（编排层面） | 提炼阶段合并证据链时需防断链 | 悬置（→GAP-3） | src: 检查表 L31；调度消息派发表 |

## 9. 锚点（ANCH-<n>：必须逐字保留）

- **标识符 / 字段名 / 键 / 前缀**：
  - ANCH-1 前缀与协议名：`SR_*`（SoundDef 键前缀，`SR_OfficialExample_Race_*` 实例）、`US_`、`srdiag`/`usdiag`（日志协议）、协议头 `fmt=2`、once key 前缀 `log-v2`、`coexistence=*_active`；repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker` 与 `coahuilite.squeakyratkin`、namespace `UniversalSqueaker.*`。src: 决策文档 §2.2 L50、§5 L376/L383；交接 0.3.0 §1 L9；检查表 L8
  - ANCH-2 内核类型：`RaceKey`/`XenotypeKey`/`AudioDomain`/`AudioDomainStatus{Available,Dormant,TargetUnavailable,Orphan}`/`AudioDomainStatuses.Classify`；`VoicePackEntry`/`ActionSoundSet`/`DomainPool`/`SqueakPoolRegistry`/`SelectionContext{Domain,ActionKey,Age,Production,AllowEggs}`/`SelectionMode{Off,Fallback,Remix}`/`ChainResult{SoundKey,Tier,PoolStableKey,IsEgg}`/`ChainTier{XenotypePack,RacePack,PackFallback,BuiltInFallback}`/`ISoundGate`/`IRollSource`；`ModulationAxis`/`Modulation.ComposeModulation`；`FallbackProfile`/`FallbackDelta`/`CopyDisposition{KeepCopy,RebuildFromSource,MergeDelta}`/`FallbackProfileOperations{DecideCopy,Merge}`/`BuiltInFallbackCatalog`/`BuiltInFallbackTable`；`DomainFilter`（0.4.x 删）/`DomainFilter.Everything`；`ActionKey.For(SqueakAction)`/`TryParseBuiltIn`；`Kernel/BuiltInActionKeys`（17 键唯一权威）。src: 决策文档 §3.1 L124–L128、§4.1 L149–L223
  - ANCH-3 适配层/外围名：`SqueakFallbackProfileStore.cs`、`SqueakLifeStageResolver.cs`、`SqueakRuntimeResolver`、`SqueakXenotypeCatalog`（类名保留）、`SqueakKernelAdapter`（唯一接缝）、`GetTargetCandidates`、`NotifyExternal`、`SqueakCompat.NotifyAction(Pawn, string actionKey)`、`SqueakActionDef`/`SqueakActionRegistry`、`SqueakGlobalActionPolicy`（零下游依赖）、`RecordOutcome`/`ExternalBlocked`、`allowExternalActions`（默认开）、`minIntervalTicks`≥60、`GetScope(string key)`、`SqueakSoundAvailabilityCache`、`SqueakTriggerInvocation`/`TriggerInvocationRules`/`SqueakActionPlan`、`PlayerSelection`/`ActiveCommand`/`IsPlayerInitiated`/`RequiresResponsivePawn`、`Pawn.IsPlayerControlled`、`!Downed && Awake()`、`Selector.Select(playSound:false)`（含 `__1` 位置注入过滤）、`MentalStateHandler.TryStartMentalState`（成功 postfix）、`MentalFitDef` 反向映射、`MentalState_BabyCry`/`MentalState_BabyGiggle`、`MentalBreakWorker.TryStart`（保持不动）、`LegacyVoicePackSource`、`Legacy/`、`SqueakVoicePackDef` 字段面 `raceDefName`/`scope`/`targetDefName`/`weight`/`fallbacks`/`actions(action,ageTag,IsEgg,sounds)`、`ageTag`（`AgeBucket?` field-presence）、`AgeBucket{Baby,Toddler,Child,Adult}`、`XmlToObjectUtils.SearchTypeHierarchy`、`Config 文件 SqueakyRatkin_Profile_<race>.xml`、`XenotypeMoodOverride` 模式、`migrationPersistencePending`、`SettingsOrigin{FreshCreated,LoadedFromFile}`。src: 决策文档 §2.2 L48–L62、§2.4、§3.1 L130–L135、§4.1、§5 L352–L385、§6 L393–L395、§6.3
  - ANCH-4 事件名（v1 28 事件集内的相关项 + v2 明细字段串）：`audio.dispatch.ok`、`audio.route.selected`、`trigger.outcome.summary`、`trigger.attempt.failed`、`audio.dispatch.no_sound`、`pack.race_defaulted`、`mod.start.identity`、`mod.start.ready`、`rebuild_failed`、`refresh_failed`、`PackRejected`、`TargetRejected`、`hook.*unavailable`、`devtools.camera_indicator.changed`；v2 明细字段序 `sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`；人类句 `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`；v2 记「开关状态/切换事件」。src: 检查表 L8–L20（转述计数不复制日志行）；决策文档 §5 L367、§2.2 L53
  - ANCH-5 工具/文件/fixture：`Source/SqueakyRatkin/Kernel/`（Domain.cs/Pool.cs/Modulation.cs/FallbackProfile.cs/DomainFilter.cs）、`tools/KernelCharacterization/`（Scenarios.cs **冻结不得改动**）、`tools/SqueakLogCharacterization/`、`tools/SettingsFixtureGenerator/`、`fixtures/corpus/corpus-0.3.0.txt`、`fixtures/input/`、`fixtures/expected/`、`tools/LegacyBridgePrototype/`、`tools/LegacyBridgeHarness/`、`dist/SqueakyRatkinEggTestVoices/`（gitignored）、`build-dev.ps1`、`release.yml`、`pack-github`/`pack-steam`/`stage-package`、`verify-local` 第 10 项 XML ABI 一致性锁、`.github/skills/squeaky-voicepack-authoring/SKILL.md`（唯一正文）、`scripts/new-voicepack.ps1`、`SqueakActionDomain.cs`/`SqueakVoicePackDomain.cs`。src: 决策文档 §5 L366；交接 0.3.0 §2 L27、§3 L31–L37；检查表 L27
- **数值 / 版本 / 阈值 / 计数**：
  - ANCH-6 ABI/协议：`SqueakAction` 17 值（序数 0–16；Crying=15、Giggling=16、0–14 不动；append-only）；内置 fallback 表 Ratkin 固定 15 键（Call…MentalBreak，不列 Crying/Giggling）；srdiag v1 = 28 事件/字段序/once key 冻结；`settingsSchemaVersion` 3→4、`voicePackSchemaVersion` 1→2（0.3.0 不 bump：现值 3/1）；五处同步 = 枚举/XML/SoundDef/本地化/统计；八道闸门链；四层链（XenotypePack→RacePack→PackFallback→BuiltInFallback）；`ChainResult`→诊断折叠 PackFallback→RacePack、BuiltInFallback→Vanilla；4 处 `Ratkin` 字面量临时层；UI partial ×3；整文件删除 0 个、符号内嵌删除约 150 行；`SqueakActionModel.cs` Count 15→17。src: 决策文档 §2.2 L62、§3.1 L135–L137、§4.6–§4.7、§5 L352、L358
  - ANCH-7 证据数字：语料 1622 例（C4）→ 3782 例（C7）；断言 41→43；fixture 9 场景（编号面 `S1-S5` + `F03-F07`，映射 U-6）；14 行对照；换面 4 项/不换面 9+1 组/dev 豁免 3 名 + 七次点击解锁面；等价验证「唯一豁免 = dev 隐藏功能」；观察窗 2 周起；350 ms ModSettings 队列；dev 包 116 文件；实机计数 87 派发、count=37、summary 13、dispatched 峰值 52、no_sound 6、egg 5/11、player 10/nonplayer 26；约 500 行内核 + 约 200 行 harness；约 +1500/−600（方案 C diff）。src: 等价评审 §2–§3；检查表 L8–L27；决策文档 §1 L10、§2 L32
  - ANCH-8 提交与版本标识（逐字）：C1 `41ca953` fixture 9 场景；C2 `efc08a0` 枚举提取+ActionKey；C3 `e48d38f` Kernel 骨架；C4 `fb9ab50` 41 断言+语料 1622；C5 `ea5fd2c` 等价评审；C6 `0893cd0` resolver 接内核+旧路径删除；C7 `7e2c3f5` 语料 3782；C8 `5c86315` 检查表（离线面绿）；C9 `1b42a04` 版本 bump 0.3.0；C10 `e6a1ed7` zip 打包脚本化；（未编号）`5b51c6a` handoff、`67028b8` MEMORY/TODO 同步（八面全绿）；C12 `0b0fa48` 错误路径修复；C13 `402d4be` Steam 打包纪律；C14 `2e5fc74` 决策文档/检查表全绿；留档 dev@`8bd383d`（`0.2.4-FINAL` 分支）；dev 包串 `SqueakyRatkin-dev-v0.3.0-67028b8-dirty.zip`、构建标识 0.3.0+`67028b88`…（截断形态照源文）；zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`；版本线 0.2.4→0.3.0→0.3.1→0.3.2→0.3.3（PKG-6 管辖）→0.4→1.0.0；热修形态 `vX.Y.Z-hotfixN`。src: 检查表 L19、L27、L31；交接 0.3.0 §4 L44–L46；决策文档 §5 L340、L384
- **授权边界与外部状态边界**：
  - ANCH-9 授权边界：提交/推送/发布需显式授权；推送前隐私审查完整可达范围；US 仓库创建 = 外部操作需另行授权；`0.2.4-FINAL` 留档分支；Kiiro 彩蛋内容「实验分支不发布、未经许可不宣传」；措辞维护边界「不提 US 名称/多种族/Kiiro/通用化」；Workshop 公告义务（旧「预计 0.3.0 失效」随正式发布同步替换、中英双预览+字符数重算）。src: 交接 0.3.0 §7 L68；决策文档 §5 L342、L348、L376；§2.4 L87
  - ANCH-10 外部状态边界：Steam Workshop 自动更新 = 发布即全员无灰度；RimWorld 无商店→无人工审核（治理 = 技术校验+玩家代理+文档礼仪）；HAR 反射非依赖（缺失静默降级）；No-DLC 基线；Biotech 门 `ModsConfig.BiotechActive`；Harmony 版本面；ProductDomainFilter 试验名单 `{Ratkin, Kiiro, Miho}`（默认 `{Ratkin}`、隐藏开关、替换非叠加、UI 不渲染、release 默认 off、0.4.x 整类删除）；卸载安全 = 存档零写入；`base.WriteSettings()` 单通道红线只约束 ModSettings；GitHub CI 只为 GitHub 包服务、dev/steam 打包为本地纪律；`0.3.x` 分支自 dev 切出隔离 0.2 线。src: 决策文档 §5 L325、§2.2 L44、§4.4 L256、§4.6 L265、§5 L338；交接 0.3.0 §1 L10、§7 L65
- **术语口径（逐字）**：「定型」= 0.x 内部冻结（0.4.x US 发布前可修正，代价 = 内部返工）；「冻结」= 已发布契约（修正须版本化兼容）；ABI/兼容面 = 五类持久化/共享格式（①Scribe 设置格式 ②srdiag 日志协议 ③VoicePack XML 包 schema ④枚举序列化值〔`SqueakAction` 序数〕⑤Def 键〔`SR_*` SoundDef 与 PackKey〕）。src: 决策文档 §1 L12

## 10. 候选教训（LES-<n>）

| id | 教训 | 支撑证据 | 强度 | 适用边界 | 来源 |
| --- | --- | --- | --- | --- | --- |
| LES-1 | 「全部门槛绿」不等于「无回归」：换链阶段门通过后同日仍需后置修复（静音回归 + `_Preview` 回归 + 断言 41→43）；失败/回退路径必须与主链路同期配断言 | INC-1/INC-2/INC-4（检查表「后置修复」+ 题注时序：八面绿与修复同属 2026-08-20） | 强（同语料直接事件） | 实现替换型变更（行为保持重构）的验收设计 | src: 检查表 L3、L25–L27 |
| LES-2 | 等价验收必须把「分布等价 ≠ 逐次对拍」写成诚实声明并给出玩家承诺口径（同池同分布），否则下游会把语料零 delta 误读为字节级对拍 | DEC-7 三层证据设计 + 等价评审 §4.1「若需逐项对拍须双实现（方案 D，已否）」 | 强 | 任何以随机源为输入的重构验收 | src: 等价评审 §4 L50；决策文档 §5 L313 |
| LES-3 | 新机制先取最小面：彩蛋裁决「同日 YAGNI 修订去掉运算符」；动作门「0.3.x 只做两件零成本事」并把注册机制判为 YAGNI 违反——无消费者需求时机制本体后置到真实窗口 | §2.4 修订标题、§2.2 落地窗口行 | 中等（同语料两例同向） | 面向未来生态的机制预建决策 | src: 决策文档 §2.2 L55、§2.4 L81–L83 |
| LES-4 | 门槛状态按证据强度分级标注（绿 / 绿（审计）/ 绿（论证）），未做的可选实测显式写「未做（可选）接受论证」，避免绿灯语义膨胀 | 检查表 E=绿（审计）、G=绿（论证）+ 实机记录第 4 条 | 中等 | 门槛类检查表的记法设计 | src: 检查表 L12、L14、L22 |
| LES-5 | 交接文档自带失效声明（写「最后更新日/可能未提交/待办清单」）仍会在一天内被实机与修订超过——冷启动必读序应把「门槛/状态类文档」置于「叙述性交接」之前 | 交接 0.3.0 L4、§4、§6 vs 检查表 L3（08-19→08-20 即过期） | tentative（n=1 文档对） | 跨会话 handoff 体裁；不适用于合同层 | src: 交接 0.3.0 L4–L5；检查表 L3 |
| LES-6 | 落选方案的「被吸收硬细节」单列是决策账本的高价值资产：迁移事务性、UI assembled-only 投影、hook 源码核验、黄金语料升级四项全部来自落选 B/D | §2.1 四条吸收项均落成 0.3.1 硬条款 | 中等 | 多方案比拼记录体裁 | src: 决策文档 §2.1 L37–L40 |

## 11. 证据缺口（GAP-<n>）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | 四份并行架构草稿各自全文与原始论据？ | 决策文档 §2 只留比拼表（核心思想/强项/弱点/裁决），草稿本体不在 `docs/**` | 草稿存档或撰写会话记录（语料外）。src: docs/0.3x-refactor-architecture-decision-zh.md §文首 L4、§2 L26–L33 |
| GAP-2 | 0.3.1/0.3.2 是否按计划实施、各验证门实际过门记录？ | 本包主源止于计划文本（决策文档 §5）与 0.3.0 门槛（检查表）；实施记录在后续 handoff | [xref: PKG-6（handoff-0.3.3 含 2026-09-13 接续）] + 发布评审 [xref: PKG-4]。src: 决策文档 §5 L350–L371；docs/0.3x-release-gate-checklist-zh.md L3 |
| GAP-3 | C15–C19 是否存在、内容为何？C11 是并号还是弃号？ | 冻结语料证据链止于 C14（含两个未编号提交）。src: docs/0.3x-release-gate-checklist-zh.md §已锁定证据链 L29–L31 | 后续会话文档（PKG-6）或仓库 log（本管道禁止外部取证） |
| GAP-4 | 「fixture 9 场景」与 `S1-S5`+`F03-F07` 编号体系如何对齐（含损坏修复场景编号）？ | 语料只分别给出「9 场景」枚举与编号引用，无对照表。src: 决策文档 §6.2 L400；docs/0.3x-release-gate-checklist-zh.md L8、L10；docs/0.3x-equivalence-review-zh.md §3 L40 | 语料文件头注释或 Scenarios.cs 文本（代码面在语料外） |
| GAP-5 | 静默回归 INC-1 的确切引入提交与暴露面（哪些玩家路径会静音）？ | 检查表只记修复不记引入点。src: docs/0.3x-release-gate-checklist-zh.md L25–L27 | `git log` 取证（本包禁止）或后续事故复盘文档 |

