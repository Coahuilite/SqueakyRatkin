# SR 维护状态与待裁决

> 2026-09-19 更新（文档收敛、独立化、archive/codemap 清理之后；0.3.3 因 R6 Remix 缺陷**撤回后本日重新发布**）。这里提供维护入口：现状、证据边界、待修复与待裁决；具体行动跟踪见 [TODO](../TODO.md)。

现行文档（10 份，全部自足，不依赖历史材料）：本维护状态、[架构合同](project-architecture-contract.md)、[设置合同](settings-ui-product-contract-zh.md)、[日志协议](logging-protocol.md)、[发布流程](release-runbook-zh.md)、[Steam 页面草稿](steam-workshop-page-copy-draft.md)、[隐私重写方案](privacy-history-rewrite-plan-zh.md)、双语 CHANGELOG。发布证据与终审报告在 [`release_review/`](release_review/)（含 7 份 Claim Pack、2 份流程复盘与终审报告）；原 `docs/archive/` 与全部 `codemap.md` 已删除，靠 git 历史追溯（收敛提交 `8481cbe`）。

## 状态与证据

| 面 | 已知 | 不能据此推出 |
| --- | --- | --- |
| 本地版本 | csproj/About = 0.3.3；CHANGELOG 时间 = 2026-09-19 17:26 UTC+8 | 本地版本号本身不代表渠道状态 |
| 发布记录 | **0.3.3 = GitHub 正式 Release（首发当日因 R6 撤回，同版本重发）**，证据 [Claim Pack](release_review/release-0.3.3-review-zh.md)；0.3.0 = GitHub 完整 + Workshop 页面级；0.3.2-pre1 = GitHub prerelease | 旧 CI/页面不证明当前版本；Workshop 本次未观察 |
| 0.3.3 | 重发 tag `v0.3.3` → `cd90a9e`（PR #32）；资产下载级核验（1,590,744 B，SHA256 `fced520a…` = API digest）；Steam 未上传，暂存包就绪（`commit=cd90a9e`） | 商店页面仍是 0.3.0 口径；首发资产（1,590,750 B、`b7eb7dac…`、downloadCount 1）已随 tag 删除，只在本记录留证，无法回收 |
| 发布分支 | 规则 = 每个 minor 在自己的分支开发（本次 `0.3.x`）、发布时 merge 到 `main`；**`dev` 已删除（本地 + 远端）**；main 已含发布提交 `cd90a9e`（= tag `v0.3.3`）及其后的纯文档提交，`0.3.x` 与之 tree 一致（不追记持续移动的 main HEAD，以 tag 为锚） | 分支已同步不代表下一版已准备 |
| Steam | 最后记录仍为阻断；新草稿面向 0.3.3，未上传 | 页面现状、旧公告已删或二进制已验证 |

0.3.1 无独立正式发布记录；0.3.2 正式版按并入 0.3.3 的工作口径推进，具体渠道裁决仍由发布会话确认。两个 CHANGELOG 按本轮要求原样保留，其中“旧前新后”规则与正文倒序、时间行与发布证据的差异仍存在，不据其自行改写发布史。

## 优先处理

| 项 | 结论与下一步 |
| --- | --- |
| **R6 Remix 折叠（0.3.3 已修正）** | 缺陷成立且玩家可见：三层路径按原始位序取 index，缺层时命中空位 → 该次事件**静默**且内置层永不可达；默认 Fallback 用户不受影响，**开启 Remix 的默认装配**（出厂播种的 `SR_OfficialExample_Race` 声明空 fallbacks）命中 shape A。修正 = 仅非 None 层等权折叠（回到 0.2.4 语义）。守卫 = shape A/B 断言（修正前 none=105/200、内置 0；修正后 none=0、105 race / 95 内置）+ `corpus-0.3.0-r6.txt` 字节回放 + 历史语料 540 行差异断言。首发 0.3.3 因此撤回并重发 |
| **R4/R5 日志** | 文档已补 v1 `pawn pawn_id` 并纠正 28/4 事件分组；实际 label 输出与旧隐私禁令冲突仍开。普通字段仅编码、非通用脱敏。处理字段须先确定兼容政策 |
| **R3 作者 ABI** | add-only/17 键 append-only/fail-closed 承诺继续有效；“首个携带 0.3.1 ABI 的发行版本”尚未精确到 tag。pre1 是有证据候选，不擅自排除 prerelease |
| 实机与发布 | Eat 三态、身份门余项、低频回归沿用现有 TODO；取得结果后决定修复或按 [runbook](release-runbook-zh.md) 发布。旧“V1–V3 都待定”不再成立：V1 追认、V3 接受、V2 仅出方案 |
| 可观测性 | tier 折叠已记录，是否细分待裁决；Eat 不新增日志事件，替代 Dev 面板尚未实施 |

## 迁移与退役

**2026-09-19 维护者裁定：SR 与 US 共存期间，两个 mod 各自装配、各自发声（双 comp / 双响）是正常行为**；玩家若选择安装 SR 的 legacy 独立版，双装配、双发声同样正常。该裁定废止先前"US 侧必须跨程序集检测并让位、以及 `U1 → 包/桥 → SR 内容化` 顺序硬门"的要求——兼容工作只剩"切换过程不破坏玩家"的部分。

仍需裁决或留意的面（技术风险不因该裁定消失）：

| 面 | 未闭合内容 |
| --- | --- |
| 类型名所有权 | 两个 DLL 同时定义 `SqueakyRatkin.SqueakVoicePackDef` 时 XML `Class=` 解析 first-wins 可能绑错；legacy 桥由谁、何时定义 |
| packageId 归属 | SR 降级为 voicepack 时谁继承 `coahuilite.squeakyratkin`（推荐主线继承、legacy 取新 id，避免玩家音源选择 PackKey 重置） |
| legacy 渠道 | 另开冻结 Workshop 条目，还是仅 GitHub 归档；是否承诺跨游戏版本维护 |
| US 前置形式 | SR 1.0 对 US 取硬前置（缺则列表警告）还是软前置（无 US 静默） |
| 设置迁移 | US 一次性导入 SR 设置（反射读）还是让玩家重设一次 |
| 公告与过渡 | 1.0 前置公告窗口（建议 4–8 周）与是否插入过渡版 |
| 切换实机验收 | 不再要求"单响"：SR 0.4 + US 双开正常（双响为预期）；SR 1.0 内容化后 US 单装可用；legacy 版可装且不破坏存档；双向卸载安全 |

已核实事实：1.6 comp 不进存档（按当前 def 重建）；缺依赖 = mod 列表警告，不崩档；重复 packageId 被 RimWorld 忽略；重复 defName 会被追加随机后缀（PackKey 失配）。Kiiro 实验分支已删除；未经作者许可不得发布或宣传相关兼容内容。

## 历史隐私债务

09-13 裁决维持 A（不重写 + known-debt 纪律），B 仅备方案；[专项方案](privacy-history-rewrite-plan-zh.md)（现行活跃文档）保留。债务报告覆盖 5 个文件；10/9 tag、227/230 revision、约 54 处 hash 引用均为历史口径，执行前须重新盘点，不能照旧数字 force-push。

B 仍需确认执行授权、mirror/refs 备份、工具、替换边界、tag/commit-map、协调窗口与跨仓通知；不可回收的 fork/缓存及旧 SHA 可访问风险未消失。当前默认扫描只覆盖跟踪树及配置的模式，known-debt 以模式/路径匹配；扫描绿不代表全部信息无隐私风险。本轮不执行历史重写、不扩大例外台账。

## 阅读纪律

合同陈述应有行为；源码回查陈述实际实现；旧 Claim Pack 是当时渠道证据；终审是带边界的判断。已解决的时间层留在 archive，尚未裁决的内容不能因“精简”升格。全量 51 CNF / 96 OQ / 51 GAP 的原陈述与编号保存在终审附录，那里不是当前待办清单。
