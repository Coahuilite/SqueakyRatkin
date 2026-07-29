# MEMORY

## 当前事实
- 项目为 RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；永久 `packageId` 为 `coahuilite.squeakyratkin`。当前 csproj 版本已提升为 `0.2.0`，changelog 仍为 Unreleased；未授权提交、推送、PR、合并、tag 或发布。
- 2026-07-28 已完成治理文档收敛：运行时、actions、resolver、VoicePack 与 Example 以 [`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md) 为准；设置 UI 以 [`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md) 为准；日志以 [`docs/logging-protocol.md`](./docs/logging-protocol.md) 为准；第三方 VoicePack 作者入口为 [`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md)。
- OGG 是正式 Example 唯一格式，也是第三方发布推荐格式；WAV 仅为兼容格式，不是推荐发布格式。

## 已接受的 0.2.0 边界
- 设置为响应式四页 shell；修改立即发布到运行时，约 350 ms 合并保存，无 Apply/Revert。Periodic 有非持久化确定性启动相位与可听鼠族数量缩放，External 豁免；诊断缓冲区保留最近 10 条成功正式 `PlayOneShot` 选择。
- 不扩展为任意 HAR 种族、基因级配置或第三方 Xenotype 冲突调解。No-DLC、品牌、Def、Example、构建和发布边界由现行合同及 `AGENTS.md` 规定。

## 交接
- 当前操作、授权与待办见 `TODO.md`；仅在历史冲突或明确请求时读 `OBLIVIONIS.md`。历史 `DEPRECATED_*` 文档和冷归档不能覆盖现行合同。
- 2026-07-28 人工测试发现心情音色页底部试听区不可达：手写内容高度漏算覆盖开关行后，`Listing_Standard` 自动换到横向不可见列。现已统一让四页设置 ScrollView 的 Listing 保持单列，并对账修正普通规则、心情音色、异种行为和开发诊断页的已确认高度误差。
- 0.2.0 代码/UI/Xenotype 修复已提交至 `dev`：`a857c63`、`526b86a`、`6f52849`。作者音频更新从 clean `6f52849` 后独立进行，尚未提交。
- 2026-07-29 运行现场确认：HAR discovery 曾把官方 `Sanguophage` 作为独立候选展示；卸载来源模组后，持久化的 `Ratkin_HouseMouse` 行为 preset 仍以不可用目标保留。现已过滤 HAR-only 的 Core/Ludeon 官方候选；显式 preset/selection/declared pack 仍可保留同一目标。不可用目标新增带确认的“忘记此目标”，精确删除其行为 preset 与 Xenotype VoicePack selection，刷新目录仍不静默删档。Dev-only `xenotype.discovery.candidate` 现在记录最终候选来源组合或 `har_official_filtered`，使这两条路径可由 Player.log 区分。
- 2026-07-29 发现成功派发记录开启后，结果悬浮字仍被 `Prefs.DevMode` 二次过滤；Debug Action 本身已由 RimWorld 开发者菜单门控，因此该重复门控会造成记录与悬浮字状态分叉。现已移除二次检查：正式派发结果悬浮字仅随成功派发记录开关及 Pawn 在地图内状态工作；详细日志仍独立。
- `SqueakLog` 架构审查结论：尚非 God class，但已是单文件日志子系统，风险 3/5。0.2.0 发布前不做大拆；当前只补齐 `xenotype.discovery.candidate` 的封闭协议。发布后在 characterization checks 保护下再考虑拆分 internal event registry、build identity、once registry、srdiag formatter/text 与 Verse sink，同时保留唯一 public typed facade。
- 2026-07-29 在 `6f52849` clean HEAD 后开始独立音频更新。用户随后指定 dirty Dev 包 built-in tree 的完整 41 条集合作为权威交付，正式合同为 15 动作/41 OGG：Attack 3、Call 4、Death 2、Draft 3、Eat 2、Equip 2、Joy 3、MentalBreak 1、Move 3、Select 3、Sleep 3、Social 3、Undraft 3、Work 3、Wounded 3。25 条合规 Vorbis/22050/mono 原字节复制，16 条 FLAC-in-OGG/44100/stereo 经 FFmpeg 规范为 Vorbis/22050/mono；全部可完整解码。Example 音频是代码 MPL-2.0 之外的公共领域素材，项目与贡献者不主张版权或相关权利，适用范围与免责声明由 `AUDIO_RIGHTS.txt` 记录。
- 2026-07-29 修复成功派发诊断状态分叉：Debug Action 的菜单可用性已受 RimWorld 开发者模式门控，开启“Record successful dispatches”后，正式派发的短暂结果悬浮字仅再要求 Pawn 在地图内，不再重复检查 `Prefs.DevMode`；详细日志仍独立。
- 旧本地 Mods 安装副本（ProductVersion `0.2.0+26d6fc1…`）共有 34 个 OGG：内置 `SR_OfficialExample_Race` 17 个、随包 Template 17 个，WAV/MP3 均为 0；因此旧版实际播放池是 17 条，34 是包含字节镜像 Template 的物理总数。
- 41 条正式音频已写入唯一 Template 源，当前工作树未提交。Dev/GitHub 测试包均已重建为 115 文件，实际播放池 41 条、物理音频 82 条（内置 41 + Template 41）；精确动作分布、相对键、镜像 SHA-256、Vorbis/22050/mono、内容白名单与隐私检查通过。Dev DLL SHA-256 `BFD0587F1C0D1D281C402746CA507123AAB7D9724F2380F33E788D1780DEEC1E`；GitHub ZIP SHA-256 `78137213B8CDBDC32ADCBE21F7D4966A0DF4120730272BCCFB5CF5CEDFD75748`。
- 最新 `Player.log` 的 `audio.dispatch.ok` 覆盖为 5/15：`Call`、`Move`、`Select`、`Draft`、`Undraft`；未见 `audio.dispatch.failed`、`audio.dispatch.no_sound` 或 `trigger.attempt.failed`。尚无成功派发证据的 10 项为 `Eat`、`Sleep`、`Wounded`、`Social`、`Joy`、`Death`、`Attack`、`Work`、`Equip`、`MentalBreak`。该证据只确认 SoundDef 已向 Verse 请求播放，不能确认听感，也不能区分同一 SoundDef 随机选中的 `_01`/`_02`。
- 最新人工结果：官方 `Sanguophage` 已消失，不可用目标可明确忘记，当前窗口下设置 UI 目测可达。此前一次新游戏初始化前的原版 `TickManager.TicksAbs` 防御红字在最新完整会话中未复现，之后新游戏与多种动作发声持续正常，故降为不阻断 0.2.0 的低优先级观察项。另发现退出设置后七击开发入口的未完成计数未归零，作为后续低风险修复保留。
- 七击计数归零、`TicksAbs` 归因和 `SqueakLog` 内部拆分均明确推迟到 0.2.0 之后的版本，只保留在 `TODO.md`，不再扩展本次发布范围。
- 2026-07-29 用户确认当前 0.2.0 测试版本已经完成测试并接受进入发布前收口；此前音频听感、UI、异种、日志和成功派发悬浮字等人工复测项不再作为未完成测试阻塞。尚未提交的 41 条音频、悬浮字门控修复及相关文档/合同仍须在发布前卫生阶段统一审阅和提交。
- 0.1.x 发布历史对照确认，0.2.0 仍需完成而容易遗漏的事项：中英文 README 与 csproj 版本须一致；changelog 在真正发布时须写实际 UTC+8 时间；工作树提交后须从 clean HEAD 重建正式 flavor 产物；之后按授权顺序完成 push `dev`、PR/squash 到 `main`、仅在 main squash commit 打发布 tag、push tag、确认 CI GitHub Release，再构建 Steam flavor 并更新既有 Workshop 条目。0.1.x 的首次 Workshop 上传、17-OGG 旧合同和过时 Gate 不得复制。
- Workshop 更新规则：`pack-steam.ps1` 只 stage，且故意不含 `PublishedFileId.txt`；既有 item 必须从本地 RimWorld Mods 上传副本（非订阅缓存）以同一作者账号 Update，ID 只短暂写入该副本、不得进 Git/stage，并核对同一 item URL/ID、可见性、预览和 change note。`Initial Workshop Upload` 仅首次使用。证据入口：Steamworks Workshop 实现文档 <https://partner.steamgames.com/doc/features/workshop/implementation>；社区条目 URL 形如 <https://steamcommunity.com/sharedfiles/filedetails/?id=<item-id>>。
- 永久隐私规则：提交和完整待推送范围不得包含具体个人路径、本地安装/订阅状态、诊断原文、凭据、密钥、令牌或 `PublishedFileId.txt`；推送前同时审计最终 tree 与待推送历史。若真实秘密曾进入 commit，先撤销或轮换，再清理可达历史；清历史不能代替轮换。
- 发布仅能在获各步骤明确授权后按受保护的 `dev` → PR/squash → `main` → main tag → CI 进行。
