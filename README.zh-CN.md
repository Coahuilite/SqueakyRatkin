# 鼠辈啁啾 · Squeaky Ratkin

[English](./README.md) | **中文**

让鼠族(Ratkin)在空闲叫、吃饭、睡觉、受击、选中、移动、社交、娱乐、死亡时,按心情状态(好/中/坏/崩溃)发出吱吱叫。屏幕外视野剔除以节省性能,镜头内通过 distRange 距离衰减自然变轻。

> 中文名「鼠辈啁啾」是个俏皮的谐音;模组本身是一个轻量、可选的氛围增强。

## 依赖
- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Humanoid Alien Races (HAR)](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
- [Ratkin / 鼠族 (NewRatkin)](https://steamcommunity.com/sharedfiles/filedetails/?id=1578693166)

## 安装
- **Steam**:订阅创意工坊(发布后)。
- **GitHub Release**:下载 release zip,解压到 `RimWorld/Mods/SqueakyRatkin/`。

## 音频素材
默认音源为原版豚鼠(啮齿类)。要使用自定义鼠叫:
- 放入 `1.6/Sounds/Squeak/<Action>/`,文件名 `SR_<Action>_<n>.wav`
- 要求:**单声道**、wav(16-bit)或 ogg、22050 或 44100 Hz、归一化到约 -3dBFS
- 取消 `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml` 里对应 grain 的注释即生效
- 心情差异靠运行时 pitch/volume 调制,只需录中性基准变体(每动作 2-3 个)
- 在模组设置「调制工作台」按音频特性差异拉 slider 补偿

## 配置
三层(底→顶):
1. **默认**(作者):`1.6/Patches/Ratkin_AddSqueakComp.xml` 的 `actions`/`moodMods`
2. **玩家 override**:模组设置「调制工作台」,勾选心情 override,滑块+输入框+预设(尖锐/中性/低沉/混乱)+预览
3. **音源开关**:`完全 override`(纯自定义 vs 混合原版 default)

另含全局触发间隔滑块、距离衰减预设/自定义控制,以及「按游戏倍速放大触发冷却」checkbox(默认开)。高倍速降噪通过降低现实时间触发密度实现,不再压低单次声音音量。选中反馈使用现实时间冷却,暂停时仍保持响应。

## 开发者菜单(开发者模式)
开发者菜单 → "Squeaky Ratkin":悬浮字开关 ×2。声音预览在模组设置工作台中进行。

## 许可
- **代码(C#/XML Defs/Patches)**:[Mozilla Public License 2.0](./LICENSE) —— 文件级 copyleft(修改须开源),项目级可与闭源代码组合。
- **音频素材**(`1.6/Sounds/Squeak/` 下自定义音频):All Rights Reserved(默认,待最终确认)。原版豚鼠音频归 Ludeon 所有,本模组仅按 Ludeon 模组政策引用 defName/clipFolderPath,不分发原版资产。
- **第三方依赖**(Harmony/HAR/Ratkin):归各自作者,遵循各自许可。

## 开发

### Junction(本地开发推荐)
让 `RimWorld/Mods/SqueakyRatkin` 指向本工作区根,编译即加载,免手动复制。`scripts/validate-junction.ps1` 每次构建前自动校验(缺失只警告,不阻断)。

**指定 RimWorld Mods 位置**(三选一,优先级递减):
1. 环境变量(推荐,持久):`$env:RIMWORLD_DIR = "<你的 RimWorld Mods 路径>"`
2. 脚本参数:`pwsh scripts/validate-junction.ps1 -wsRoot <项目根> -modName SqueakyRatkin`
3. 自动检测常见 Steam 路径

**手动建 junction**(脚本缺失时会打印这条命令,复制运行即可):
```powershell
New-Item -ItemType Junction -Path '<你的 RimWorld>\Mods\SqueakyRatkin' -Target '<本项目根>'
```

### 构建
```
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```
必须 0 errors。junction 缺失的 WARNING 正常(非阻断)。

### Build Flavor(分发用)
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub`,影响启动日志 banner(`[dev|steam|github]`)。运行时功能三态相同。启动日志中 dev 版按提交号区分,GitHub 版按 tag+提交区分,Steam 版只显示包版本号。

### 打包
- 先构建目标 flavor;打包脚本只整理/压缩已有构建产物。
- 本地测试:先构建 Dev flavor,再 `pwsh scripts/pack-dev.ps1` → `dist/dev/SqueakyRatkin/`。
- Steam 工坊:先构建 Steam flavor,再 `pwsh scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/`。
- GitHub Release:仅 CI/tag 流程构建 GitHub flavor,再 `pwsh scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip`。
- 内容只含 `About/`、`LoadFolders.xml`、`1.6/`(排除源码/pdb/文档)

## 分支与贡献
- `main`:发版分支(稳定;tag `v*` 触发 release)。
- `dev`:开发分支,PR 提到这里。

音频与代码贡献指南见 [`CONTRIBUTING.md`](./CONTRIBUTING.md)。

> 给 AI agent 的开发指引见 [`AGENTS.md`](./AGENTS.md)。
