# RimWorld 1.6 模组 UI 设计方法论

> **通用参考，非本项目产品合同。** 鼠辈啁啾的具体选择以 [`settings-ui-product-contract-zh.md`](./settings-ui-product-contract-zh.md) 和 [`project-architecture-contract.md`](./project-architecture-contract.md) 为准。
>
> 面向 RimWorld 1.6 C# IMGUI 模组开发者、编码 agent 与审阅者。本文只规范**模组自己拥有的 Window、设置页、工具页与对话框**。
>
> 融合来源（只读，路径级）：
> - `modding_documents/RimWorld_Mod_UI_UX_Guide_for_Coding_Agents_zh.md`
> - `modding_documents/rimworld-mod-engineering-skills/SKILL.md`
> - `modding_documents/rimworld-mod-engineering-skills/imgui-performance.md`
> - `modding_documents/rimworld-mod-engineering-skills/csharp-pitfalls.md`
>
> 本稿是上述材料与实际 UI 重构经验的**压缩重组**，不替代工程性能、C#/Mono、Scribe 或兼容性细则。外部材料并未因本文而修改。

## 1. 适用范围与证据层级

### 1.1 Scope

本指南适用于模组自己创建或控制的：

- `ModSettings` 页面；
- 自定义 `Window` / `Dialog`；
- 模组工具、浏览器、诊断面板；
- 模组内部复用的 IMGUI helper。

默认**不**通过 Harmony patch 改 RimWorld 本体或其他模组 UI，也不改变全局字体、纹理、控件或窗口外观。最外层窗口若由 RimWorld 拥有，应保留其关闭、拖动、输入和焦点行为。项目确需全局 UI patch 时，必须另立架构决策，写明兼容面、撤销方案与实机矩阵；不能把“统一风格”当作默认理由。

### 1.2 证据层级

| 层级 | 能证明什么 | 不能外推什么 |
|---|---|---|
| 官方资料 | 公开 API、版本范围、官方声明 | 未写明的视觉规范或私有 API 稳定性 |
| 目标版本源码/反编译 | 控件签名、调用形状、生命周期和原版实现模式 | “官方 design system”或跨版本承诺 |
| 目标版本实机 | 实际布局、点击、滚动、保存和玩家可观察结果 | 未覆盖版本、语言、DLC 或模组组合 |
| 项目约定 | 本项目接受的主题、语义、兼容和交付边界 | RimWorld 或其他项目的通用事实 |

引用原版模式时应写“目标版本源码观察”或“实机惯例”，不要称为 Ludeon 官方 design system。

## 2. 十四条硬原则

1. **UI 是状态解释器，不是字段编辑器。** 每个区块都要说明当前有效状态、来源、可执行动作和生效时机。
2. **顶层按玩家任务组织。** 不按 C# 类、Scribe 字段或开发阶段建页；通常保持 3～5 个稳定任务入口。
3. **持久设置不能伪装成一次性命令。** Toggle、Selector、Tab 与 Button 必须有不同形态和组件契约。
4. **层级不能倒置。** 普通选择器不得使用比主提交动作更大、更重的强调；大面积高强调按钮会让列表看起来像命令面板。
5. **状态必须多通道表达。** 主题映射、文字、形状/指示线、位置、禁用和 tooltip 至少组合两种；主题色绝不是唯一状态通道。
6. **所有视觉值来自 token。** 页面不得各自复制 surface、spacing、typography、border 和 interaction-state 常量。
7. **先分 Rect，再画控件。** 宽度不足时明确堆叠；禁止负宽度、重叠和只适配一张截图的比例。
8. **长文本必须可恢复。** 可见区域允许截断，但完整文本须通过 tooltip、详情或复制入口获得；关键原因不能只藏在 tooltip。
9. **组件只返回意图。** 通用 helper 不写设置、不 Scribe、不扫描 Def、不播放声音，也不自行提交事务。
10. **状态生命周期必须唯一。** 立即生效、draft、Apply、Revert、close-save、preview 和 UI preference 不得混用。
11. **可选内容失败安全。** 无 DLC、空结果、orphan 与解析失败是四种不同状态，必须分别呈现和回退。
12. **Draw 路径以零分配为目标。** 不在每帧扫描、反射、排序、LINQ、创建集合/委托或拼装大字符串；用 cache、dirty revision 与 viewport。
13. **危险操作是事务。** 变更只在最后确认回调中提交；取消、关闭和中间确认不得提前修改 canonical 状态。
14. **编译通过不是 UI 完成。** 必须补齐目标语言、UI scale、宽度、数据状态和保存 roundtrip 的实机证据。

## 3. 与具体颜色脱钩的 design tokens

Token 表达**语义**，具体映射由项目主题决定。不得在通用规范中规定 RGB、十六进制值或某一种固定色相。

### 3.1 Token 分类

| 分类 | 推荐 token | 语义 |
|---|---|---|
| Surface | `surface.base / raised / emphasized / overlay` | 页面、卡片、重点区域、对话框层级 |
| Text | `text.primary / secondary / muted / disabled` | 信息优先级，不代表业务成功或失败 |
| Intent | `accent / primary / danger / warning / success` | 当前选择、主操作、危险、警告、成功 |
| Border | `border.default / strong / focus / disabled` | 边界、焦点和不可用状态 |
| Spacing | `space.xs / sm / md / lg / xl` | 统一 padding、gap 和区块节奏 |
| Size | `control.height / row.compact / row.regular` | 控件与列表密度 |
| Typography | `title / section / body / meta` | 使用 RimWorld 现有字体档位建立层级 |
| Interaction | `idle / hover / pressed / focused / selected / disabled / dirty` | 每个可交互组件的状态集合 |

### 3.2 使用规则

- `accent` 只承担小面积 active、focus 或 dirty 指示，不铺满普通列表。
- `primary` 只给当前页面最重要的提交动作；同一区域通常只有一个。
- `danger` 表示潜在破坏性，不等于“更醒目的普通按钮”。
- `warning` 与 `success` 必须同时有准确文字或图标/形状，不能只换主题映射。
- `disabled` 要降低层级并阻止点击，同时给出不可用原因和恢复路径。
- IMGUI 没有可靠圆角时，使用稳定 surface、留白、细边框和指示线即可；不要为了网页感引入额外纹理。

## 4. 组件 API 契约

### 4.1 契约模板

每个共享组件在实现或评审时填写：

```text
组件名：
输入：Rect、显示值、状态、语义 kind、tooltip/disabled reason
输出：clicked / changed / selected value；不得隐式提交业务状态
高度：固定、测量公式、窄宽变体
禁用：是否阻止输入；原因在哪里可见
截断：何时截断；完整文本如何恢复
副作用：允许的仅为绘制与返回用户意图
分配约束：Draw 中不得创建的引用对象；缓存及失效点
验证：idle/hover/pressed/focus/selected/disabled + EN/SC + scale
```

### 4.2 推荐组件语义

| 组件 | 契约重点 |
|---|---|
| Button | `Primary/Secondary/Danger/Ghost` 语义固定；点击才返回命令意图 |
| Toggle | 表示持久布尔状态；整个行可点击；label 与当前值清楚 |
| Selector | 表示持久枚举/范围选择；显示当前值和展开 affordance，不像执行命令 |
| Tab | 只切换视图；active 不是“提交成功” |
| Card | 承载层级或对象摘要；可选时需 selected 状态和完整 tooltip |
| Search | 保留 RimWorld 文本输入与焦点；focus 可见；查询变化才使 filter cache dirty |
| Chip | 轻量筛选；明确同维 OR、跨维 AND 等组合语义 |
| Status | 中性、warning、success 等必须配准确状态文本和证据级别 |
| Empty | 区分无能力、空结果、失败和未选择；给下一步，而非只写“没有” |
| ActionBar | 固定 draft 状态、Apply/Revert；dirty 与 clean 占位稳定 |
| Dialog | 明确标题、正文、确认/取消位置、危险级别与关闭行为 |
| Input | 保留焦点、选择、复制粘贴；非法中间文本不能污染 canonical 值 |
| SliderField | slider 与 numeric buffer 同步；禁用与范围说明一致 |

### 4.3 Helper signature 示例

```csharp
internal enum UiButtonKind { Primary, Secondary, Danger, Ghost }
internal enum UiSurfaceKind { Base, Raised, Emphasized, Warning, Success }

internal static bool Button(
    Rect rect, string label, UiButtonKind kind,
    bool enabled = true, string disabledReason = "");

internal static bool Toggle(
    Rect rect, string label, ref bool value,
    bool enabled = true, string disabledReason = "");

internal static bool Selector(
    Rect rect, string currentLabel, bool active,
    string tooltip = "");
```

页面层负责：构造 `FloatMenu`、修改 draft、发起确认、Apply 和刷新 runtime。Helper 只绘制并返回输入意图。

## 5. 任务 IA 与 Rect 响应式

### 5.1 信息架构

- 全局行为、内容调制、逐对象配置等按玩家任务拆分。
- 数十个对象使用 master-detail；不要把对象选择和复杂编辑混进全局页。
- 页面先显示当前状态/来源，再给操作；内部 `defName`、stable key 放次级行或 tooltip。
- 相互关联但分别保存的设置可以视觉成组，但必须保留各自 selector、dirty 和持久化键。

### 5.2 Rect 规则

```csharp
bool narrow = rect.width < layoutBreakPoint;
Rect header = new(rect.x, rect.y, rect.width, measuredHeaderHeight);
Rect body = new(rect.x, header.yMax + gap, rect.width,
    rect.yMax - header.yMax - gap);

if (narrow)
{
    DrawMaster(StackTop(body));
    DrawDetail(StackBottom(body));
}
else
{
    DrawMaster(LeftColumn(body));
    DrawDetail(RightColumn(body));
}
```

执行规则：

- breakpoint 由最小可用内容宽度决定，不由某张截图决定。
- 稳定双列表单在宽屏使用 label + control；窄屏变为 label 在上、control 在下。
- master-detail 宽屏并排，窄屏上下堆叠；给 scrollbar 留固定 gutter。
- `Text.CalcHeight` 使用实际宽度；语言或宽度变化时使高度缓存失效。
- 页面优先一个主 scroll；嵌套 scroll 必须有清楚的空间边界和输入归属。
- 截断前先关闭 `Text.WordWrap`，绘制后恢复；仅在实际溢出时注册完整 tooltip。

## 6. 设置状态生命周期

### 6.1 状态类型

| 类型 | 写入时机 | UI 表达 |
|---|---|---|
| immediate | 控件改变后立即发布 | 不显示虚假的 Apply |
| canonical saved | Scribe 的正式值 | 标明当前有效来源 |
| deep draft | 编辑时只改副本 | 固定 ActionBar 显示 dirty/clean |
| Apply | 校验、canonicalize 后原子替换 | 唯一 Primary 操作 |
| Revert | 从 canonical 重建 draft | 无 runtime 副作用 |
| preview | 只读取 draft 发起试听/预览 | 明示未保存，不写生产状态 |
| UI preference | tab、scroll、折叠等 | 与行为配置分离 |

### 6.2 数据流与提交

```text
defaults -> canonical settings -> runtime snapshot
                  |
                  +-> deep draft -> validate/canonicalize -> Apply
                  +-> UI preferences
```

提交顺序：解析输入 → 校验/夹紧 → 去重与稳定排序 → 语义 dirty 比较 → 替换 canonical → 发布 runtime → 重建 draft baseline。

关窗策略必须明确选择一种：自动提交、Apply/Discard/Cancel、关窗丢弃，或全部 immediate。不要同时显示 Apply 又静默提交未应用草稿，除非页面明确写出并经过产品确认。

Scribe key/default 是兼容契约。复杂集合优先显式 `IExposable` record；加载后修复 null、非法 enum 和旧缺省，但不得把当前未发现的保存键无理由删除。

## 7. 可选 DLC、Catalog 与四态

可选内容只能是基础 UI/runtime 上的 delta。进入 DLC 类型、DefDatabase 或 pawn 路径前先检查能力；无能力时仍应能打开和保存基础设置。

| 状态 | 定义 | UI 与回退 |
|---|---|---|
| unavailable | DLC/能力不存在 | 禁用相关入口，说明需求；基础功能继续 |
| empty | 扫描成功但无候选 | 空态、计数和刷新条件 |
| orphan | 保存键存在但当前未发现 | 保留并标记；允许等待恢复或显式移除 |
| failed | 扫描/反射/解析失败 | 显示失败摘要，安全回退；不能伪装成 empty |

Catalog 推荐发布不可变 snapshot。由 snapshot 构造 row summary：stable key、localized label、技术名、来源、状态 flags、search text。filter cache 至少在 catalog revision、语言、查询、chip 和影响状态的 draft 变化时失效。

大量列表只绘制 viewport 可见行；稳定行高可直接计算首尾索引，动态行高则缓存 y offset/height 后定位。

## 8. 原生控件与现代层级

现代化不是网页化。可以保留以下原生控件作为底层：

- `Widgets.TextField` / `TextFieldNumeric`：输入焦点、选择与复制粘贴；
- `Widgets.HorizontalSlider`：拖动习惯；
- `BeginScrollView/EndScrollView`：滚轮和滚动条；
- `FloatMenu`：轻量选择；
- 原生窗口 close、拖动和外层控制。

它们应被放入统一 surface、spacing、border、disabled/focus 状态中，而不是裸露在现代卡片之间。不要为了让最外层 Close 与模组内容“完全一致”而 patch RimWorld 窗口。

## 9. IMGUI 性能预算

`DoWindowContents`、`Draw`、`CalcHeight/Width` 都是热路径，目标是零 GC 分配和稳定帧时间。

Draw 中禁止：

- 全库 Def 扫描、文件 IO、Scribe、反射发现或声音播放；
- LINQ 排序/过滤/`ToList`，临时 `List`/`StringBuilder`/`GUIContent`；
- 每帧 lambda/闭包、正则、循环字符串拼接；
- 每帧重建搜索索引、行摘要和内容高度；
- 大列表全量绘制。

推荐：

- 数据变化时置 `dirty`，在明确定义的失效点重建 cache；
- 复用集合、buffer、`StringBuilder` 和反射结果；
- 热路径遍历 `List<T>` 优先 `for`，避免依赖老 Mono 的枚举器行为；
- 显式 `Refresh/Resolve/Preview/Copy` 承担昂贵或有副作用的命令；
- 用 RimWorld profiler/外部 profiler 检查 Draw 的 GC.Alloc 和耗时。

性能细节、边界和检测方法以融合来源中的 `imgui-performance.md` 与 `csharp-pitfalls.md` 为准。

## 10. i18n、焦点与可访问性

- 玩家可见文字走 Keyed；语言文件 key 集合一致，参数化翻译代替句子拼接。
- 测 EN、SC 和可用的长语言；按钮宽度不按英文短词固定。
- focus 必须可见，但不能只靠主题映射；输入光标和底层 TextField 行为保留。
- 截断文字提供完整 tooltip；技术标识应可查看或复制。
- disabled 同时阻止输入、降低层级并说明原因/恢复路径。
- selected、dirty、warning、success 同时使用文字与形状/位置/图标等通道。
- tooltip 是补充，不承载唯一关键说明。
- 不宣称普通 RimWorld IMGUI 自动支持 screen reader；若项目有无障碍目标，应独立调研并验证。

## 11. 危险操作与自有 Dialog

危险流程先在 draft 中准备，最终确认回调才提交：

```text
request change
  -> first explanation (optional)
  -> final explicit confirm
  -> commit canonical + publish runtime
cancel/close at any earlier step -> no mutation
```

- 普通变更单确认；高风险且易误触时才双确认。
- 最终确认文案描述玩家结果，不展示内部函数名。
- 确认、取消的左右位置和键盘行为必须实机验证。
- 自有 Dialog 应复用模组 design tokens；若使用原生 Dialog，则保持原生完整交互，不做半套覆盖。
- 视觉成组不等于数据合并：一组中的两个 selector 可以分别持久化、分别提交。

## 12. 证据词汇与诚实状态

UI 状态不得超过可证明事实：

```text
Referenced -> Indexed -> Candidate -> Resolved
-> Selected -> Eligible -> Dispatched -> Observed
```

“已派发”不等于玩家已听到/看到；“Def owner”不等于资源作者；“已索引”不等于当前地图可用。无法观察时写“未观察”“需解析”或“需实机验证”。项目约定不得伪装成官方事实。

## 13. 验证矩阵

### 13.1 静态与行为验证

- 目标配置 build 通过，按项目政策处理 warning；`git diff --check` 通过。
- Begin/End 对称；恢复 `GUI.color`、`GUI.enabled`、`Text.Font`、`Text.Anchor`、`Text.WordWrap`。
- helper 无业务副作用；Draw 无扫描、反射、写盘、自动 Resolve/Preview。
- draft 为 deep copy；dirty 为语义比较；Scribe roundtrip 保留 orphan。
- Keyed 无重复/缺失；所有禁用原因和截断 tooltip 可达。

### 13.2 最小实机矩阵

| 维度 | 至少覆盖 |
|---|---|
| UI scale | 100%、125%、150% |
| 语言 | EN、SC；有条件增加更长语言 |
| 宽度 | 常规、窄宽/高缩放触发的堆叠 |
| interaction | idle、hover、pressed、focus、selected、disabled、dirty |
| 数据 | normal、unavailable、empty、orphan、failed |
| 保存 | 新设置、Apply、Revert、Cancel、Close、重开、重启 |
| Dialog | 单确认、双确认、最终提交、每层取消/close |
| 工具 | Search、Filter、Refresh、Resolve、Preview、Copy、scroll |
| 性能 | GC.Alloc、长列表 viewport、连续输入/滚动帧时间 |

截图至少审四遍：布局、状态、语言、证据真实性。截图不能替代点击、键盘、滚动、保存和运行时观察。

## 14. 反模式

| 反模式 | 替代方案 |
|---|---|
| 巨大高强调 selector 让持久设置像执行命令 | 低强调中性 selector + 小面积 active 指示 |
| 每个页面自带一套常量和按钮 | 集中 tokens + component helper |
| 全部按钮都 Primary | 每区最多一个主操作，其余 Secondary/Ghost |
| 只靠主题映射区分成功、危险或 disabled | 文字 + 形状/位置/图标 + tooltip |
| 宽屏三栏压缩到负宽度 | breakpoint 后明确纵向堆叠 |
| 相关设置为了排版被合并保存 | 视觉分组、数据与 Scribe 仍独立 |
| 裸 TextField/Slider 混在卡片中 | 保留原生输入底层，纳入统一 surface/focus/disabled |
| 为统一 Close 而 patch 原版窗口 | 尊重所有权边界，只统一模组自有区域 |
| Apply 与关窗静默提交并存 | 选定唯一生命周期并写清楚 |
| 无 DLC、empty、failed、orphan 都显示“无内容” | 四态建模与不同恢复路径 |
| Draw 中 LINQ、反射、排序和拼字符串 | cache + dirty revision + 显式命令 |
| build 通过即宣布完成 | 实机语言/scale/宽度/状态/roundtrip 矩阵 |

## 15. Definition of Done

只有同时满足以下条件，UI 才算完成：

1. 玩家能完成目标任务，并理解当前有效值、来源和生效时机。
2. design tokens 与组件契约集中，页面没有同类控件的风格漂移。
3. 常规与窄宽均无重叠、负宽度、不可达按钮或错误点击矩形。
4. immediate、draft、Apply、Revert、preview、close-save 和 Scribe 语义一致。
5. unavailable、empty、orphan、failed 均安全且可区分。
6. Draw 热路径达到项目性能预算，大列表有 cache/viewport 证据。
7. 状态不只靠主题映射，长文与禁用原因可恢复。
8. 危险操作在最终确认前没有 canonical 副作用。
9. 未 patch RimWorld/其他模组 UI；若有例外，存在单独签署的架构决策。
10. build、静态检查与实机矩阵均有记录；不可观察项明确标为待验证。
