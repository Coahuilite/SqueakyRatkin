# Squeaky Ratkin Example Voices Template

This is a complete, independently enableable **Race-only Template** voice pack for Squeaky Ratkin.
Its packageId is `coahuilite.squeakyratkin.examplevoices`; its selectable PackDef is
`SR_ExampleTemplate_Race`. It has its own SoundDefs, PackKey/Catalog identity, and audio root, so
it neither patches, overrides, nor reuses the main mod's built-in Example Defs or paths.

`LoadFolders.xml` loads only `1.6/Race`. The `1.6/Biotech/` tree is TXT-only author guidance and
15 empty action-directory skeletons; it contains no loadable XML, no Xenotype PackDef,
`targetDefName`, Biotech load rule, or audio.

## Audio

The current 41 OGG Vorbis clips are 22050 Hz mono and use the 15 fixed actions: Attack 3, Call 4, Death 2,
Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3,
Work 3, and Wounded 3. These counts are the current reference baseline, not a fixed contract, and
may vary as the Example audio evolves. They are the maintained Template source mirrored into the
main mod's built-in Example during staging. These Example clips are public-domain material outside the
MPL-2.0 code license; the project and contributors claim no copyright or related rights in them,
and they may be used, copied, modified, and redistributed. See `AUDIO_RIGHTS.txt` for the limited
provenance/status disclaimer.

## Installation

Copy the `SqueakyRatkinExampleVoices` folder from this repository, or from the staged main mod's
`Extras/` folder, into the top level of RimWorld's `Mods` directory, then enable it.
