# PKG-4 P0 slices

----- 6. 矛盾与反转（CNF-<n>） L489-504 -----
## 6. 矛盾与反转（CNF-<n>）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
|---|---|---|---|---|---|
| CNF-1 | 规则「已发布条目按时间顺序，旧前新后；未发布条目置顶」（src: docs/CHANGELOG.md §Changelog Template L8；ZH L8）vs 文件实际排序（EN L39–202：0.3.0→0.2.0 逆序、0.1.x 正序留底；ZH 同） | 规则与正文并存于冻结快照；正文演化史 [I] I-9 | 同一文档的规范条款与自身正文排序互相否定 | I-9（prepend 惯例 + 0.1.x 块未回改） | 终裁：改规则为"新在前"或重排正文——属维护者裁决面，本包不代选 |
| CNF-2 | 0.2.0 词表 `GitHub：verified / Steam：unverified.manual`（src: docs/release_review/release-0.2.0-review-zh.md L17–18）vs runbook 词表 `完整/待补；unverified/页面级已核验/完整`（src: docs/release-runbook-zh.md L99–101） | 0.2.0 复盘早于模板化（≤2026-08-16 → runbook 2026-09-13 面） | 渠道状态枚举不连续、"verified"未入词表 | 模板词表是从 0.2.0 实践中析出的细化（页面级/完整二分） | 提炼阶段统一词表时保留 0.2.0 原文措辞为历史层 |
| CNF-3 | 模板行集（src: docs/release-runbook-zh.md L75–92）vs 六个实例实际行集差异（0.2.1 缺字节数；0.2.3/0.2.4 多 `zip SHA256`；0.3.0 `资产 SHA256`；pre1 多"分支状态"；LoadFolders 行 0.2.4 起消失） | 实例各自写作时点 2026-08-16→08-23；模板现行文本 2026-09-13 面 | 模板不是严格 schema，实例先行演化 | 模板滞后于实践（[I]）；LoadFolders 行消失 ≈ 被 `check-pack-readiness` 读时门接管（runbook L18） | 回查 `check-pack-readiness.ps1` 断言清单（不在本包主源；终裁阶段可开） |
| CNF-4 | 0.2.4 GitHub 发布时间 2026-08-18 00:10 UTC+8（=08-17 16:10 UTC）（src: docs/release_review/release-0.2.4-review-zh.md L12）vs 同日页面观察"Updated 17 Aug 09:22"（L32）；0.2.1"Updated Aug 15"vs 发布 2026-08-15T17:54:24Z（src: docs/release_review/release-0.2.1-review-zh.md L12、L41） | 同会话内两个记录面 | 页面 Updated 时间与 UTC+8/UTC 都不能直接对齐 | I-3：页面为 ≈UTC-7 时钟 | 在任何跨渠道时间账里显式标注各字段时区；回查 Steam 页面时区字段定义 |
| CNF-5 | 0.3.0 Claim Pack：旧置顶公告"**仍在线**——待维护者在 Steam 编辑器删除后复核"（2026-08-21）（src: docs/release_review/release-0.3.0-review-zh.md L23、L29）vs 文案维护源："置顶公告已于 0.3.0 **如期删除**"（写作日期 [U]）（src: docs/steam-workshop-page-copy-draft.md L17、L25、L200） | 前者 = 发布当日观察；后者 = 之后的写作层 | 删除动作完成与否两口径 | 后者写得更晚、代表更新；但"删除后复核"这一动作在语料内无独立观察留痕（后写口径不代表前段不存在） | OQ-3：一次带日期的页面复采即可关闭 |
| CNF-6 | pre1 待办：「正式 0.3.2 发布时替换 changelog 时间、执行 Steam 阶段 3 与完整收尾」（2026-08-23，预设正式 0.3.2 会发生）（src: docs/release_review/release-0.3.2-pre1-review-zh.md L27）vs CHANGELOG Notes：「0.3.2 仅作为 GitHub prerelease 发布……（工作）随 0.3.3 一同发布」（写作日期 [U]，U-8）（src: docs/CHANGELOG.md L56；ZH L56） | pre1 层 vs 其后改道层 | 0.3.2 的命运：待正式 vs 永不正式 | 版本规划改道（动机不在本包主源）；后写口径 = 现行 | 0.3.3 发布时是否继承 pre1 资产证据；编排方可向 PKG-6 核改道决策记录 |
| CNF-7 | Change Notes 计数 3（2026-08-16）→ 6（2026-08-18）（src: docs/release_review/release-0.2.1-review-zh.md L43；docs/release_review/release-0.2.4-review-zh.md L32）vs 「0.2.3 上传未执行」（src: docs/release_review/release-0.2.3-review-zh.md L29） | 三处不同日期的页面/仓内记录 | +3 条 notes 与仅 0.2.2、0.2.4 两次有记录的上传不匹配 | I-12：0.2.3 实际可能上传过，或存在未记录条目；change notes 与版本事件非一一对应 | 回查 Steam 页面 change notes 列表原文（导出）；影响 DEC-08 矩阵 0.2.3 格 |
| CNF-8 | 隐私结论措辞：`0 命中`（src: docs/release_review/release-0.2.1-review-zh.md L19）/ `0 真实命中`（docs/release_review/release-0.2.2-review-zh.md L19；release-0.3.0-review-zh.md L18；release-0.3.2-pre1-review-zh.md L19）vs 模板 `0 未接受命中`（docs/release-runbook-zh.md L91） | 各实例写作时点 vs runbook 2026-09-13 面 | 三种叫法是否同义未有定义 | [I] 都指 known-debt 台账外的真实隐私命中为零；"未接受"对应 `[known-debt]` 不判失败机制（DEC-11） | 提炼阶段统一定义一次，不抹平实例原文 |
| CNF-9 | ZH CHANGELOG：0.2.4 小节末行与 0.2.3 标题行间缺空行（src: docs/CHANGELOG.zh-CN.md L75–76 相邻）vs EN 对应处有空行（src: docs/CHANGELOG.md L75–76 之间）+ 双语同步规则（EN L17 / ZH L17） | 冻结快照层（各自写作时点不可分） | 版式漂移，Markdown 标题紧贴正文的渲染风险（语义内容仍等价） | 纯格式事故；不构成口径冲突 | 若产物入库前跑隐私/格式检查可顺带修复（本包不改源文件） |
| CNF-10 | 「0.3.0 失效公告」已现于 0.2.4 时期 Workshop 页面（2026-08-18）（src: docs/release_review/release-0.2.4-review-zh.md L32）vs CHANGELOG 0.3.0 Notes「正式发布前另行公告」（2026-08-21 条目，语义=拆分/前置 mod 的公告时机）（src: docs/CHANGELOG.md L70；ZH L70） | 0.2.4 页面记录 vs 0.3.0 文案层 | 对外预告的时点纪律是否被提前打破 | I-5：两者所指不同（置顶公告失效预告 ≠ 拆分预告），不构成直接违规；但"拆分预告不提名"（DEC-14）与页面曾含 0.3.0 字样事件的区分依赖该解释 | 回查当次页面公告原文（U-7） |
| CNF-11 | runbook L45「历史 `merge -s ours` 分叉处理**只在出现真实分叉时才用**」vs 0.2.4 教训「发布收尾后 dev 应**尽快**以 `merge -s ours`/对账吸收 main 指针」（src: docs/release-runbook-zh.md L45；docs/release_review/release-0.2.4-review-zh.md L44） | runbook 2026-09-13 面 vs 0.2.4 2026-08-18 记录 | 一个是"克制使用"，一个是"每次发布主动使用"——条件句张力 | [I] 不矛盾：squash 收尾**本身就制造真实分叉**，主动对账即"真实分叉出现时"的第一时刻；两处并存保留 | 提炼阶段合并为一条规则时保留 runbook 原条件句 |


----- 8. 开放问题、阻塞与交接风险（OQ-<n>） L539-556 -----
## 8. 开放问题、阻塞与交接风险（OQ-<n>）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
|---|---|---|---|---|---|
| OQ-1 | 0.2.2 Workshop 页面级观察"待补"从未回填；该版本渠道状态永久停在"维护者报告已发布" | 未验证（外部人工态） | 渠道矩阵 0.2.2 格；0.2.x 账完整性 | 冻结时仍开放（若渠道历史不可回采，按 [U] 永久记账） | src: docs/release_review/release-0.2.2-review-zh.md L22、L28 |
| OQ-2 | 0.2.3 究竟有没有被上传过（staging 未上传记录 vs change notes +3 矛盾） | 待裁决（需外部回查） | 渠道矩阵 0.2.3 格、CNF-7 | 冻结时开放 | src: docs/release_review/release-0.2.3-review-zh.md L29；docs/release_review/release-0.2.4-review-zh.md L32 |
| OQ-3 | 0.3.0 旧置顶公告删除后的"复核"是否做过（文案源声称删除，无带日期观察） | 未验证（依赖外部） | DEC-18、页面事实与 0.2.3 默认策略对外承诺 | 两口径并存（CNF-5），一次带日期页面复采可关闭 | src: docs/release_review/release-0.3.0-review-zh.md L23；docs/steam-workshop-page-copy-draft.md L25 |
| OQ-4 | changelog 时间政策：记录到分钟 vs 只记日期 vs 定义 cutoff（0.2.0 复盘留的岔路口，从未裁决） | 待裁决 | DEC-05/DEC-16；跨渠道时间账 | 开放（现行 = 分钟 + UTC+8） | src: docs/release_review/release-0.2.0-review-zh.md L83 |
| OQ-5 | 0.2.0 三条脚本改进候选的消化度：输入 DLL flavor 自检 / `pack-github` tag 基点校验 / PR CI 是否打包 | 未验证 | 发布面残留人工风险 | 部分吸收可证、基点校验无记录（I-7） | src: docs/release_review/release-0.2.0-review-zh.md L49；docs/release-runbook-zh.md L13、L50 |
| OQ-6 | Steam 二进制下载级验证（订阅后游戏内核对 DLL 版本）从未执行；0.2.1 起所有"页面级已核验"都停在这一边界 | 需授权（维护者人工）+ 未验证 | 渠道矩阵全部 Workshop 格的上限 | 现行口径即如此（runbook 边界条款明文） | src: docs/release_review/release-0.2.1-review-zh.md L47、L64；docs/release-runbook-zh.md L97 |
| OQ-7 | 0.3.3 发布准备面在语料内只有 CHANGELOG Unreleased + 功能规格交接：无 Claim Pack、Workshop 文案锚点仍在 0.3.0（版本号/下载链接/字符数/清单一次核对未做） | 依赖外部 + 未验证 | 下一次发布的完整仪式 | 冻结时状态 | src: docs/CHANGELOG.md L39–57；docs/steam-workshop-page-copy-draft.md L39、L92；[xref: PKG-6（0.3.3 交接面）] |
| OQ-8 | CI Node 20 弃用警告是否已消除（"不应描述为已经修复"的纪律残留问题） | 未验证 | CI 观察面 | 0.2.1 后语料无记录 | src: docs/release_review/release-0.2.0-review-zh.md L43；docs/release_review/release-0.2.1-review-zh.md L33 |
| OQ-9 | `vX.Y.Z-hotfixN` 与 `archive/` 分支两条命名/归档法从未被真实事件检验 | 未验证（前瞻条款） | 0.3.x 之后的发布路线 | 现行条款，零实例 | src: docs/release_review/release-0.3.0-review-zh.md L34；docs/release-runbook-zh.md L45 |
| OQ-10 | `$knownHistoryDebt` 5 条台账的清偿路径依赖授权（历史/tag 重写）；发布纪律依赖其不恶化（新增一条 = 一次裁决） | 需授权 | 隐私门与未来 push | 活跃债务（方案在 PKG-5） | src: docs/release-runbook-zh.md L71；[xref: PKG-5（隐私历史债务）] |
| OQ-11 | 交接风险：最小仪式第 3 件"发布裁决本身"（版本号/渠道/是否发/Workshop 文案）无规定留痕格式——0.3.2-pre1 靠文档头注"按维护者指示阻断"留下孤例 | 流程缺口 | DEC-03、DEC-15 类裁决的可审计性 | 现行无格式要求 | src: docs/release-runbook-zh.md L26；docs/release_review/release-0.3.2-pre1-review-zh.md L3 |
| OQ-12 | 交接风险：`release-<version>-review-zh.md` 命名无 hotfix/prerelease 规范（`release-0.3.2-pre1-review-zh.md` 是既成先例，模板未收编） | 流程缺口 | Claim Pack 文件轴可检索性 | 现行实例已偏离 | src: docs/release-runbook-zh.md L3、L62；docs/release_review/release-0.3.2-pre1-review-zh.md（文件名本身） |
| OQ-13 | 页面公告短语「0.3.0 失效公告」所指无原文快照（U-7），影响 DEC-18 对外承诺链条解释 | 需外部回查 | DEC-18、CNF-10 | 开放 | src: docs/release_review/release-0.2.4-review-zh.md L32 |


----- 9. 锚点（ANCH-<n>：必须逐字保留） L557-586 -----
## 9. 锚点（ANCH-<n>：必须逐字保留）

- **标识符 / 字段名 / 键 / 前缀 / 脚本与命令**：
  - 脚本：`scripts/verify-local.ps1`、`scripts/check-pack-readiness.ps1`、`scripts/privacy-audit.ps1`、`stage-package.ps1`、`pack-github.ps1`、`pack-steam.ps1`、`pack-dev.ps1`、`build-dev.ps1`、`build-steam.ps1`、`new-voicepack.ps1`、`tools/SqueakLogCharacterization`（src: docs/release-runbook-zh.md §入口契约 L12–16、§阶段 0 L38、§阶段 3 L55；各 Claim Pack）
  - 参数：`-RequireReleaseMetadata`、`-FullHistory`、`-PrePush`、`-NoRestore`；变量/字段：`SqueakyBuildFlavor=GitHub`、`[claim]`、`$knownHistoryDebt`、`[known-debt]`（src: docs/release-runbook-zh.md L12–16、L24–25、L71；docs/release_review/release-0.2.1-review-zh.md L13）
  - 版本主源字段：`<Version>`（`Source/SqueakyRatkin/SqueakyRatkin.csproj`）、`<modVersion>`（`About/About.xml`）、包内 `version.txt`（版本/flavor/commit 三行）（src: docs/release-runbook-zh.md §阶段 0 L32、L18；docs/release_review/release-0.3.0-review-zh.md L17）
  - 包内容排除项：`*.pdb`、`*.gitkeep`、`codemap.md`、`PublishedFileId.txt`（`About/PublishedFileId.txt` 仅存于上传副本）；staging 路径 `dist/steam/SqueakyRatkin`（src: docs/release-runbook-zh.md L18、L55–56）
  - 产品字段（发布面引用）：`LoadFolders.xml`、`IfModActive="Solaris.RatkinRaceMod"`、`defName="Ratkin"`、`<lowercase packageId>/<PackDef.defName>/<Action>/`、`1.6/Sounds/Squeak/`、csproj 浮动版本 `1.6.*`、`MentalBreakWorker.TryStart`、`MentalStateHandler.TryStartMentalState`、`MentalStates_BabyFits.xml`、`LifeStages.xml`、`canDoRandomMentalBreaks=false`、`SqueakLogData`、`PawnName`/`PawnId`、`pawn=`/`pawn_id=`、`srdiag v1`（28-event 协议）、`IsEgg`、`_Preview`、`GABP`（src: docs/CHANGELOG.md L104、L123、L132；docs/release_review/release-0.2.4-review-zh.md L37–38；docs/release_review/release-0.3.0-review-zh.md L35）
  - git 操作词：`git merge -s ours`、`git checkout --ours`、`git diff --stat`、`cherry-pick`、merge-tree（src: docs/release-runbook-zh.md L44–45；docs/release_review/release-0.2.1-review-zh.md L25–27；docs/release_review/release-0.2.0-review-zh.md L42）
  - 外部标识路径形态：`releases/tag/vX.Y.Z`、`actions/runs/<id>`、`.github/skills/squeaky-voicepack-authoring/SKILL.md`、`issues`（账号登录名按本包隐私申报不复制）（src: docs/steam-workshop-page-copy-draft.md L92–94、L170–172；docs/release_review/release-0.2.0-review-zh.md L9、L23）
- **数值 / 版本 / 阈值 / 计数 / 文件数 / 哈希 / 时间戳**：
  - 版本/tag：`0.1.0`、`0.1.1`、`Initial Workshop Upload`、`0.2.0`、`0.2.1`、`0.2.2`、`0.2.3`、`0.2.4`、`0.3.0`、`0.3.1`（未发布快照）、`v0.3.2-pre1`、`0.3.2`（无正式版）、`Unreleased — 0.3.3` / `未发布 — 0.3.3`、`vX.Y.Z`、`vX.Y.Z-hotfixN`、严格 SemVer / SemVer 2.0（src: 各 Claim Pack 版本行；docs/CHANGELOG.md L39、L56；docs/release-runbook-zh.md L50；docs/release_review/release-0.3.0-review-zh.md L34）
  - commits：`5818dedc3f22a8e7a4286d2b8b48f57377098b3f`、`57dfd1f`、`31c4e18ad0d98e6d5731f8e76e32fb4950ed061c`、`2936879`、`20d5c50`、`d73a0c3`、`1b1fe9e`、`e3ed623`、`03ebb6be20896071185193d94e8bf1006862f969`、`fca5fa6fbcc6bdacf13f5caa9959d6be2cd33ea7`、`53686f9222dce06dda1a2cd705e4ca368d635961`、`c06a90b360dab8824248e805b64bf4e9730c0007`（短式 `c06a90b`）、`a3e26e8`、`60f7d88893268528464ff8a326e3c90b0f716651`（短式 `60f7d88`）（src: 各 Claim Pack 源码提交行）
  - DLL identity：`v0.2.0+5818dedc3f22`、`v0.2.1+31c4e18ad0d9`、`v0.2.2+03ebb6be2089`、`v0.2.3+fca5fa6fbcc6`、`v0.2.4+53686f9222dc`、`v0.3.0+c06a90b360da`、`v0.3.2-pre1+60f7d8889326`；FileVersion `0.2.1.0`…`0.3.2.0`（src: 各 Claim Pack）
  - PR：`#7`、`#8`、`#9`、`#10`、`#13`、`#15`、`#23`、`#24`（src: 各 Claim Pack）
  - CI runs：`30478707424`、`31899583405`、`31955162848`、`32038162813`、`32037789607`、`32037960290`、`32038064450`、`32044477840`、`32044181594`、`32044344615`、`32044346462`、`32044414901`、`32436779849`、`32620324890`（src: 各 Claim Pack + 备注链）
  - SHA-256：zip `7B532A39BAF64FBCC192694B93271A54362E87429074A4C3D9944DACAF42BAB3`（0.2.0）、`cc8327ba54dd1453225578df7b798bce9f86d0963ab1b59d599eedbd0b04c268`（0.2.3）、`cfb4f5cbad3b868ca90755ee46835569ebbc6ce62b03ab221edb0214344fbfec`（0.2.4）、`3C0AD055E9DDBADAEC53CF2F55BF5B8B38D37D921A1FD2D5EA1C0ECEB54DAB97`（0.3.0 资产）、`d8c785a7a027c37ef5abb4e2eed0f85848044e7e18cc9ab199a453243e9aeb4b`（pre1）；DLL `98F66F8343824FFC060436074BB73B1D38E8EC02F72FF21F9814030BB4820A72`（0.2.1）、`3B0D1E8C969C44180C88BFA45C157A077F2392ED9345D5ED692290BA6C223C1A`（0.2.2）、`9e57d4b1e44d2d4b2324e0a6d269ec8d42f5582add436239793045fb8b2ba8e8`（0.2.3 GitHub）、`40c5c1c68cb0c28e4a3c041e834a29872a7b34b28bfee850769a474d1ef3ad7c`（0.2.3 Steam staging）、`17B9A2660D26BA05B9136D4800CEAF69F014A4F5EB61496714577E7C073DD3CF`（0.3.0）（src: 各 Claim Pack）
  - 资产字节：1,569,962（0.2.0）、1,573,976（0.2.2）、1,574,271（0.2.3）、1,574,386（0.2.4）、1,578,195（0.3.0）、1,588,909（pre1）；Steam 侧：1.871 MB（0.2.1 页）、1.872 MB（0.2.4 页）、1,879,864 B（0.3.0 API file_size）（src: 各 Claim Pack + EV-5/9/12）
  - 计数：115 文件（0.2.0–0.2.4 基线）、121（0.2.1 首发布泄漏态）、116（0.3.0 起）、6 个 `codemap.md` 泄漏、OGG 41、15 动作 / 17 动作键、28-event `srdiag v1`、其余 27 事件、24 提交扫描、known-debt 5 条、扫描模式 5 类、四个打包脚本、3 命令、3 常规设置页、七击 Developer & Diagnostics 页、8000 字符、2101（中文案）、4741（英文案）、3 评论/评分计数 79、Change Notes 3→6、41 条音频镜像、pawn 编号 37212 / 37215 / 37127（Pinenut、Rainlin、Wildtail）（src: 分布见 §3/§5/§7 各行）
  - 时间戳（全部逐字）：2026-07-04 18:35 UTC+8、2026-07-05 08:14 UTC+8、2026-07-30 00:32 UTC+8、2026-08-15T17:54:24Z（UTC+8 2026-08-16 01:54）、2026-08-16T15:17:38Z（UTC+8 2026-08-16 23:17）、2026-08-17 22:12 UTC+8（14:12 UTC）、2026-08-18 00:10 UTC+8（16:10 UTC）、2026-08-21T01:35:38Z（UTC+8 2026-08-21 09:35）、2026-08-21 10:02 UTC+8、2026-08-23 05:26:26Z（UTC+8 13:26）、Posted 5 Jul、Updated 17 Aug 09:22、Aug 15、22.05 kHz、2026-09-06、2026-09-13（src: 各 Claim Pack + CHANGELOG + runbook L6/L20）
- **授权边界与外部状态边界（逐字）**：
  - 「本地 `commit` 不需要授权；**远端 push / PR / merge / tag / Release / Workshop 上架**都需要维护者明确授权」（src: docs/release-runbook-zh.md L4）
  - 「以同一作者 Update（后续版本绝不用 `Initial Workshop Upload`）」；「**绝不尝试登录态或编辑界面**（Steam 编辑 = 维护者人工操作）」（src: docs/release-runbook-zh.md L56、L58）
  - 「页面级核验 ≠ 玩家下载内容已验证」；「新增一条 = 一次维护者裁决」「历史/tag 重写另行授权」；「工作树干净不蕴含历史干净」（src: docs/release-runbook-zh.md L97、L71、L69）
  - 「任何渠道都不再手工逐项核验」「新检查一律先落脚本再写进本文；不再新增人工清单」（src: docs/release-runbook-zh.md L18、L28）
  - Workshop item ID / PublishedFileId 数值：多份源含之，**本包按隐私硬门一律不复制**（DEC-11 隐私申报；src: docs/release_review/release-0.2.0-review-zh.md L53 等五处）
  - 外部先例锚：「UniversalSqueaker 2026-09-06 维护者裁决」；页面标题事实 `Squeaky Ratkin` / `鼠辈啁啾`；术语禁区 `rat-rats`、`mousefolk`、`Masterpiece`；`AI-Generated Work Disclosure`、`Hide The Book of Squeakudges on a high stool!`、`adorable little mousie` / `mousies`（src: docs/release-runbook-zh.md L20；docs/steam-workshop-page-copy-draft.md L14–16、L40、L126、L154；docs/release_review/release-0.2.1-review-zh.md L40；docs/release_review/release-0.2.4-review-zh.md L32）
  - C 锚点引用（账本号在 PKG-2，本包只登记其发布面出现）：`C12`、`C13`、`C17`、`C18`、`C19`、`C34–C40`、"八面门槛"、"决策文档 §5"（src: docs/release_review/release-0.3.0-review-zh.md L33–35；docs/release_review/release-0.3.2-pre1-review-zh.md L20、L24）
  - C34–C40 链可见范围（本包主源内逐字登记）：载体 = `release-0.3.2-pre1-review-zh.md` L20（分支状态"`0.3.x` 已推送（C34–C40）"）与 L24（"0.3.x 提交链：C34 身份门控 → C35 彩蛋/身份日志 → C36 作者指南/SKILL+脚手架+ABI 锁 → C37 legacy 桥原型 → C38 0.3.2 版本/changelog/docs → C39/C40 fixtures 行尾修复（corpus LF、expected/input 跟随宿主）"）；C12/C13/C17/C18/C19 的逐字短语载体 = `release-0.3.0-review-zh.md` L35。派发口径"C1–C19"的偏差（PKG-2 主源止于 C14、C15+ 载体散见本包主源）由编排方在 coverage-check 消解（见 §13-9）。C38 与 commit `7c3a956` 的对应关系为（调度方指针），本包按规则未做仓外取证。


----- 11. 证据缺口（GAP-<n>） L606-620 -----
## 11. 证据缺口（GAP-<n>）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
|---|---|---|---|
| GAP-1 | 0.1.0 / 0.1.1 的 tag、zip、哈希、Workshop 首次上传包身份 | Claim Pack 制度 0.2.0 后才有；CHANGELOG 只有日期与要旨 | 早期 GitHub release 页/Workshop 后台历史快照（外部归档）。src: docs/CHANGELOG.md L141–202 |
| GAP-2 | Steam Change Notes 6 条的实际内容与各自对应事件 | 页面观察只计数不抄录 | 一次带日期的 change notes 列表页面复采。src: docs/release_review/release-0.2.1-review-zh.md L43；docs/release_review/release-0.2.4-review-zh.md L32 |
| GAP-3 | 0.2.2 / 0.2.3 的 Workshop 真实状态（OQ-1/OQ-2 合并） | 一版只有所谓"报告"、一版记录未上传但计数矛盾 | 同 GAP-2 + item 版本历史（若有）。src: docs/release_review/release-0.2.2-review-zh.md L22；docs/release_review/release-0.2.3-review-zh.md L29 |
| GAP-4 | Steam 页面展示时间的时区定义 | 语料未定义（I-3 仅为折算推断） | 一次同时记录 GitHub 时间 + 页面 Updated + 观察者时钟的对照观察。src: docs/release_review/release-0.2.4-review-zh.md L12、L32；docs/release_review/release-0.2.1-review-zh.md L12、L41 |
| GAP-5 | CHANGELOG 排序纪律的最终口径（改规则还是改文件） | 规则与正文并存且互相否定（CNF-1），无裁决记录 | 维护者裁决一条。src: docs/CHANGELOG.md L8、L39–202；docs/CHANGELOG.zh-CN.md L8、L39–201 |
| GAP-6 | UniversalSqueaker 2026-09-06 裁决的原文与"最小仪式"对应条款 | 引用了裁决但语料不含该文 | 跨文档授权：引入 PKG-5/语料外文件后重开。src: docs/release-runbook-zh.md L20 |
| GAP-7 | 0.3.3 发布是否沿用 pre1 的哪些证据与待办 | pre1 待办写于改道前（CNF-6），0.3.3 面只有 CHANGELOG | 0.3.3 发布会话产物（未来事件）；当前可向 [xref: PKG-6（0.3.3 交接）] 要交接计划。src: docs/release_review/release-0.3.2-pre1-review-zh.md L27；docs/CHANGELOG.md L39–57 |
| GAP-8 | 8000 字符上限的权威来源 | 源文件措辞"常见的…以内"，无出处 | Steam 官方限制文档或后台实测。src: docs/steam-workshop-page-copy-draft.md L10 |
| GAP-9 | 0.2.0 "artifact manifest"字段指什么、去向如何 | 一次性出现，模板与后续实例均无该词 | 0.2.0 发布会话的原始记录（若有）。src: docs/release_review/release-0.2.0-review-zh.md L25 |
| GAP-10 | 本包隐私申报列出的 item ID 在多少文档中出现、各自字段名 | 本包已按形态描述并计数（5 处），但未逐处抄录（隐私硬门） | 不需要回答（隐私规则有意使其不可答）；仅供审计定位。src: docs/release-runbook-zh.md §隐私审查门禁 L66–73（本包 DEC-11 隐私申报定位五处源内出现） |

