# 鼠辈啁啾 · Squeaky Ratkin

[English](./README.md) | **中文**

鼠辈啁啾(Squeaky Ratkin) 是一款轻量级音效模组，为鼠族(Ratkin)增加随心情变化的啁啾与吱声。空闲叫、吃饭、睡觉、受击、选中、移动、社交、娱乐、死亡都可以带上一点动静，并随摄像机距离自然变轻。

> 面向 Ratkin 种族的轻量、可选氛围增强。

> **3A 声明:**本模组由 AI 设计、AI 开发、AI 绘制。人类维护者负责审阅、打包与发布。

## 依赖
- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Humanoid Alien Races (HAR)](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
- [Ratkin / 鼠族 (NewRatkin)](https://steamcommunity.com/sharedfiles/filedetails/?id=1578693166)

## 安装
- **Steam**:订阅创意工坊(发布后)。
- **GitHub Release**:下载 release zip,解压到 `RimWorld/Mods/SqueakyRatkin/`。

## 音频素材
默认音源为原版豚鼠声音组。要使用自定义鼠叫:
- 将单声道文件放入 `1.6/Sounds/Squeak/<Action>/`,文件名 `SR_<Action>_<n>.ogg`。
- 推荐 ogg;wav(16-bit)也可。采样率使用 22050 或 44100 Hz,峰值归一化到约 -3dBFS。
- 取消 `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml` 里对应 grain 的注释。
- 只需录制中性基准变体;心情差异由运行时音高与音量调制产生。
- 可在模组设置工作台中按自定义音频特性做补偿。

## 配置
核心行为由 `1.6/Patches/Ratkin_AddSqueakComp.xml` 数据驱动:actions 定义触发模式、冷却、概率与距离预设;moodMods 定义运行时音高与音量调制。

模组设置提供:
- 自定义音频全覆盖,可在混合原版兜底与纯自定义音频之间切换。
- 按游戏倍速放大触发冷却,默认开启,通过降低高倍速触发密度降噪,不压低单次音量。
- 按语言能力缩放触发频率,默认开启,语言能力受损会降低普通发声频率;死亡反馈只在器官性无声时静音。
- 全局触发间隔倍率。
- 距离音量衰减预设或自定义衰减区间。
- 心情调制工作台,可按心情覆盖音高、音量、精确输入、套用预设并预览。
- 可选的内置调试动作本地化,默认关闭;切换后重开调试动作菜单生效。

## 开发者菜单(开发者模式)
开发者菜单 → "Squeaky Ratkin":悬浮字开关与摄像机指示器开关。声音预览在模组设置工作台中进行。

## 许可
- **代码(C#/XML Defs/Patches)**:[Mozilla Public License 2.0](./LICENSE) —— 文件级 copyleft(修改须开源),项目级可与闭源代码组合。
- **音频素材**:自定义音频贡献者需自行声明其文件的许可与权利归属。原版豚鼠音频归 Ludeon 所有,本模组仅按 Ludeon 模组政策引用 defName/clipFolderPath,不分发原版资产。
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
