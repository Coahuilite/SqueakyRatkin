# 03 · 终审强模型提示词（阶段 C：产出最终 handoff package）

> **用途**：把阶段 B 的 `handoff-package-v2.md` 交给最高档模型，产出**最终 handoff package**——面向维护者 / 执行者的独立判断文档，供其推进 `TASK-docs-consolidation-zh.md` 的收敛与记忆压缩。
> **输入**：只接收 v2（+ 按 §5 质疑协议允许的少量源回查）。**不接收**全量 `docs/**`。
> **输出**：最终 handoff package（§4 骨架）。
> 末尾附录是上游原始要求（英文原文），§2 是其中文化执行版本。

---

## 0. 调度方填写的变量

```text
{{IN_V2}}       = {{OUT_DIR}}/final/handoff-package-v2.md
{{OUT_FILE}}    = {{OUT_DIR}}/final/handoff-package-final.md
{{RAW_STATS}}   = docs/** 原始规模（字符数 + 文件数）
{{REPO_REV}}    = <git rev-parse HEAD>
{{TASK_BOOK}}   = TASK-docs-consolidation-zh.md（下游用途上下文；**不是**本阶段语料）
{{MAX_REREAD}}  = 允许的源回查次数（建议 ≤ 8；每次限单文件单章节）
{{OUT_DIR}}     = _consolidation
```

---

## 1. 角色与任务

你是**终审专家模型**。输入是一份由上游 agent 压缩的任务包 v2，目标是为下游读者重建这个项目的**决策演化**，并产出一份可用于独立判断的 handoff package。

三条立场要求：

1. **不盲信上游**：v2 是二手材料。凡结论让你觉得可疑、含糊或代价重大，按 §5 回查源文件（有限次），或在文中明确标注不确定。
2. **不重开语料**：不得要求全量 `docs/**`；只能按 v2 §11 的质疑钩子做定点回查。
3. **判断权在你**：你可以不同意 v2 的表述，但必须给出依据（回查到的源材料或 v2 内部矛盾），并把"我的判断"与"文档陈述"分开标注。

## 2. 上游要求（执行版，共 12 条）

1. **高召回保留决策相关信息**：优先保留决策、备选方案、理由、假设、约束、分歧、反转、事故、结果、交接风险、未决问题与重要时间顺序。
2. **积极去重，但不折叠有意义差异**：尤其不能抹平「随时间变化」或「互相冲突的解释」。
3. **明确区分四类信息**：documented facts / participant claims / your own inferences / unresolved or missing information（`[F]/[C]/[I]/[U]`）。
4. **保留 provenance**：重要结论必须可回溯到源文档或源位置。
5. **不做最大压缩**：采用适度、感知损失的压缩；凡可能实质影响后续解释的信息，保留。
6. **重建决策演化**，而不是逐文档摘要。
7. **识别矛盾、被放弃的方案、变化的假设，以及结果与原始预期不符之处**。
8. **教训只在证据支持时提取**：保留 tentative 标记；不得把一次性事件写成通用最佳实践。
9. **围绕最有后果或最含糊的决策保留足够细节**，使下游强模型无需重开全部源语料即可独立挑战你的解释。
10. **产出面向另一个专家模型的最终 handoff package**，结构见 §4。
11. **对含糊、重大、有争议的区域多花 token**，对例行状态少花。
12. 内部结构、中间产物与模型分工由上游工作流决定（即本套件的 README / 01 / 02）。

## 3. 硬边界

1. **只读**：禁止修改任何仓库文件；唯一写入 = `{{OUT_FILE}}`。
2. **不得新增事实**：最终包的事实性内容必须来自 v2；你的推断标 `[I]`，回查所得标 `[src-回查]`。
3. **不得消除矛盾**：v2 §6 的每一组 `CNF` 必须在最终包保留（可重组，不可删）。
4. **不得升格标签**：`[C]/[I]/[U]` 不得写成确定事实；tentative 教训保持 tentative。
5. **数值/标识逐字**：版本、commit/tag、run id、hash、文件数、字段名、ABI 键原样。
6. **隐私写法**：不复制本机绝对路径、盘符形态、日志摘录、凭据、`PublishedFileId` 值。
7. **术语**：`鼠辈啁啾` / `Squeaky Ratkin` 原样；中文散文称鼠族。
8. **先读 v2 §0**：确认它是全语料 v2 还是部分包 v2；若只覆盖部分包，最终包必须显式声明适用范围，**不得**把结论外推到未覆盖主题，并把缺口写进 §8 / §10。

## 4. 输出骨架（最终 handoff package）

> **引用约定**：本提示词正文提到 `§n` 时，除写明「本文 §n」或「§5 质疑协议」这类小节名外，**一律指本骨架的输出章节**（即输出文件里的 `## n.`）。

```markdown
# 最终 handoff package：<主题范围>（独立判断版）

## 0. 元数据与可信度声明
- 输入：v2 路径 / v2 覆盖范围（全语料或部分包）/ v2 压缩比 / repo_rev / 允许的回查次数与实际使用次数
- 本包相对 v2 的改动：**新增判断（一律标 `[I-终审]`，不得引入 v2 之外的事实）**、修正（附回查依据）、仍存疑处
- 不能保证的部分（诚实声明）

## 1. 项目上下文（简洁）
（是什么项目、当前版本与发布状态、权威序、时间范围）

## 2. 主要时间线
| 时间 | 事件 / 裁决 / 变化 | 当时状态 vs 现行状态 | 来源 |

## 3. 关键决策史（最重篇幅）
逐条：决策 / 状态 / 时间与主体 / 备选与被吸收项 / 理由与当时假设 / 反转与修订 / 后果与证据 / 我的独立判断（若与 v2 不同，给依据）/ 来源

## 4. 重要假设及其演化
（当时假设 → 是否仍成立 → 何时因何变化 → 证据）

## 5. 重大失败 / 成功与支撑证据
（含证据强度：实测/复跑/论证/单方声明；不得把声明写成实测）

## 6. 矛盾与未决问题（零丢弃）
- 矛盾：双方要旨 + 时间层 + 可能解释 + 建议回查点
- 未决 / 待裁决 / 需授权 / 未验证：问题 + 影响面 + 当前状态

## 7. 候选教训
（每条：教训 + 证据 + 强度 + 适用边界；tentative 保持 tentative）

## 8. 证据缺口
（想回答但答不了的问题 + 需要什么才能回答）

## 9. 来源回引
（源文件 → 主题 → 章节/行 → 载体包；含回查记录）

## 10. 供下游与维护者挑战本包的钩子
（结论 → 最可能的两种误判 → 回查点 → 若被推翻会影响什么）

## 11. 对下游动作的约束提示（仅提示，不作裁决）
（本包证据支持的、影响 `TASK-docs-consolidation-zh.md` 收敛判定的硬约束，例如路径不可变的合同、必须保留的发布证据、未授权不得触碰的远端/隐私动作。逐条给来源；**不替维护者做保留/废弃决定**）
```

## 5. 质疑协议（有限回查）

1. 优先对以下内容回查：你的 `[I]` 推断、全部"未来路线"判断、所有后果重大的决策、v2 中自相矛盾之处。
2. 每次回查前写明：要验证的结论 / 要读的文件与章节 / 预期会看到什么。
3. 每次回查后写明：看到什么 / 结论是否改变 / 是否留下新的不确定。
4. 达到 `{{MAX_REREAD}}` 次仍不确定 → 写进 §8 证据缺口与 §10 钩子，**不要继续扩查**。
5. 回查记录进 §9，格式：`[src-回查] <文件> §<章节> L<a>–L<b> → 结论/不变/修正`。

## 6. 完成前自检

1. 12 条上游要求逐条自检（§2），列出落点章节。
2. **反盲信检查**：至少写出 3 处你自己也不确定或与 v2 表述不同的地方（若无，说明为何）。
3. v2 的 `CNF` / `OQ` / `GAP` 数量是否与最终包一致（可重组但不得减少）？
4. 是否存在把 `[C]` / `[I]` 写成事实陈述的句子？
5. 是否存在无来源的关键结论？
6. 抽查 5 个数字/标识符是否与 v2 逐字一致？
7. 隐私写法是否成立？
8. §11 是否只写了"证据支持的约束"，而没有替维护者做保留/废弃裁决？

---

## 附录：上游原始要求（英文原文，供对照）

> Design your own workflow for this project based on the actual document structure and content you observe.
> The goal is to reduce the amount of raw material that must later be sent to a stronger model, while preserving enough information for high-quality independent reasoning.
> 1. Preserve decision-relevant information with high recall. Prioritize decisions, alternatives, rationale, assumptions, constraints, disagreements, reversals, incidents, outcomes, handover risks, unresolved questions, and important chronology.
> 2. Deduplicate aggressively, but do not collapse meaningful differences between documents, especially when they reflect changes over time or conflicting interpretations.
> 3. Clearly distinguish: documented facts, participant claims, your own inferences, unresolved or missing information.
> 4. Preserve provenance. Important conclusions should remain traceable to their source documents or source locations.
> 5. Do not optimize for maximum compression. Use moderate, loss-aware compression. If information may materially affect later interpretation, retain it.
> 6. Reconstruct the project's decision evolution rather than merely summarizing documents one by one.
> 7. Identify contradictions, abandoned approaches, changed assumptions, and cases where later outcomes differ from original expectations.
> 8. Extract transferable lessons only when the evidence supports them. Keep tentative lessons explicitly tentative, and do not turn one-off events into universal best practices.
> 9. Preserve enough detail around the most consequential or ambiguous decisions so that a stronger downstream model can independently challenge your interpretation without reopening the entire source corpus.
> 10. Produce a final handoff package optimized for another expert model. It should include: concise project context, major timeline, key decision history, important assumptions and how they evolved, major failures/successes and supporting evidence, contradictions and unresolved questions, candidate lessons, evidence gaps, references back to source material.
> 11. Spend more tokens on ambiguous, consequential, or disputed areas and fewer tokens on routine status information.
> 12. Use your judgment to choose the most suitable internal structure, intermediate artifacts, and division of work between available models.
