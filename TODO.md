# TODO

> 只列开放动作、阻塞与明确延后；已完成历史见 `OBLIVIONIS.md` 与 `docs/release_review/`。当前状态与证据边界以 `docs/maintenance-status-zh.md` 为准。

## 发布
- 0.3.3 已于 2026-09-19 发布到 GitHub（`v0.3.3` 正式 Release，资产核验通过）；记录见 Claim Pack `docs/release_review/release-0.3.3-review-zh.md`。分支模型已落地（`dev` 已删，本次走 `0.3.x` → `main`）。
- [ ] Steam：**未上传**；暂存包 `dist/steam/SqueakyRatkin` 已就绪（116 文件，build=steam/commit=c7fb868）。解除阻断时同步 `docs/steam-workshop-page-copy-draft.md`（版本号/下载链接/字符数）→ 维护者人工上传 → 页面只读核验 → 更新 Claim Pack 渠道格。
- [ ] 下一版开工时从 `main` 新建版本分支（`0.4` 或按路线直推 `1.0`）；`archive/` 分支只在最终版本建立。

## 待修复 / 待裁决
- [ ] R6（终审发现）：Remix 在「无 Xenotype 层 + Race + BuiltIn」组合可能选空层；公开 Select 入口已复现，源码无兜底。需修复 + 处理冻结语料取舍 + 实际游戏验证。
- [ ] R4/R5：日志 `pawn=<label>` 与旧隐私禁令冲突未裁决；普通字段只做编码、非通用脱敏，不得写成隐私例外。
- [ ] R3：作者 ABI 起点「首个携带 0.3.1 ABI 的发行版本」未精确到 tag。
- [ ] 实机余项（自然覆盖，不阻塞）：身份门控余项（倒地/敌对/访客/野化、任务加入、Draft/Equip/Attack/周期动作零变化）；Eat 三态矩阵；0.3.1 低频项（Remix、损坏设置失败路径、Config 副本手改、婴儿年龄调制听感、彩蛋开关无回归、No-DLC dormant）。
- [ ] 提案：音频包 XML 简化（指南级修正 / manifest→XML 生成器 / 运行时 SoundDef 生成），仅在第三方需求信号出现时推进。

## US 兼容与迁移（退役期硬前置）
- [ ] U1–U4 转述并落地：US 侧跨程序集 squeak-comp 检测与让位（U1 是发布硬前置）、US 是否服务 Ratkin、legacy 桥重叠期与类型名所有权、双开矩阵纳入两侧发布门。
- [ ] 迁移裁决 Q1–Q10：过渡版、legacy 渠道、设置导入、公告窗口、US 前置硬/软、跨版本维护预算、packageId 归属（推荐主线继承、legacy 新 id）、是否接受选择重置、新内容包 Def 类型。
- [ ] 双开实机矩阵：无/有 US 型 Ratkin 包、旧包经桥、事件与周期单响、双向卸载。

## 明确延后
- [ ] 证据退役：`docs/release_review/` 的 7 份 Claim Pack + 2 份流程复盘 + 终审报告留到下一次证据退役再评估删除或压缩。
- [ ] 历史/tag 隐私重写（方案 B）需单独授权；当前维持 known-debt 纪律。
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。
- [ ] Kiiro：分支已删，不再推进；未经作者许可不得发布或宣传相关兼容内容。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南、第三方 VoicePack 示例等旧候选方向需维护者重新确认后才恢复。
- [ ] US 仓库显示名与许可；US 0.4 是否服务 Ratkin（与 US 兼容 Q1 同源）。
