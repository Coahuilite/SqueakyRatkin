# Source/

## Responsibility

C# 运行时源码根：整个 mod 的可执行逻辑都在此，唯一子目录 `SqueakyRatkin/` 是一个独立装配（net472，输出到 `1.6/Assemblies`）。本层不做架构决策——它只是装配边界；所有模块理解请向下导航。

## Design

- 单装配布局：`SqueakyRatkin/` 内含 `Mod.cs`（唯一入口与装配点）、运行时核心（resolver/catalog/policy/comp）、设置与数据模型，以及四个职责分离的子目录（`Patches/`、`UI/`、`Debug/`、`Logging/`）。
- 每一层有独立 codemap；子目录各自维护自己的细节，本文件只给入口与导航。

## Flow

- 构建流：`SqueakyRatkin.csproj`（`SQUEAKY_$(BuildFlavor)` 常量）编译 → `..\..\1.6\Assemblies`，运行时由 `1.6/` 资源层加载。
- 运行时流、数据/控制流细节见 [SqueakyRatkin/codemap.md](SqueakyRatkin/codemap.md) 的 Data & Control Flow。

## Integration

- **→ 1.6/**（`1.6/codemap.md`）：本目录的编译产物与 Def/XML 契约消费方；核心对 SoundDef 名（`SR_*`）、MoteDefs、ThingDef 上挂 `CompSqueaker` 的键入口在核心地图的 Integration 一节。
- **← SqueakyRatkin/**（[SqueakyRatkin/codemap.md](SqueakyRatkin/codemap.md)）：全部源码内容，含四个子目录地图链接（`Patches/codemap.md`、`UI/codemap.md`、`Debug/codemap.md`、`Logging/codemap.md` 均已填充）。

## Change Guidance

- 新增源码文件一律放 `SqueakyRatkin/` 下的对应职责目录；跨目录引用时遵守核心地图 Integration 中定义的扇入/扇出契约。
- 改动核心行为前先读 [SqueakyRatkin/codemap.md](SqueakyRatkin/codemap.md) 的 Change Guidance——resolver 主线程红线、保存防抖、序列化枚举 append-only 等约束在此层以上不可绕过。
