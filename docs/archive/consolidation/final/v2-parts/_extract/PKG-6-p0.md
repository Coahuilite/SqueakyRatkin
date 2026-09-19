## 4. 假设账本（ASM-1…ASM-8）

| id | 假设（要旨） | 提出时间 | 是否仍成立 | 变更/失效点 | 验证或反证 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| ASM-1 | 玩家喜欢 job 级连珠炮手感（"太期待吃饭了"为正面反馈）⇒ 默认即招牌行为，不得收窄 | 2026-08-23（反馈日期不可得 [U]） | 成立（定案口径无重开）[F] | 无 | 定案 §4.1；验收矩阵要求"父关与旧实现完全一致" | `src: docs/handoff-eat-occurrence-granularity-zh.md §1 L18/L23; docs/handoff-0.3.3-zh.md §4 L46` |
| ASM-2 | Ludeon 不改 `ChewIngestible` debugName（方案 B 字符串依赖的软假设） | 2026-08-23 | 软性成立：真值层不可证，但已被 fail-open 机制风险中和 [F 设计] | 改名 ⇒ 子开关退化为无操作（非静默） | 进程级 `chewToilNameConfirmed` + 单测锁常量；静默退化仅 Dev 面板可见（未实施 → OQ-22） | `src: docs/handoff-eat-occurrence-granularity-zh.md §2.3 L52, §5 L115, §5.1 L181–185` |
| ASM-3 | `GainingNutritionNow` 语义是"营养"（`CachedNutrition > 0`），不是"在嚼"；零营养摄入物永不触发 | 2026-08-23 | 成立 [F]（1.6 源码核对） | 若上游改接口语义则失效 | §2.3 接口/实现核对 + §2.4 真值表 | `src: docs/handoff-eat-occurrence-granularity-zh.md §2.3 L49–58, §2.4 L64–75` |
| ASM-4 | public 面足够且零成本：`Pawn.jobs.curDriver` public 字段、`CurToilString` public 属性返回既有 debugName（无分配）、Ordinal 比较无分配 | 2026-08-23 | 成立 [F] | — | 成本说明 + 现行代码 §7 节选 | `src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L179, §7 L233–247` |
| ASM-5 | 两新字段默认 false + add-only ⇒ 不写节点、不 bump schema ⇒ settings fixture 零 delta | 2026-08-23 | 成立 [F 已验证] | "父关子真"若被写入将破坏 add-only 语义 → 三层强制归一堵死 | `fixtures/expected/01-new-install-first-save.xml` 零 delta 已验证 | `src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L195–198, §6 L218, §7.1 L257; docs/handoff-0.3.3-zh.md §7 L86` |
| ASM-6 | 新接线的 CI（11 项门+隐私门+发布面门，SDK 10.0.x）推送后会按预期运行 | 2026-09-13 | 未验证 [U]（本机无法实跑 runner；YAML 只做人工校对） | 首次远端 CI 运行 | 无反证；语料明载这是未验证面 | `src: docs/handoff-0.3.3-zh.md §10.1 L106, §10.4 L125` |
| ASM-7 | US 仓库为只读参考（维护者授权路径），SR 交接窗口内对其零写入 | 2026-08-23 | 成立（自述 [C]；语料内无反证） | — | §8 隐私与授权边界声明 | `src: docs/handoff-0.3.3-zh.md §8 L92` |
| ASM-8 | 负向自测在"隔离一次性仓库"执行 ⇒ 真实仓库不受写脏风险 | 2026-09-13 | 成立（自述 [C]；证据=三向量+身份全部触发、EXIT 1） | — | `src: docs/handoff-0.3.3-zh.md §10.2 L114`（"写脏真实仓库"事故叙事在 PKG-5 主源，此处只存指针） | `src: docs/handoff-0.3.3-zh.md §10.2 L114` |

## 5. 认识论分账（C / I / U）

**参与者声明 C**

- C-1 玩家（Steam 评论者，身份/日期不可得 [U]）：鼠族端食物横穿地图时"连珠炮"叫、"太期待吃饭了"——正面定性。被独立证据支持？部分：机制层由 `v0.3.0` tag 指纹 [F] 佐证。`src: docs/handoff-eat-occurrence-granularity-zh.md §1 L18`
- C-2 2026-08-23 交接会话（自述）：本轮无 commit/push/tag/release/上传；新文档与 MEMORY/TODO 无本机绝对路径/凭据/`PublishedFileId`/日志摘录。未被本包独立复核（隐私声明类自述）。`src: docs/handoff-0.3.3-zh.md §8 L89–93`
- C-3 fresh-context 对抗复核方（2026-08-23，早期时点）：判"vanilla 无公开 toil 权威"。被同文档后续核实**推翻**（`CurToilString` public ⇒ 方案 B 可行并已落地）。这是"复核结论=快照"的直接实例。`src: docs/handoff-eat-occurrence-granularity-zh.md 注1 L279`
- C-4 2026-09-13 接续会话（自述可复跑）：`verify-local -NoRestore` 11/11 全绿 EXIT 0；`check-pack-readiness -RequireReleaseMetadata` all checks passed；`privacy-audit -FullHistory` 227 revision 全扫未接受命中 0、5 条 `[known-debt]`；负向自测三向量+身份全触发 EXIT 1。证据强度=实测（声称可复跑，本包未复跑）。`src: docs/handoff-0.3.3-zh.md §10.2 L109–114`
- C-5 实现会话（2026-08-23）：「本项目 UI 复核专门查过（Measure/Draw 一致性）这一点」——用作 UI 高度常量化要求的依据；复核记录本体不在本包主源。`src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L192`
- C-6 2026-09-13 会话对冗余评估的核心结论（"门禁覆盖不缺，冗余在『谁来做』"；runbook 清单 3 次、隐私 4 处人工表述 0 脚本）[C]；论证细节在 PKG-5 主源。`src: docs/handoff-0.3.3-zh.md §10.1 L105`

**本包推断 I**

- I-1：§4.5 的"跳过"定案与 §5/§9.2 的"是否确认跳过"开放并存不矛盾——解释 [I]：前者是工作口径（default-of-record），后者是发布执行会话的正式确认门；09-13"口径不变"佐证两态并存。替代解释：文档撰写时未对齐（语料无裁定）。`src: docs/handoff-0.3.3-zh.md §4 L50, §5 L58, §9 L98, §10.1 L107`
- I-2：eat 文档 §7 前言 L223（两级"尚待实施"）是成文顺序上的较早层，晚于其后的 §7.1（"已实施"）与注 2；两段时间标签同为 2026-08-23，无法用日期裁决先后。替代解释 [I]：§7 为姊妹项目写的可移植参考节未随实施更新。`src: docs/handoff-eat-occurrence-granularity-zh.md §7 L223, §7.1 L249–262, 注2 L280`
- I-3：§9.1 的 commit 拆分建议（两块）被 09-13 会话吸收并扩展为三块（新增"流程块"，承载流程简化产物）——推断链：§9.1 建议 + §10.1 三块清单 + 三块恰与本轮三类改动（功能/文档记忆/流程脚本 CI）对应。`src: docs/handoff-0.3.3-zh.md §9 L97, §10.1 L105–107`
- I-4：§8"三份新文档"与 §2 表格 4 个 untracked 新文档的差异，解释 [I]：计数排除了交接文档自身（本文），指随本文一并交付的另外三份专项文档。替代解释：笔误。`src: docs/handoff-0.3.3-zh.md §2 L20/L31, §8 L92`

**未决 / 缺失 U**

- U-1：Eat 三态手工验收矩阵与 US 双开矩阵是否有任何一次实机执行——缺实机会话记录（语料外，`TODO.md` 记忆面不在本次覆盖）；影响 DEC-2/DEC-7 的最终验收。`src: docs/handoff-0.3.3-zh.md §5 L61`
- U-2：0.3.2 正式版"确认跳过"的终局裁决与理由——缺发布会话裁决记录；影响 DEC-6/版本序列 [xref: PKG-4]。`src: … §5 L58, §9 L98`
- U-3：V1–V3 待裁决项内容本体——在主源仅存编号；缺 PKG-5 主源展开；影响 DEC-11 推送仪式第三步。`src: … §10.1 L105, §10.3 L119`
- U-4：隐私债务"先前记录"的具体范围（"5 文件比先前更大"的先前值）——主源未载先前口径；影响 CNF-5 定性。`src: … §10.4 L126`
- U-5：Steam 评论的原文/链接/时间——仅转述；影响 C-1 的证据强度。`src: docs/handoff-eat-occurrence-granularity-zh.md §1 L18`
- U-6：v0.3.0 时代该手感是否有过负面反馈——语料只收正面反馈单侧证据；若存在负面侧证会动摇 DEC-1 的"招牌"定性。`src: docs/handoff-eat-occurrence-granularity-zh.md §1 L18–23`
- U-7：三块提交的具体 commit hash / 日期——§10 仅声称；可用 `git log` 恢复但本包禁外部取证（规则 9）。`src: docs/handoff-0.3.3-zh.md §10.1 L107`

## 6. 矛盾与反转（CNF-1…CNF-6）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
| --- | --- | --- | --- | --- | --- |
| CNF-1 | `handoff-0.3.3-zh.md` 头 L4"工作树未提交、未推送"+ §2 L19"本轮 0 提交"+ §2 L34 / §8 L91"未做 commit" ↔ §10.1 L107"0.3.3 工作树已提交（功能块/文档记忆块/流程块分离）" | 前段 2026-08-23；§10 2026-09-13（同文档分层写作，README §4 的示例原型） | 提交状态相反 | §10 是后补时间层，只更新不抹除；前段忠实记录其写作时点 [I] | `git log` 找三块提交（下游可用；本包禁取证）；现行建议口径：以 09-13 层为准 = 本地已提交、远端未推 |
| CNF-2 | eat 文档 §7 前言 L223"§7 描述的是已实施的单开关（方案 A）；父+子两级形态见 §5.1（尚待实施）" ↔ §0.4 L14"已落地的形态…本仓库已实施"、§7.1 L249–262 落地全表、注 2 L280"两级形态已在本仓库实施（2026-08-23）" | 同为 2026-08-23（日内先后不可证 [I-2]） | 两级形态"尚待实施" vs "已实施" | L223 为较早草稿层残留；§7 主体仍是单开关可移植参考、§7.1 才是两级落地 [I] | 下游若引用"实施状态"，以 §7.1 + 注 2 + §4 已定案 2–4 条为准 |
| CNF-3 | 注 1 L279：复核方当时判"vanilla 无公开 toil 权威" ↔ §2.3 L52 记载 `CurToilIndex`/`CurToilString` 为 public（`CurToil` protected） | 同日两层：复核早期 ↔ 后续核实 | B 方案可行性结论反转 | API 面核实滞后 [I]；属"结论被新证据取代"而非文档笔误 | 该更正同时是方案 B 立项前提（DEC-2 ALT-2）；引用时标注反转链 C-3→F |
| CNF-4 | §4.5 L50"版本 = 0.3.3；0.3.2 正式版按'跳过'口径（其工作并入 0.3.3）"（已定案，"不要重开"） ↔ §5 L58"0.3.2 正式版是否确认跳过"列为开放 + §9.2 L98"是否确认跳过（当前口径：跳过）" | 均 2026-08-23；§10.1 L107（09-13）"跳过口径不变"重确认 | "已定案"与"开放裁决表"同时收录同一事项 | 定案=工作口径、开放=终局确认门，两态并存 [I-1] | 发布会话的正式裁决记录（语料外）；下游勿把"口径"当"裁决"引用 |
| CNF-5 | §10.4 L126"门以 `[known-debt]` 列出 5 个文件（范围比先前记录更大，需按 5 文件重估）" ↔ "先前记录"口径数值主源未载（U-4） | 2026-09-13 | 债务范围版本间漂移，本包内无法定量 | 前次评估遗漏文件 [I] | 回查 [xref: PKG-5（隐私债务 5 文件 / 漂移记录）]；两包数字应互为校验 |
| CNF-6 | §8 L92"三份新文档均无……" ↔ §2 L31 untracked 列表含 4 个新文档（本文 + eat + compatibility + migration） | 均 2026-08-23 | 计数 3 vs 4 | "三份"排除交接文档自身 [I-4]；非实质矛盾，防下游误报 | 对照 §1 产出 B/C/D 恰为三份新专项文档 + 本文 |

（无跨源实质冲突：eat 文档与 handoff-0.3.3 在 Eat 已定案 1–4 条上逐条一致 [F]；`src: docs/handoff-0.3.3-zh.md §4 L46–49 ↔ docs/handoff-eat-occurrence-granularity-zh.md §5.1 L123–129`。）

## 7. 事故 / 失败 / 成功与后果（INC / EV）

| id | 类别 | 现象 | 根因（若知） | 处置与结果 | 证据强度 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| INC-1 | 设计陷阱（已被规格吸收的"预演事故"） | 静默 hole：开关一开，抽烟/吸薄片的 pawn 整个 ingest 期间零 `Eat` 事件；无异常、无 warn/error、无 rejected/cooldown 日志（走路时可能出 `Move`，站定时落 `Call`）——`Eat` 在采样层被换掉了 | 营养权威语义 = 营养 > 0（ASM-3），零营养 ingestible 永不满足；动作统计只记录进入管线的动作，计数直接归零看不出被谁替代 | 进规格：文案三要素必含边界（DEC-4 条 6）、方案 B 子档、fail-open 回落（DEC-3）；复现步骤文档化（父开→按药物政策抽烟→观察） | 论证（源码/Def 核对 + 复现步骤成文；未见实机执行记录 [U-1]） | `src: docs/handoff-eat-occurrence-granularity-zh.md §0 L13, §3 L79–94` |
| INC-2 | 过程遗漏（已补） | 文档同步面清单"含一个曾漏掉的"：**UI codemap** 曾被漏 | 未载（语料只标"曾漏掉"） | 补入同步清单并实施（§7.1 文档同步面行含 **UI codemap**，已实施） | 记载于落地表 | `src: docs/handoff-eat-occurrence-granularity-zh.md §6 L219, §7.1 L260` |
| INC-3 | 复核误判→更正 | 复核方判"vanilla 无公开 toil 权威"，若成立则只能 A（静默 hole 无解） | 当时未核到 `CurToilString` public | 更正后方案 B 立项并落地两级形态（→ CNF-3） | 同文档记载 | `src: docs/handoff-eat-occurrence-granularity-zh.md 注1 L279` |
| INC-4 | 未决风险（非事故） | 隐私历史重写未执行；`privacy-audit -FullHistory` 门以 `[known-debt]` 列 5 个文件，范围比先前记录更大 | 债务范围漂移（CNF-5） | 挂账 `$knownHistoryDebt` 台账，重写需另行授权 | 实测（C-4） | `src: docs/handoff-0.3.3-zh.md §10.2 L113, §10.4 L126` |
| EV-1 | 验证（行为一致性） | 默认双关 ⇒ 新 fixture（两个新字段默认 false）零 delta ⇒ 不 bump schema | — | 回归门：默认值不产生新存档节点 | 实测（`src: docs/handoff-0.3.3-zh.md §7 L86; docs/handoff-eat-occurrence-granularity-zh.md §6 L218, §7.1 L257`） |
| EV-2 | 成功（构建） | 主模组构建 0 warning / 0 error（Dev + Steam 双 flavor）；2026-08-23 工作树 19 文件改动 +161/−17 + 4 untracked | — | verify-local 11/11 全绿；最后运行在全部代码/版本写入之后、只改文档前 | 实测（自述，可复跑） | `src: docs/handoff-0.3.3-zh.md §2 L20/L33` |
| EV-3 | 成功（可复跑证据，09-13） | 四项：verify-local 11/11 EXIT 0；check-pack-readiness `-RequireReleaseMetadata` all checks passed；privacy-audit `-FullHistory` 227 revision 未接受命中 0 + 5 `[known-debt]`；负向自测（隔离一次性仓库）三向量+身份全触发 EXIT 1 | — | 支撑 DEC-10/11 | 实测（C-4） | `src: docs/handoff-0.3.3-zh.md §10.2 L109–114` |
| EV-4 | 成功（本地发布推进） | 0.3.3 工作树已提交（功能/文档记忆/流程三块分离）；dev 包已产出 | — | 远端未动（TL-9） | 传闻→实测之间：[C] 声称 + 工具可复跑（本包未复跑） | `src: docs/handoff-0.3.3-zh.md §10.1 L107` |
| EV-5 | 成功（实现落地） | §7.1 全表 8 行"已实施"：纯规则文件（纯度门不变）、`CompSqueaker` 静态旗标+进程级 confirmed、设置字段两处发布+Toggle(disabledReason)+父关清零+高度 +34f、Scribe 一行+PostLoadInit 归一、fixture 镜像（零 delta 已验证）、单测 `EatOccurrenceRules` 扩为模式+回落+常量+默认值断言、本地化 542 键零重复中英对齐、文档同步面 | — | 两级形态交付 | 记载+可复跑 | `src: docs/handoff-eat-occurrence-granularity-zh.md §7.1 L251–260` |
| EV-6 | 验证（出厂手感指纹） | `v0.3.0` tag 与写作时工作树机制一致（`IsEating`=job 级、`EachTime 144 t`、216 t 节拍、3.6 秒一发）⇒ "现网 Steam 版本就是这个手感" | — | 支撑 DEC-1 默认不变论证 | 论证（指纹比对陈述） | `src: docs/handoff-eat-occurrence-granularity-zh.md §1 L19–22` |
| EV-7 | 成功（范围界定） | 带营养成瘾品（啤酒 0.08 / 仙馔 0.2）在父开子关时仍响——被本仓库**明确接受**为默认行为（营养权威口径）；勾子项不改变它们 | — | 记录为对接收方的开放问题（OQ-17）与 IsDrug 备选（OQ-23） | 论证（源码数值核对） | `src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L203, §8.3 L268` |

## 8. 开放问题、阻塞与交接风险（OQ-1…OQ-23）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 0.3.2 正式版是否**确认**跳过（口径=跳过，工作并入 0.3.3） | 待裁决 | 版本序列/CHANGELOG/渠道 | 开放（09-13 仍"口径不变"） | `src: docs/handoff-0.3.3-zh.md §5 L58, §9 L98, §10.1 L107` |
| OQ-2 | Steam 是否恢复发布（0.3.2 从未上 Steam，阻断中） | 待裁决+需授权（人工编辑面） | 外部渠道 | 开放 | `src: … §3 L40, §9 L99, §10.4 L127` |
| OQ-3 | 0.3.3 走 GitHub full 还是 prerelease 先行（上一产物 `v0.3.2-pre1`） | 待裁决 | 发布节奏 | 开放 | `src: … §5 L58` |
| OQ-4 | 发布裁决 V1–V3（内容 [xref: PKG-5]）与推送前仪式绑定 | 待裁决 | 推送链路 | 开放；主源仅存编号 [U-3] | `src: … §10.1 L105, §10.3 L119` |
| OQ-5 | 完整推送链执行：push dev → PR dev→main → CI 绿 → squash merge → tag `v0.3.3` → release CI → 资产核验 → CHANGELOG 换 UTC+8 时间 → Claim Pack `release-0.3.3-review-zh.md` → 渠道核验 | 需授权 | 已发布版本面 | 停在远端推送前（冻结时点） | `src: … §10.3 L120` |
| OQ-6 | 迁移/身份 Q1–Q10 逐项：过渡版、legacy 条目、设置导入、公告窗口、前置硬/软、跨版本维护、id 归属、音源重置、legacy 上架、新线 def 类型 | 待裁决 | SR→US 路线 | 开放；分派决策会话（§6.3）；条文 [xref: PKG-3] | `src: … §5 L59` |
| OQ-7 | 身份归属推荐（packageId 跟内容走：继承人就地继承 `coahuilite.squeakyratkin`、legacy 拿新 id 冻结）裁决 | 待裁决 | 外部渠道身份 | 推荐未裁决 | `src: … §4 L52` |
| OQ-8 | US 兼容 Q1–Q4：US 是否服务 Ratkin；检测方式；桥重叠期类型名归属；Domain 级跳过 | 待裁决 | U1–U4 前置（Q1 决定 U2） | 开放；条文 [xref: PKG-3] | `src: … §5 L60` |
| OQ-9 | US 侧 U1–U4 实施进度（SR 只读窗口内零运行时改动） | 依赖外部 | 双开兼容 | 未知（GAP-6） | `src: … §4 L51, §6.2 L70–75` |
| OQ-10 | Eat 三态手工验收矩阵未实机：meal/烟卷/薄片/啤酒/仙馔/营养膏/背包吃/动物/尸体 ×（父关、父开子关、父开子开）；重点=父关与旧实现完全一致、父开子开时食物行为与父开子关一致（并集不改变食物）、父开子关时啤酒/仙馔仍响、关父项后子项灰显且值归 false | 未验证 | 已发布手感/功能验收 | 开放（列"实机"类） | `src: … §5 L61; docs/handoff-eat-occurrence-granularity-zh.md §6 L217` |
| OQ-11 | US 双开矩阵未实机（四种组合"恰好一条声音"+跳过记录） | 未验证 | SR+US 组合 | 开放；矩阵本体 [xref: PKG-3] | `src: docs/handoff-0.3.3-zh.md §5 L61, §6.2 L75` |
| OQ-12 | CI 接线（11 项门+隐私门+发布面门、SDK 10.0.x）推送前无法实跑；YAML 仅人工校对 | 未验证 | 发布流水线 | 开放 | `src: … §10.4 L125` |
| OQ-13 | 历史/tag 重写：5 个 `[known-debt]` 文件需重估（范围比先前记录更大）；重写本身需授权 | 需授权+未验证 | 仓库历史隐私 | 开放；方案 [xref: PKG-5] | `src: … §10.4 L126` |
| OQ-14 | Steam 文案/包核验随解除阻断执行（本轮未做） | 依赖外部（OQ-2） | 外部渠道 | 开放；文案源 [xref: PKG-4] | `src: … §10.4 L127` |
| OQ-15 | （对接收方）是否采用父+子两级形态，还是 3 态选择卡 | 待裁决（外部） | US/姊妹项目 UI | 开放；SR 已取两级 | `src: docs/handoff-eat-occurrence-granularity-zh.md §8.1 L266` |
| OQ-16 | （对接收方）子项命名范围：「使用成瘾品」原版等价于药物，实现覆盖面是"任意零营养可摄入物"；模组内容可能需更宽措辞 | 待裁决（外部） | 本地化正确性 | 开放；SR 已用原版措辞 | `src: … §8.2 L267` |
| OQ-17 | （对接收方）是否接受"带营养成瘾品（啤酒/仙馔）父开子关时仍响"；若要静默需加 `ThingDef.IsDrug` 排除（另一语义决定） | 待裁决（外部） | 判定语义 | SR 默认接受 [F 记载]；接收方开放 | `src: … §5.1 L204, §8.3 L268` |
| OQ-18 | （对接收方）回落口径：本仓库取"未确认 ⇒ 完整 job 级（不丢声音）"；接收方若更在意"严格"可取"未确认即静默"——源文档评估"会回到静默 hole，不建议" | 待裁决（外部） | 行为一致性 | SR 已定案；外部开放 | `src: … §8.4 L269` |
| OQ-19 | 是否泛化到其他动作：`Sleep`（在床/未上床）、`Work`、`Social`、`Joy` 目前同样 job 级/粗略判定，同类粒度问题会重复出现 | 待裁决 | 后续版本（0.4/US） | 开放 | `src: … §8.5 L270` |
| OQ-20 | 粒度偏好放哪一层：策略层（纯函数+注入采样）还是动作定义/注册表（与动作门 `SqueakActionDef` / `allowExternalActions` 的关系） | 待裁决 | 架构 | 开放（DEC-4 条 8 已给 SR 倾向：策略层） | `src: … §4 L107, §8.6 L271` |
| OQ-21 | 默认值是否写进兼容政策（"一旦发布，父关 = 默认 job 级就不允许再翻"） | 待裁决 | 存档/设置兼容承诺 | 开放；主源未见政策落档 | `src: … §8.7 L272` |
| OQ-22 | 是否需要 Dev 诊断（面板显示当前模式与 `chewToilNameConfirmed`）以发现 B 的静默退化；约束=不新增日志事件 | 待裁决（可选增强，未实施） | 可观测性 | 开放 | `src: … §5.1 L206–208, §7.1 L262, §8.8 L273` |
| OQ-23 | 若产品要求父开子关时啤酒类带营养药饮也静默 → 需追加 `ThingDef.IsDrug` 排除判定（明载"这是另一个语义决定，不是本规格默认行为"） | 悬置（未采纳备选） | 判定语义 | 开放（挂 OQ-17） | `src: … §5.1 L204` |

## 9. 锚点（ANCH：必须逐字保留）

**设置/字段/UI**：`eatOnlyDuringChewing`（父，UI「仅在真正进食（正在摄入营养）时触发」）/ `eatIncludeDrugs`（子，UI「使用成瘾品」，原版措辞）；两者默认 `false`；`SqueakyRatkinSettings.cs`（字段+`ApplyToRuntime()`/`NotifyCheapRuntimeChanged()` 两处发布；发布表达式 `eatOnlyDuringChewing && eatIncludeDrugs`）；`SqueakyRatkinSettings.ExposeData.cs`（Scribe add-only 各一行；`PostLoadInit` 父关⇒子 false 归一）；行高 `34f`、高度 +34f、`MeasureBasicsContentHeight` 同步、`Toggle(enabled:/disabledReason:)`、禁用原因键"需先开启上方开关"、父 Tooltip 边界句、子 Tooltip"按咀嚼/点燃阶段判定，无法识别时回落完整进食流程"。`src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L123/L187–198, §7 L225–231, §7.1 L253–260; docs/handoff-0.3.3-zh.md §4 L47–48`

**规则/实现符号**：`Source/SqueakyRatkin/SqueakEatOccurrence.cs`（纯文件、零 Verse、纯度门不变）；`enum SqueakEatOccurrenceMode { WholeJob, GainingNutrition, ChewingToil }`；`ChewingToilDebugName = "ChewIngestible"`；`ChewingOnlyDefault = false`；`ResolveMode(bool eatingOnly, bool includeDrugs)`；四参 `AllowsOccurrence(mode, gainingNutritionNow, chewToilActive, chewToilNameConfirmed)`；`CompSqueaker.cs` — `IsEating()` / `IsGainingNutritionNow()`（`Pawn.jobs?.curDriver is IEatingDriver eating && eating.GainingNutritionNow`）/ `SampleChewingToil()` / 进程级静态 `chewToilNameConfirmed`（不 Scribe）；`StringComparison.Ordinal`；"决策出生即纯"。`src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L131–177, §7 L225–230, §7.1 L253–254`

**vanilla 符号**：`JobDefOf.Ingest`；`JobDriver_Ingest.MakeNewToils()`；`Toils_Ingest.PickupIngestible` / `CarryIngestibleToChewSpot` / `FindAdjacentEatSurface` / `ChewIngestible(...)` / `FinalizeIngest`；`ReserveFood`；`JumpIf(chewing, …)`；`PrepareToIngestToils_Dispenser`；`eatingFromInventory`；`IEatingDriver.GainingNutritionNow`（接口唯一成员）；`ToilMaker.MakeToil("ChewIngestible")`；`Toil.debugName`；`Toil.ToString()` = `debugName ?? "unnamed"`；`JobDriver.CurToil`(protected)/`CurToilIndex`/`CurToilString`(public)；`Pawn.jobs.curDriver`(public)；`toil.actor.pather.StopDead()`；`ticksLeftThisToil = Mathf.RoundToInt(thing.def.ingestible.baseIngestTicks * durationMultiplier)`；`ThingDef.IsNutritionGivingIngestible => IsIngestible && ingestible.CachedNutrition > 0f`；`ThingDef.IsDrug`；`preferability=NeverForNutrition`；`DrugAIUtility.IngestAndTakeDrug`；`JobGiver_Binge`；`JobGiver_TakeCombatEnhancingDrug`；`EatAtCannibalPlatter`；`StatDefOf.Nutrition = 5.2`（尸体）；`CompSqueaker.CurrentAction`；`1.6/Patches/Ratkin_AddSqueakComp.xml`（SR 装配点）。`src: docs/handoff-eat-occurrence-granularity-zh.md §2 L25–75, §5 L112–117; docs/handoff-0.3.3-zh.md §7 L85`

**数值**：`baseIngestTicks` 默认 `500`（≈8.3 秒）；烟卷 `720` t（≈12 秒）；薄片 `650` t（≈10.8 秒）；`Beer Nutrition 0.08`；`Ambrosia Nutrition 0.2`、`baseIngestTicks 80`；corpse `Nutrition = 5.2`；`Eat = EachTime, 144 ticks`；`globalMinIntervalTicks = 216` ≈ **3.6 秒**现实时间一发；横穿地图 20–30 秒 ≈ 5–8 发；站桩 ≈ 1–2 发；`scaleCooldownWithTimeSpeed` 默认开、1×/2×/3× 节拍一致；概率门只在 `RandomOneShot` 生效、`EachTime` 不抽概率。`src: docs/handoff-eat-occurrence-granularity-zh.md §1 L21–22, §2 L60/L68/L70/L74, §5.1 L203`

**验收/工具**：`tools/KernelCharacterization/{KernelCharacterization.csproj,UnitTests.cs}` 的 `EatOccurrenceRules`（四组合含"父关+子真=`WholeJob`"、三模式、回落、常量、两默认值断言）；`tools/SettingsFixtureGenerator/Contract/SettingsContract.cs`（镜像子字段）；`fixtures/expected/01-new-install-first-save.xml` 零 delta；`scripts/verify-local.ps1`（`-NoRestore` 11/11 全绿；`-PackDev`）；构建 0 warning / 0 error（Dev + Steam 双 flavor）；本地化 542 键、零重复、中英对齐。`src: docs/handoff-eat-occurrence-granularity-zh.md §6 L214–218, §7.1 L255–259; docs/handoff-0.3.3-zh.md §2 L33, §7 L86, §10.2 L111`

**版本/发布面**：`0.3.3`；`0.3.2`（跳过口径）；`v0.3.2-pre1`；计划 tag `v0.3.3`；`v0.3.0`（手感指纹基线）；`csproj <Version>` ≡ `About.xml <modVersion>`（`stage-package.ps1` 硬断言；release tag 基版本=csproj）；`未发布 — 0.3.3` / `Unreleased — 0.3.3`（UTC+8 换时间）；分支 `dev`/`origin/dev`/main；squash merge；`docs/release_review/release-0.3.3-review-zh.md`（Claim Pack）；`merge -s ours`（runbook 内条件，本体 [xref: PKG-4]）。`src: docs/handoff-0.3.3-zh.md §3 L38–41, §10.3 L120; docs/handoff-eat-occurrence-granularity-zh.md §1 L19`

**流程/隐私工具（09-13）**：`scripts/privacy-audit.ps1`（三向量+身份、`-FullHistory`、`-PrePush`、`$knownHistoryDebt`）；`scripts/check-pack-readiness.ps1`（版本轴/红线/暂存包/DLL 身份/`[claim]` 快照、`-RequireReleaseMetadata`）；`ci.yml`/`release.yml`；11 项门+隐私门（release 再加发布面门）；SDK 对齐 `10.0.x`；`227 revision`；`5` 条 `[known-debt]`；`19` 个已跟踪文件改动（`+161` / `−17`）+ `4` 个 untracked 新文档；`AGENTS.md`「External-state boundaries」。`src: docs/handoff-0.3.3-zh.md §2 L20, §10.1 L106, §10.2 L113`

**编号表与兼容标识（跨包指针）**：U1–U4；`VoicePackCompAttach`；skip 原因日志 `foreign_squeak_comp`；legacy 桥薄空类型 `SqueakyRatkin.SqueakVoicePackDef`；顺序硬门 `U1 → US 型 Ratkin 包/桥 → SR 1.0 内容化`；packageId `coahuilite.squeakyratkin`；F1–F8、Q1–Q4、B1–B5、P0–P4 [xref: PKG-3]；Q1–Q10 [xref: PKG-3]；R1–R12、N1–N3、V1–V3 [xref: PKG-5]；"每次事件恰好一条声音"（验收信号原话）。`src: docs/handoff-0.3.3-zh.md §1 L13–14, §4 L51–52, §5 L59–60, §6.2 L71–75, §7 L83–84, §10.1 L105`

**授权边界**：外部有效操作（commit 属边界内地带：09-13 口径为"本地 commit 允许"、远端操作需授权）需维护者显式授权（`AGENTS.md`）；隐私审查覆盖完整可达范围（发布/推送/上架前）；US 仓库只读（维护者授权路径）；不改语义/默认值（发布会话边界）；Steam 涉及人工编辑。`src: docs/handoff-0.3.3-zh.md §2 L34, §6.1 L68, §8 L89–93, §10.3 L116`

## 10. 候选教训（LES-1…LES-7）

| id | 教训 | 支撑证据 | 强度 | 适用边界 | 来源 |
| --- | --- | --- | --- | --- | --- |
| LES-1 | "静默的归属变更"最难排查：不异常、不 warn、不进 rejected 日志——开关类功能必须把**行为变化映射到用户可感/可查信号**（文案边界要素 + 真值表 + 可观测性钩子），否则只有盯现场才能发现 | INC-1 全案（含复现步骤与"为什么容易漏"三条） | 强 | 任何在采样/过滤层改变动作归属的开关；不特指 Eat | `src: docs/handoff-eat-occurrence-granularity-zh.md §3 L85–89, §2.4, §5.1 L206–208` |
| LES-2 | 已发布的手感/默认值是兼容资产：新开关默认双关 = 与旧实现逐位一致；"默认收窄等于删掉招牌行为" | DEC-1（§4.1 定案 + fixture 零 delta EV-1） | 强 | 玩家可感默认行为；延伸明载"0.4/US 同样适用" | `src: docs/handoff-eat-occurrence-granularity-zh.md §4 L101; docs/handoff-0.3.3-zh.md §4 L46` |
| LES-3 | 依赖外部标识符（debugName 类字符串）的严格判定，回落方向应选"退回宽松旧行为（不丢声音）"而非"退回严格判定（静默）"；语料并明载回落副作用="子开关退化为无操作"，优于"成瘾品静默" | DEC-3 + `AllowsOccurrence` 实现 + §8.4 反向选项被评"不建议" | 中等（单一仓库裁决 + 论证，无对照实验） | fail-safe/fail-open 择向；对接收方仍是开放选项 OQ-18 | `src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L149–151/L181–185, §8.4 L269` |
| LES-4 | 对抗复核的"无解"结论是时间点快照：本轮"vanilla 无公开 toil 权威"被同文档后续的 public 属性核实推翻，且推翻直接解锁了最终形态（B 档） | INC-3/注 1 + §2.3 事实层 | 中等（一次实例，但更正链完整可核） | API 面可能随版本演化的判据复核；不构成"所有复核都要复核"的普适规则 | `src: docs/handoff-eat-occurrence-granularity-zh.md 注1 L279, §2.3 L52` |
| LES-5 | 文档同步面要落成清单并携带"曾漏掉的"标注：UI codemap 曾漏被显式记录，防止下次清单回退 | INC-2 + §6/§7.1 两处标注 | tentative（单例） | 本仓文档同步面场景 | `src: docs/handoff-eat-occurrence-granularity-zh.md §6 L219, §7.1 L260` |
| LES-6 | 同一交接文件追加接续记录会产生内部时间层矛盾（"未提交"vs"已提交"）：接续内容自带日期可定位，但前段读者若忽略 §10 会拿到过期状态——引用 handoff 必须带段落时层 | CNF-1（README §4 亦以本文为示例原型 [xref: README 层观测，不摘要]） | 中等 | handoff/日志类文档；"分段写作要带时层"对本仓是惯例（§10 标题自带日期）而非缺陷 | `src: docs/handoff-0.3.3-zh.md 头 L4, §2 L19, §10 L101–107` |
| LES-7 | 门禁"谁来做"是冗余主源：覆盖不缺时，重复人工表述（同一清单 runbook 3 次、隐私 4 处 0 脚本）应转成脚本 + 三步仪式 + CI 接线；且落地后以 AGENTS.md 禁止"重新加回人工仪式"（防回潮） | DEC-10/11 与 EV-3（09-13 评估与落地一体） | 中等（[C] 核心结论 + 已落地结构佐证；论证细节在 PKG-5） | 发布流程域；不泛化到所有人工检查 | `src: docs/handoff-0.3.3-zh.md §10.1 L105–106` |

## 11. 证据缺口（GAP-1…GAP-7）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | Eat 三态矩阵/US 双开矩阵是否已实机执行、结果如何 | 两主源均只列"开放/待实机"；实机记录属 `TODO.md`/后续会话（记忆面语料外） | 实机会话记录或 TODO 快照 [xref: MEMORY.md/TODO.md（语料外指针）] |
| GAP-2 | 0.3.2 跳过的终局裁决与理由 | 语料只有"口径不变"，无裁决动作 | 发布会话的裁决记录/CHANGELOG 终稿 [xref: PKG-4] |
| GAP-3 | 隐私债务"比先前记录更大"的先前口径 | 先前记录不在本包主源 | PKG-5 主源（rewrite-plan）对照 |
| GAP-4 | Q1–Q10 / Q1–Q4 / V1–V3 / R1–R12 逐项内容 | 主源仅存编号与主题词 | PKG-3 / PKG-5 展开 |
| GAP-5 | Steam 评论原文与时间、是否另有负面反馈（U-5/U-6） | 仅正面转述一条 | 评论渠道访问（语料外） |
| GAP-6 | US 侧 U1–U4 在分派后是否动过 | SR 仓库只读窗口，US 进展不在语料 | US 仓库记录（授权面外） |
| GAP-7 | 09-13 三块提交的 commit 标识与顺序 | §10 仅声称；本包被禁外部取证（规则 9） | 下游 `git log`（冻结动作豁免外，供终审） |

