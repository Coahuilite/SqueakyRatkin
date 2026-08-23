namespace SqueakyRatkin;

// 薄继承 legacy root（唯一 legacy 类型）。字段全部继承自 canonical；
// Verse XmlToObjectUtils.SearchTypeHierarchy 沿层级填充继承的 public 字段，
// 因此 legacy XML 节点无需任何重复声明。US 0.3.x–0.4 期间禁止定义本类型（防与 SR DLL first-wins 冲突）。

public class SqueakVoicePackDef : UniversalSqueaker.SqueakVoicePackDef
{
}
