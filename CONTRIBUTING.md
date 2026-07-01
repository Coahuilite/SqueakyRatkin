# 贡献指南 · Contributing

本模组欢迎音频与代码两方面的贡献，下文分别说明。
This mod welcomes both audio and code contributions; each is covered below.

---

## 音频贡献（无需编程）· Audio (no coding required)

模组默认采用原版豚鼠的叫声——近似啾啾，但并非真正的鼠叫。若您能录制或合成更贴近真实鼠类的吱吱声，欢迎用以替换默认音源。
The default audio is the vanilla guinea-pig — close, but not a real rat. Recordings or synth of something more rat-like are welcome as replacements.

**提交流程 / Submission**
1. 按 `1.6/Sounds/Squeak/AUDIO_GUIDE.txt` 的要求录制与命名（要点：单声道 ogg、22050 Hz、峰值约 -3dBFS）。
   Record and name per `AUDIO_GUIDE.txt` (mono ogg, 22050 Hz, peak ~-3dBFS).
2. 选择一种方式提交 / submit via one of:
   - 熟悉 Git：文件置于 `1.6/Sounds/Squeak/<Action>/`，向 `dev` 分支发起 Pull Request。
     With Git: place files in `1.6/Sounds/Squeak/<Action>/`, PR to `dev`.
   - 不熟悉 Git：开启 Issue 并附上音频文件。
     Without Git: open an issue and attach the audio.
3. 维护者会接入游戏配置，并在发布说明中致谢。
   A maintainer wires it in and credits you on release.

### 术语通俗解释 · Plain-language terms

若下列术语不熟悉，可参考此处的口语化说明。
If any term below is unfamiliar, here is a plain-language explanation.

- **单声道（mono）**：录制时不分左右声道。游戏通过单声道实现声音随距离衰减，立体声会破坏这一机制。
  Record without left/right split. Distance fading relies on mono; stereo breaks it.
- **ogg**：音频压缩格式，导出时选择即可。原版与多数音效模组均使用 ogg，兼容性最佳。请勿使用 mp3。
  An audio format. Vanilla and most sound mods use ogg — best compatibility. Avoid mp3.
- **采样率（22050 / 44100 Hz）**：决定清晰度。22050 Hz 已足够，44100 Hz 为 CD 音质，二者皆可。
  Determines clarity. 22050 Hz is sufficient, 44100 Hz is CD quality.
- **音量归一化（-3dBFS）**：使用音频软件的 Normalize 功能，将峰值调至 -3dB 左右，确保音量充足且不失真。
  Use Normalize to set peak around -3dB — loud, but not clipping.
- **去除静音（trim）**：裁剪首尾空白片段，避免游戏触发后出现延迟。
  Trim silence at start/end to avoid trigger delay.
- **中性录制（neutral）**：录制时无需刻意表现情绪，正常的一声吱叫即可。游戏会通过音高与音量自动呈现不同心情（高=开心，低=难过，乱=崩溃）。
  Record one neutral take. The game shifts pitch up (happy), down (sad), chaotic (breakdown) automatically.
- **SoundDef / grain**：游戏内部的音频接线配置，贡献者无需了解，由维护者处理。
  Internal wiring — handled by the maintainer.

太长不看：录制几声鼠叫（单声道、ogg、音量适中不爆音），按 `SR_动作_序号.ogg` 命名（例如 `SR_Call_1.ogg`），放入对应文件夹后提交，其余由维护者完成。
TL;DR: record a few rat squeaks (mono, ogg, not clipping/too quiet), name as `SR_<Action>_<n>.ogg` (e.g. `SR_Call_1.ogg`), place in the folder, submit. The maintainer handles the rest.

---

## 代码贡献 · Code

1. Fork 仓库并克隆至本地。Fork and clone.
2. 基于 `dev` 创建特性分支（请勿直接使用 main）：`git checkout dev && git checkout -b my-feature`
   Branch off `dev` (not main).
3. 确保编译通过：`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`，要求 0 错误。
   Build must pass with 0 errors.
4. 向 `dev` 发起 Pull Request，说明改动内容与原因。
   PR to `dev`, describe what and why.

## 构建 · Build
```
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```
本地开发环境搭建（junction 机制）请参阅 README「开发」章节。
Local dev setup (junction) — see README "Development".

## 编码规范 · Conventions
- Def 名称使用 `SR_` 前缀（避免全局数据库冲突）；C# 类名不加前缀（由命名空间隔离）。
  `SR_` prefix on Defs; no prefix on C# classes (namespace isolation).
- 所有面向用户的字符串须通过 Keyed 本地化，中英文同时提供。
  Localize user-facing strings via Keyed, both EN and SC.
- 请勿扫描项目根目录之外的路径（详见 AGENTS.md）。
  Do not scan paths outside the project root (see AGENTS.md).
- 代码采用 MPL-2.0 协议，贡献代码同样适用。
  Code is MPL-2.0; contributions are licensed likewise.

## 提交前自查 · Pre-submit checklist
- [ ] `dotnet build` 无错误 / 0 errors
- [ ] 无硬编码本地路径（`E:\`、`C:\Users\` 等） / no hardcoded local paths
- [ ] 新增字符串已提供中英文翻译 / new strings localized (EN + SC)
- [ ] 新增 Def 使用 `SR_` 前缀 / new Defs prefixed `SR_`
- [ ] 未将原版音频或贴图打包分发（仅以 defName 引用） / no vanilla assets bundled

## 分支模型 · Branches
- `main`：发版分支，保持稳定。推送 `v*` 标签触发自动发布。
  Release branch, stable. `v*` tags trigger automated release.
- `dev`：开发分支，所有 Pull Request 提交至此。
  Development branch; PRs go here.

## 联系方式 · Contact
通过 GitHub Issue 反馈。Via GitHub Issue.
