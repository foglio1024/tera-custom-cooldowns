# TeraDpsMeterData
Data for Tera DPS meter

How to get opcode:
--------------------
GoneUp method: https://github.com/GoneUp/Tera_PacketViewer/tree/master/Opcode%20DLL

Gl0 updated dll - look [here](https://github.com/neowutran/TeraDpsMeterData/blob/master/copypaste-tuto/Gl0-opcodes.txt)

How to read the database: 
----------------------
https://github.com/gothos-folly/TeraDataTools

Decrypted Database repository:
----------------------
https://cloud.neowutran.ovh/index.php/s/6DikGNZC7oXhWvx

File formats:
----------------------
HotDot*.tsv:

AbnormalId	AbnormalType	HPChange	MPChange	Method	Time	Tick	Amount	Name	Itemid	ItemName	Tooltip	IconName


skills*.tsv:

SkillId	Race	Gender	Class	SkillName	Chained	SkillDetailedInfo IconName


pet-skills*.tsv:

HuntingZoneId	TemplateId	SummonName	SkillId	SkillName	IconName

note that game send pet skills id as (HuntingZoneId<<16 + SkillId)

glyph*.tsv

GliphId	SkillName	SkillId	SkillIconName	GlyphName	GlyphIcon	Tooltip

There are some issues in glyph*.tsv, skill Id and icon can be wrong or even absent, especially for non-english files
