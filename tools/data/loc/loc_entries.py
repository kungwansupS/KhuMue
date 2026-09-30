# Boss / event / quest texts. Every leaf is (English, Thai).
# Fields: Short (one line in lists), Summary, Summon (how to start), Unlocks, Tips, Warning, Requires (shown while locked).
# A field with the suffix "Cal" replaces the base field while Calamity is loaded (vanilla entries whose rules Calamity changes).
# Facts come from the game code of the exact versions in Core/SupportedMods.cs and from the mods' own Boss Checklist texts;
# drops, recipes and shops are not written here because the guide reads them live.

E = {}

# ====================================================================== Terraria 1.4.4

E["KingSlime"] = {
    "Short": ("A giant slime; an easy first boss.", "สไลม์ยักษ์ บอสแรกที่ไม่ยาก"),
    "Summary": ("A giant slime that jumps, teleports and spits out smaller slimes. A good first boss.",
                "สไลม์ยักษ์ที่กระโดด วาร์ป และปล่อยสไลม์ตัวเล็กออกมา เหมาะเป็นบอสตัวแรก"),
    "Summon": ("Use a [item:Terraria/SlimeCrown]. It can also appear by itself in the outer parts of the world, or after 150 slimes are killed during a Slime Rain.",
               "ใช้ [item:Terraria/SlimeCrown] หรืออาจปรากฏเองในบริเวณริมโลก หรือหลังกำจัดสไลม์ครบ 150 ตัวระหว่าง Slime Rain"),
    "Tips": ("• Fight on flat ground with a few platforms above you.\n• Piercing or area weapons clear the small slimes it spawns.",
             "• สู้บนพื้นเรียบและมีแพลตฟอร์มด้านบนไว้หลบ\n• อาวุธที่ทะลุหรือโจมตีเป็นวงช่วยเคลียร์สไลม์ตัวเล็กที่มันปล่อยออกมา"),
}

E["TorchGod"] = {
    "Short": ("Secret event: too many torches underground.", "อีเวนต์ลับ: วางคบเพลิงใต้ดินมากเกินไป"),
    "Summary": ("A secret event: the Torch God attacks when you place too many torches close together underground. Survive it to get [item:Terraria/TorchGodsFavor].",
                "อีเวนต์ลับ: Torch God จะโจมตีเมื่อคุณวางคบเพลิงใกล้ ๆ กันจำนวนมากใต้ดิน รอดให้ได้เพื่อรับ [item:Terraria/TorchGodsFavor]"),
    "Summon": ("Place 101 torches close together while underground. The event starts when the last torch is placed.",
               "วางคบเพลิง 101 อันใกล้ ๆ กันขณะอยู่ใต้ดิน อีเวนต์จะเริ่มเมื่อวางอันสุดท้าย"),
    "Unlocks": ("[item:Terraria/TorchGodsFavor]: a toggle next to your inventory that turns placed torches into the torch of your current biome.",
                "[item:Terraria/TorchGodsFavor]: ปุ่มข้างช่องเก็บของที่เปลี่ยนคบเพลิงที่วางให้เป็นคบเพลิงของไบโอมที่คุณอยู่"),
    "Tips": ("• Do it in a small underground room with space to dodge the falling torch flames.",
             "• ทำในห้องเล็ก ๆ ใต้ดินที่มีที่ว่างพอให้หลบเปลวไฟจากคบเพลิงที่ตกลงมา"),
}

E["EyeOfCthulhu"] = {
    "Short": ("First main boss; drops your world's evil ore.", "บอสหลักตัวแรก ดรอปแร่สายชั่วร้ายของโลก"),
    "Summary": ("A giant eye that charges at you and becomes much faster in its second phase. The first step of the main route.",
                "ดวงตายักษ์ที่พุ่งชนใส่คุณและจะเร็วขึ้นมากในเฟสที่สอง เป็นขั้นแรกของเส้นทางหลัก"),
    "Summon": ("Use a [item:Terraria/SuspiciousLookingEye] at night. It may also come by itself at nightfall (1 in 3) once a player has 200 max life and more than 10 defense and at least 4 town NPCs live in the world.",
               "ใช้ [item:Terraria/SuspiciousLookingEye] ตอนกลางคืน หรืออาจมาเองตอนหัวค่ำ (โอกาส 1 ใน 3) เมื่อมีผู้เล่นที่มีพลังชีวิตสูงสุด 200 และพลังป้องกันมากกว่า 10 และมี NPC ในเมืองอย่างน้อย 4 คน"),
    "Unlocks": ("Drops Demonite or Crimtane Ore (matching your world's evil) for your first boss-tier gear.",
                "ดรอปแร่ Demonite หรือ Crimtane (ตามสายชั่วร้ายของโลก) สำหรับทำอุปกรณ์ระดับบอสชุดแรก"),
    "UnlocksCal": ("Drops Demonite or Crimtane Ore. In Calamity it also lets Acid Rain start: the first one begins when you enter the Sulphurous Sea. Blood Moon enemies start dropping Blood Orbs.",
                   "ดรอปแร่ Demonite หรือ Crimtane ส่วนใน Calamity ยังทำให้ Acid Rain เริ่มได้ ครั้งแรกจะเริ่มเมื่อคุณเข้าไปใน Sulphurous Sea และศัตรูใน Blood Moon จะเริ่มดรอป Blood Orb"),
    "Tips": ("• Build a long skybridge of platforms so you can run left and right.\n• In the second phase it dashes several times in a row; keep moving sideways.",
             "• สร้างสะพานแพลตฟอร์มยาว ๆ กลางอากาศเพื่อวิ่งซ้ายขวาได้\n• ในเฟสที่สองมันจะพุ่งหลายครั้งติดกัน ให้เคลื่อนที่ไปด้านข้างตลอด"),
}

E["BloodMoon"] = {
    "Name": ("Blood Moon", "Blood Moon"),
    "Short": ("Red night: more and stronger enemies.", "คืนสีแดง: ศัตรูมากขึ้นและแรงขึ้น"),
    "Summary": ("A night event: enemies spawn much more often, special Blood Moon enemies appear and fishing can pull up monsters. Zombies can open doors.",
                "อีเวนต์กลางคืน: ศัตรูเกิดบ่อยขึ้นมาก มีศัตรูพิเศษของ Blood Moon และการตกปลาอาจได้มอนสเตอร์ขึ้นมา ซอมบี้เปิดประตูได้"),
    "Summon": ("Happens by itself at the start of some nights (1 in 9, never on a new moon) once a player has used at least 2 Life Crystals. Or use a [item:Terraria/BloodMoonStarter] at night.",
               "เกิดเองตอนเริ่มกลางคืนบางคืน (โอกาส 1 ใน 9 และไม่เกิดในคืนเดือนมืด) เมื่อมีผู้เล่นใช้ Life Crystal อย่างน้อย 2 ชิ้น หรือใช้ [item:Terraria/BloodMoonStarter] ตอนกลางคืน"),
    "SummonCal": ("Happens by itself at the start of some nights (1 in 9, never on a new moon). Calamity raises the requirement: a player must have used at least 5 Life Crystals (200 max life). Or use a [item:Terraria/BloodMoonStarter] at night.",
                  "เกิดเองตอนเริ่มกลางคืนบางคืน (โอกาส 1 ใน 9 และไม่เกิดในคืนเดือนมืด) Calamity เพิ่มเงื่อนไข: ต้องมีผู้เล่นใช้ Life Crystal อย่างน้อย 5 ชิ้น (พลังชีวิตสูงสุด 200) หรือใช้ [item:Terraria/BloodMoonStarter] ตอนกลางคืน"),
    "Tips": ("• Fishing during a Blood Moon can catch dangerous but rewarding monsters.\n• Stay in a safe base: zombies can open doors.",
             "• การตกปลาช่วง Blood Moon อาจได้มอนสเตอร์ที่อันตรายแต่ให้ของดี\n• อยู่ในฐานที่ปลอดภัย เพราะซอมบี้เปิดประตูได้"),
    "TipsCal": ("• After the Eye of Cthulhu, Blood Moon enemies drop Blood Orbs, which can craft many potions.\n• In Death Mode, Blood Moons send endless waves of enemies.\n• Stay in a safe base: zombies can open doors.",
                "• หลังชนะ Eye of Cthulhu ศัตรูใน Blood Moon จะดรอป Blood Orb ซึ่งใช้คราฟต์ยาได้หลายชนิด\n• ในโหมด Death ช่วง Blood Moon ศัตรูจะมาเป็นระลอกไม่หยุด\n• อยู่ในฐานที่ปลอดภัย เพราะซอมบี้เปิดประตูได้"),
}

E["EaterOfWorlds"] = {
    "Short": ("The Corruption's worm boss.", "บอสหนอนของ Corruption"),
    "Summary": ("A huge worm made of many segments; hitting the body splits it into smaller worms. The Corruption's boss and a main-route step.",
                "หนอนยักษ์ที่มีหลายปล้อง ตีกลางลำตัวแล้วจะแยกเป็นหนอนตัวเล็ก เป็นบอสของ Corruption และเป็นขั้นในเส้นทางหลัก"),
    "Summon": ("Use [item:Terraria/WormFood] in the Corruption, or break Shadow Orbs in the Corruption chasms (every third orb summons it).",
               "ใช้ [item:Terraria/WormFood] ใน Corruption หรือทุบ Shadow Orb ในหุบเหวของ Corruption (ทุบครบลูกที่สามจะเรียกบอส)"),
    "Unlocks": ("Meteorites can now land and the Tavernkeep can move in. Its Demonite Bars and Shadow Scales make gear and a pickaxe strong enough for Hellstone.",
                "อุกกาบาตเริ่มตกได้และ Tavernkeep ย้ายเข้ามาได้ ส่วน Demonite Bar และ Shadow Scale ที่ได้ใช้ทำอุปกรณ์และ Pickaxe ที่ขุด Hellstone ได้"),
    "UnlocksCal": ("Meteorites can now land and the Tavernkeep can move in. Its Demonite Bars and Shadow Scales make gear and a pickaxe strong enough for Hellstone. Calamity's own Corruption boss, [e:HiveMind], is next.",
                   "อุกกาบาตเริ่มตกได้และ Tavernkeep ย้ายเข้ามาได้ ส่วน Demonite Bar และ Shadow Scale ใช้ทำอุปกรณ์และ Pickaxe ที่ขุด Hellstone ได้ บอสถัดไปคือบอส Corruption ของ Calamity: [e:HiveMind]"),
    "Tips": ("• Piercing and area weapons hit many segments at once.\n• Break the Shadow Orbs one at a time when you are ready: every third one summons the boss.",
             "• อาวุธที่ทะลุหรือโจมตีเป็นวงจะโดนหลายปล้องพร้อมกัน\n• ทุบ Shadow Orb ทีละลูกเมื่อพร้อม เพราะลูกที่สามจะเรียกบอส"),
}

E["BrainOfCthulhu"] = {
    "Short": ("The Crimson's brain boss.", "บอสสมองของ Crimson"),
    "Summary": ("A floating brain protected by Creepers. It cannot be hurt until its Creepers are gone, then it teleports around you. The Crimson's boss and a main-route step.",
                "สมองลอยได้ที่มี Creeper คอยป้องกัน จะทำดาเมจไม่ได้จนกว่า Creeper จะหมด จากนั้นมันจะวาร์ปรอบตัวคุณ เป็นบอสของ Crimson และเป็นขั้นในเส้นทางหลัก"),
    "Summon": ("Use a [item:Terraria/BloodySpine] in the Crimson, or break Crimson Hearts in the Crimson chasms (every third heart summons it).",
               "ใช้ [item:Terraria/BloodySpine] ใน Crimson หรือทุบ Crimson Heart ในหุบเหวของ Crimson (ทุบครบลูกที่สามจะเรียกบอส)"),
    "Unlocks": ("Meteorites can now land and the Tavernkeep can move in. Its Crimtane Bars and Tissue Samples make gear and a pickaxe strong enough for Hellstone.",
                "อุกกาบาตเริ่มตกได้และ Tavernkeep ย้ายเข้ามาได้ ส่วน Crimtane Bar และ Tissue Sample ใช้ทำอุปกรณ์และ Pickaxe ที่ขุด Hellstone ได้"),
    "UnlocksCal": ("Meteorites can now land and the Tavernkeep can move in. Its Crimtane Bars and Tissue Samples make gear and a pickaxe strong enough for Hellstone. Calamity's own Crimson boss, [e:Perforators], is next.",
                   "อุกกาบาตเริ่มตกได้และ Tavernkeep ย้ายเข้ามาได้ ส่วน Crimtane Bar และ Tissue Sample ใช้ทำอุปกรณ์และ Pickaxe ที่ขุด Hellstone ได้ บอสถัดไปคือบอส Crimson ของ Calamity: [e:Perforators]"),
    "Tips": ("• Kill the Creepers first; only then can the brain be damaged.\n• Area and piercing weapons clear the Creepers quickly.",
             "• กำจัด Creeper ให้หมดก่อน สมองจึงจะโดนดาเมจได้\n• อาวุธโจมตีเป็นวงหรือทะลุช่วยเคลียร์ Creeper ได้เร็ว"),
}

E["GoblinArmy"] = {
    "Name": ("Goblin Army", "Goblin Army"),
    "Short": ("Goblin invasion; frees the Goblin Tinkerer.", "ก็อบลินบุก ปลดล็อก Goblin Tinkerer"),
    "Summary": ("An army of goblins invades your town. Beating it lets you rescue the Goblin Tinkerer, who reforges items and sells the Tinkerer's Workshop for combining accessories.",
                "กองทัพก็อบลินบุกเมืองของคุณ ชนะแล้วจะช่วย Goblin Tinkerer ได้ ซึ่งรีฟอร์จไอเทมและขาย Tinkerer's Workshop สำหรับรวมเครื่องประดับ"),
    "Summon": ("Use a [item:Terraria/GoblinBattleStandard]. It can also arrive at dawn once a Shadow Orb or Crimson Heart has been broken and a player has 200 max life.",
               "ใช้ [item:Terraria/GoblinBattleStandard] หรืออาจมาเองตอนรุ่งเช้า เมื่อเคยทุบ Shadow Orb หรือ Crimson Heart แล้วและมีผู้เล่นที่มีพลังชีวิตสูงสุด 200"),
    "Unlocks": ("The Goblin Tinkerer can be found tied up in the Cavern layer. Talk to him to free him.",
                "จะพบ Goblin Tinkerer ถูกมัดอยู่ในชั้นถ้ำ คุยกับเขาเพื่อปลดปล่อย"),
    "Tips": ("• The event ends when enough goblins are killed; the progress bar shows how far you are.",
             "• อีเวนต์จบเมื่อกำจัดก็อบลินได้ครบตามจำนวน ดูความคืบหน้าได้จากแถบด้านบน"),
}

E["OldOnesArmy"] = {
    "Name": ("Old One's Army", "Old One's Army"),
    "Short": ("Defend the Eternia Crystal (Tavernkeep).", "ป้องกัน Eternia Crystal (Tavernkeep)"),
    "Summary": ("A tower-defense event: waves of enemies march toward the Eternia Crystal. Rewards are Defender Medals for the Tavernkeep's sentries and armor.",
                "อีเวนต์แนวป้องกันฐาน: ศัตรูเดินเป็นระลอกเข้าหา Eternia Crystal รางวัลคือ Defender Medal สำหรับซื้อ Sentry และชุดเกราะจาก Tavernkeep"),
    "Summon": ("Buy an [item:Terraria/DD2ElderCrystalStand] and an [item:Terraria/DD2ElderCrystal] from the Tavernkeep, place the stand and use the crystal on it.",
               "ซื้อ [item:Terraria/DD2ElderCrystalStand] และ [item:Terraria/DD2ElderCrystal] จาก Tavernkeep วางแท่นแล้วใช้คริสตัลกับแท่น"),
    "Unlocks": ("Tier 1 works any time, Tier 2 after any mechanical boss, Tier 3 after Golem. Each tier has stronger waves and better rewards.",
                "ระดับ 1 เล่นได้ตลอด ระดับ 2 หลังชนะบอสจักรกลตัวใดก็ได้ ระดับ 3 หลังชนะ Golem แต่ละระดับศัตรูแรงขึ้นและรางวัลดีขึ้น"),
    "Requires": ("The Tavernkeep moves in after the Eater of Worlds or Brain of Cthulhu is defeated.",
                 "Tavernkeep จะย้ายเข้ามาหลังชนะ Eater of Worlds หรือ Brain of Cthulhu"),
    "Tips": ("• Enemies go for the crystal, not you: build sentries along both lanes.\n• If the crystal breaks, the event ends.",
             "• ศัตรูมุ่งโจมตีคริสตัล ไม่ใช่คุณ วาง Sentry ไว้ทั้งสองฝั่ง\n• ถ้าคริสตัลแตก อีเวนต์จะจบทันที"),
}

E["DarkMage"] = {
    "Short": ("Old One's Army miniboss (Tier 1 and 3).", "มินิบอสของ Old One's Army (ระดับ 1 และ 3)"),
    "Summary": ("The miniboss of the Old One's Army's first and third tiers. It appears in wave 5 of Tier 1 and waves 5 and 6 of Tier 3.",
                "มินิบอสของ Old One's Army ระดับ 1 และ 3 ปรากฏในระลอกที่ 5 ของระดับ 1 และระลอกที่ 5 กับ 6 ของระดับ 3"),
    "Tips": ("• Focus it quickly so the other enemies do not overwhelm your lanes.",
             "• จัดการให้เร็วเพื่อไม่ให้ศัตรูตัวอื่นล้นแนวป้องกัน"),
}

E["QueenBee"] = {
    "Short": ("Jungle bee queen; optional.", "ราชินีผึ้งแห่ง Jungle ไม่บังคับ"),
    "Summary": ("A giant bee that charges across the screen and releases swarms of bees. Optional, but her drops are strong early gear.",
                "ผึ้งยักษ์ที่พุ่งข้ามจอและปล่อยฝูงผึ้ง ไม่บังคับ แต่ของที่ดรอปเป็นอุปกรณ์ช่วงต้นที่ดี"),
    "Summon": ("Use an [item:Terraria/Abeemination] in the Jungle, or break the larva inside a Bee Hive in the underground Jungle.",
               "ใช้ [item:Terraria/Abeemination] ใน Jungle หรือทุบตัวอ่อนในรังผึ้ง (Bee Hive) ใน Jungle ใต้ดิน"),
    "Tips": ("• Standing in honey gives regeneration.\n• Her charges are horizontal: dodge by moving up or down.",
             "• ยืนในน้ำผึ้งจะได้บัฟฟื้นพลังชีวิต\n• ท่าพุ่งของเธอเป็นแนวนอน หลบโดยขยับขึ้นหรือลง"),
}

E["Skeletron"] = {
    "Short": ("Guardian of the Dungeon; opens it.", "ผู้พิทักษ์ Dungeon ชนะแล้วจึงเข้า Dungeon ได้"),
    "Summary": ("A giant skull with two hands that guards the Dungeon. Defeating it opens the Dungeon and frees the Clothier.",
                "หัวกะโหลกยักษ์พร้อมมือสองข้างที่เฝ้า Dungeon ชนะแล้ว Dungeon จะเปิดและ Clothier จะเป็นอิสระ"),
    "Summon": ("Talk to the Old Man at the Dungeon entrance at night and choose to curse him. Or equip a [item:Terraria/ClothierVoodooDoll] and kill the Clothier at night.",
               "คุยกับ Old Man ที่หน้าทางเข้า Dungeon ตอนกลางคืนแล้วเลือกสาปเขา หรือสวม [item:Terraria/ClothierVoodooDoll] แล้วกำจัด Clothier ตอนกลางคืน"),
    "Unlocks": ("The Dungeon: its enemies, chests (Golden Keys) and the Mechanic, who is found bound inside.",
                "Dungeon: ศัตรู หีบ (ใช้ Golden Key) และ Mechanic ที่ถูกมัดอยู่ข้างใน"),
    "UnlocksCal": ("The Dungeon: its enemies, chests (Golden Keys) and the Mechanic, who is found bound inside. Calamity also unlocks the locked chests of the Abyss.",
                   "Dungeon: ศัตรู หีบ (ใช้ Golden Key) และ Mechanic ที่ถูกมัดอยู่ข้างใน ส่วนใน Calamity ยังปลดล็อกหีบที่ถูกล็อกใน Abyss ด้วย"),
    "Warning": ("Do not enter the Dungeon before Skeletron is defeated: the Dungeon Guardian kills almost instantly.",
                "อย่าเข้า Dungeon ก่อนชนะ Skeletron เพราะ Dungeon Guardian จะทำให้คุณตายแทบทันที"),
    "Tips": ("• Its spinning attack hurts a lot; keep moving and use a long platform arena.\n• The fight must end before dawn, or it becomes deadly.",
             "• ท่าหมุนตัวของมันแรงมาก เคลื่อนที่ตลอดและใช้สนามแพลตฟอร์มยาว\n• ต้องชนะก่อนรุ่งเช้า ไม่อย่างนั้นมันจะแรงขึ้นจนแทบสู้ไม่ได้"),
}

E["Deerclops"] = {
    "Short": ("Snow biome boss; optional.", "บอสไบโอมหิมะ ไม่บังคับ"),
    "Summary": ("A one-eyed deer monster from the snow. It creates ice spikes and shadow hands and can slow you. Optional.",
                "สัตว์ประหลาดกวางตาเดียวแห่งดินแดนหิมะ สร้างหนามน้ำแข็งและมือเงา และทำให้คุณช้าลงได้ ไม่บังคับ"),
    "Summon": ("Use a [item:Terraria/DeerThing] in a Snow biome. It can also appear at midnight during a Blizzard.",
               "ใช้ [item:Terraria/DeerThing] ในไบโอมหิมะ หรืออาจปรากฏเองตอนเที่ยงคืนระหว่างพายุหิมะ (Blizzard)"),
    "Tips": ("• Fight on flat ground; its ice spikes travel along the floor.",
             "• สู้บนพื้นเรียบ เพราะหนามน้ำแข็งของมันวิ่งไปตามพื้น"),
}

E["WallOfFlesh"] = {
    "Short": ("Gatekeeper of Hardmode.", "ประตูสู่ Hardmode"),
    "Summary": ("A wall of flesh that moves across the whole Underworld. Defeating it turns the world into Hardmode.",
                "กำแพงเนื้อที่เคลื่อนผ่านทั่วทั้ง Underworld ชนะแล้วโลกจะเข้าสู่ Hardmode"),
    "Summon": ("Throw a [item:Terraria/GuideVoodooDoll] into lava in the Underworld while the Guide is alive.",
               "โยน [item:Terraria/GuideVoodooDoll] ลงลาวาใน Underworld ขณะที่ Guide ยังมีชีวิตอยู่"),
    "Unlocks": ("Hardmode: new enemies, the Hallow and a second evil stripe, Pwnhammer for smashing altars, and many new bosses.",
                "Hardmode: ศัตรูใหม่ ไบโอม Hallow และแถบสายชั่วร้ายใหม่ รวมถึง Pwnhammer สำหรับทุบแท่นบูชา และบอสใหม่อีกมาก"),
    "UnlocksCal": ("Hardmode: new enemies, the Hallow and a second evil stripe, and many new bosses. With Calamity's early Hardmode rework (on by default) Cobalt and Palladium appear right away, altars give nothing when smashed, and each mechanical boss adds the next ore tier.",
                   "Hardmode: ศัตรูใหม่ ไบโอม Hallow และแถบสายชั่วร้ายใหม่ รวมถึงบอสใหม่อีกมาก ด้วยระบบปรับ Hardmode ช่วงต้นของ Calamity (เปิดเป็นค่าเริ่มต้น) แร่ Cobalt และ Palladium จะเกิดทันที การทุบแท่นบูชาจะไม่ให้อะไร และบอสจักรกลแต่ละตัวจะเพิ่มแร่ระดับถัดไป"),
    "Warning": ("Hardmode spreads the Hallow and the Corruption or Crimson. Consider protecting important areas before you fight it.",
                "Hardmode ทำให้ Hallow และ Corruption หรือ Crimson แพร่กระจาย ควรป้องกันพื้นที่สำคัญก่อนสู้"),
    "Tips": ("• Build a long bridge through the Underworld so you can keep running away from it while attacking.\n• Do not get too far from it: it pulls you back with its tongue.",
             "• สร้างสะพานยาวผ่าน Underworld เพื่อวิ่งหนีไปพร้อมกับโจมตี\n• อย่าอยู่ห่างจากมันเกินไป เพราะมันจะใช้ลิ้นดึงคุณกลับไป"),
}

E["FrostLegion"] = {
    "Name": ("Frost Legion", "Frost Legion"),
    "Short": ("Snowman invasion; Christmas only.", "มนุษย์หิมะบุก ช่วงคริสต์มาสเท่านั้น"),
    "Summary": ("An invasion of armed snowmen during the Christmas season.",
                "การบุกของมนุษย์หิมะติดอาวุธในช่วงเทศกาลคริสต์มาส"),
    "Summon": ("Use a [item:Terraria/SnowGlobe] during the Christmas season. Snow Globes come from opening Presents.",
               "ใช้ [item:Terraria/SnowGlobe] ในช่วงเทศกาลคริสต์มาส Snow Globe ได้จากการเปิด Present"),
    "Requires": ("Hardmode, during the Christmas season.", "ต้องเป็น Hardmode และอยู่ในช่วงเทศกาลคริสต์มาส"),
}

E["PirateInvasion"] = {
    "Name": ("Pirate Invasion", "Pirate Invasion"),
    "Short": ("Pirate invasion from the ocean.", "โจรสลัดบุกจากทะเล"),
    "Summary": ("Pirates invade your town, sometimes with the Flying Dutchman ship. They drop coins and pirate gear.",
                "โจรสลัดบุกเมืองของคุณ บางครั้งมีเรือ Flying Dutchman มาด้วย ดรอปเหรียญและอุปกรณ์โจรสลัด"),
    "Summon": ("Use a [item:Terraria/PirateMap], dropped by enemies in the Ocean in Hardmode. It can also start by itself in Hardmode.",
               "ใช้ [item:Terraria/PirateMap] ซึ่งดรอปจากศัตรูในทะเลช่วง Hardmode หรืออาจเริ่มเองในช่วง Hardmode"),
    "Unlocks": ("After the first win, the Pirate can move in.", "หลังชนะครั้งแรก Pirate จะย้ายเข้ามาได้"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
}

E["FlyingDutchman"] = {
    "Short": ("Flying pirate ship miniboss.", "มินิบอสเรือโจรสลัดบินได้"),
    "Summary": ("A flying pirate ship with four cannons. It sometimes appears during a Pirate Invasion.",
                "เรือโจรสลัดบินได้ที่มีปืนใหญ่ 4 กระบอก บางครั้งจะปรากฏระหว่างการบุกของโจรสลัด"),
    "Tips": ("• Destroy its cannons one by one from above or below.", "• ทำลายปืนใหญ่ทีละกระบอกจากด้านบนหรือด้านล่าง"),
}

E["QueenSlime"] = {
    "Short": ("Hallow slime queen; first Hardmode boss.", "ราชินีสไลม์แห่ง Hallow บอส Hardmode ตัวแรก"),
    "Summary": ("A crystal slime queen from the Hallow. In her second phase she flies. A good first Hardmode fight.",
                "ราชินีสไลม์คริสตัลแห่ง Hallow ในเฟสที่สองจะบินได้ เป็นบอส Hardmode ที่เหมาะเริ่มต้น"),
    "Summon": ("Use a [item:Terraria/QueenSlimeCrystal] in the Hallow.", "ใช้ [item:Terraria/QueenSlimeCrystal] ใน Hallow"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Tips": ("• Kill the crystal slimes she spawns; they fire in all directions.", "• กำจัดสไลม์คริสตัลที่เธอปล่อยออกมา เพราะมันยิงได้ทุกทิศทาง"),
}

E["Twins"] = {
    "Name": ("The Twins", "The Twins"),
    "Short": ("Mechanical boss: two laser eyes.", "บอสจักรกล: ดวงตาเลเซอร์สองดวง"),
    "Summary": ("Retinazer and Spazmatism: two mechanical eyes that fight together. One of the three mechanical bosses needed for Plantera.",
                "Retinazer และ Spazmatism ดวงตาจักรกลสองดวงที่สู้พร้อมกัน เป็นหนึ่งในบอสจักรกลสามตัวที่ต้องชนะก่อนสู้ Plantera"),
    "Summon": ("Use a [item:Terraria/MechanicalEye] at night. Mechanical bosses can also appear by themselves at night in Hardmode until one of them is defeated.",
               "ใช้ [item:Terraria/MechanicalEye] ตอนกลางคืน บอสจักรกลยังอาจปรากฏเองตอนกลางคืนในช่วง Hardmode จนกว่าจะชนะตัวใดตัวหนึ่ง"),
    "Unlocks": ("Souls of Sight. Together with the other two mechanical bosses: Plantera, Chlorophyte tools and the Life Fruit.",
                "Soul of Sight เมื่อรวมกับบอสจักรกลอีกสองตัว: Plantera, เครื่องมือขุด Chlorophyte และ Life Fruit"),
    "UnlocksCal": ("Souls of Sight. Each mechanical boss adds the next Hardmode ore tier (Calamity's early Hardmode rework). After all three: Plantera and Hallowed ore.",
                   "Soul of Sight ส่วนบอสจักรกลแต่ละตัวจะเพิ่มแร่ Hardmode ระดับถัดไป (ระบบปรับ Hardmode ช่วงต้นของ Calamity) และเมื่อชนะครบสามตัว: Plantera และแร่ Hallowed"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Tips": ("• Focus one eye at a time; Spazmatism's cursed flames hit hard up close.", "• โฟกัสทีละดวง เพราะไฟคำสาปของ Spazmatism แรงมากเมื่ออยู่ใกล้"),
}

E["Destroyer"] = {
    "Short": ("Mechanical boss: giant metal worm.", "บอสจักรกล: หนอนเหล็กยักษ์"),
    "Summary": ("A giant mechanical worm whose segments release probes and fire lasers. One of the three mechanical bosses needed for Plantera.",
                "หนอนจักรกลยักษ์ที่แต่ละปล้องปล่อยโดรนและยิงเลเซอร์ เป็นหนึ่งในบอสจักรกลสามตัวที่ต้องชนะก่อนสู้ Plantera"),
    "Summon": ("Use a [item:Terraria/MechanicalWorm] at night.", "ใช้ [item:Terraria/MechanicalWorm] ตอนกลางคืน"),
    "Unlocks": ("Souls of Might. Together with the other two mechanical bosses: Plantera, Chlorophyte tools and the Life Fruit.",
                "Soul of Might เมื่อรวมกับบอสจักรกลอีกสองตัว: Plantera, เครื่องมือขุด Chlorophyte และ Life Fruit"),
    "UnlocksCal": ("Souls of Might. Each mechanical boss adds the next Hardmode ore tier (Calamity's early Hardmode rework). After all three: Plantera and Hallowed ore.",
                   "Soul of Might ส่วนบอสจักรกลแต่ละตัวจะเพิ่มแร่ Hardmode ระดับถัดไป (ระบบปรับ Hardmode ช่วงต้นของ Calamity) และเมื่อชนะครบสามตัว: Plantera และแร่ Hallowed"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Tips": ("• Piercing weapons hit many segments at once.", "• อาวุธที่ทะลุจะโดนหลายปล้องพร้อมกัน"),
}

E["SkeletronPrime"] = {
    "Short": ("Mechanical boss: Skeletron with weapons.", "บอสจักรกล: Skeletron ติดอาวุธ"),
    "Summary": ("A mechanical Skeletron with four weapon arms. One of the three mechanical bosses needed for Plantera.",
                "Skeletron เวอร์ชันจักรกลที่มีแขนอาวุธ 4 ข้าง เป็นหนึ่งในบอสจักรกลสามตัวที่ต้องชนะก่อนสู้ Plantera"),
    "Summon": ("Use a [item:Terraria/MechanicalSkull] at night.", "ใช้ [item:Terraria/MechanicalSkull] ตอนกลางคืน"),
    "Unlocks": ("Souls of Fright. Together with the other two mechanical bosses: Plantera, Chlorophyte tools and the Life Fruit.",
                "Soul of Fright เมื่อรวมกับบอสจักรกลอีกสองตัว: Plantera, เครื่องมือขุด Chlorophyte และ Life Fruit"),
    "UnlocksCal": ("Souls of Fright. Each mechanical boss adds the next Hardmode ore tier (Calamity's early Hardmode rework). After all three: Plantera and Hallowed ore.",
                   "Soul of Fright ส่วนบอสจักรกลแต่ละตัวจะเพิ่มแร่ Hardmode ระดับถัดไป (ระบบปรับ Hardmode ช่วงต้นของ Calamity) และเมื่อชนะครบสามตัว: Plantera และแร่ Hallowed"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Tips": ("• Destroying the arms first makes the head much easier.", "• ทำลายแขนก่อนจะทำให้สู้กับหัวง่ายขึ้นมาก"),
}

E["Ogre"] = {
    "Short": ("Old One's Army miniboss (Tier 2 and 3).", "มินิบอสของ Old One's Army (ระดับ 2 และ 3)"),
    "Summary": ("The big miniboss of the Old One's Army's second and third tiers. It appears in wave 7 of Tier 2 and waves 4 and 6 of Tier 3.",
                "มินิบอสตัวใหญ่ของ Old One's Army ระดับ 2 และ 3 ปรากฏในระลอกที่ 7 ของระดับ 2 และระลอกที่ 4 กับ 6 ของระดับ 3"),
    "Requires": ("Tier 2 of the Old One's Army opens after any mechanical boss is defeated.", "Old One's Army ระดับ 2 เปิดหลังชนะบอสจักรกลตัวใดก็ได้"),
}

E["SolarEclipse"] = {
    "Name": ("Solar Eclipse", "Solar Eclipse"),
    "Short": ("Daytime horror-movie monsters.", "มอนสเตอร์หนังสยองขวัญตอนกลางวัน"),
    "Summary": ("A daytime event full of monsters from horror movies. Mothron appears after Plantera.",
                "อีเวนต์กลางวันที่เต็มไปด้วยมอนสเตอร์จากหนังสยองขวัญ Mothron จะปรากฏหลังชนะ Plantera"),
    "Summon": ("Use a [item:Terraria/SolarTablet] during the day. It can also happen by itself at dawn (1 in 20) once a mechanical boss is defeated.",
               "ใช้ [item:Terraria/SolarTablet] ตอนกลางวัน หรืออาจเกิดเองตอนรุ่งเช้า (โอกาส 1 ใน 20) หลังชนะบอสจักรกล"),
    "Requires": ("Defeat any mechanical boss first.", "ต้องชนะบอสจักรกลตัวใดก็ได้ก่อน"),
    "TipsCal": ("• After the Devourer of Gods, Eclipse enemies also drop Darksun Fragments.", "• หลังชนะ Devourer of Gods ศัตรูใน Eclipse จะดรอป Darksun Fragment ด้วย"),
}

E["Plantera"] = {
    "Short": ("Jungle plant boss; opens the Temple.", "บอสพืชแห่ง Jungle เปิดทางสู่วิหาร"),
    "Summary": ("A giant plant rooted in the underground Jungle. It shoots seeds and thorn balls and, in its second phase, charges with a biting maw. Defeating it opens the Jungle Temple.",
                "พืชยักษ์ที่หยั่งรากใน Jungle ใต้ดิน ยิงเมล็ดและลูกหนาม ในเฟสที่สองจะพุ่งกัดอย่างรุนแรง ชนะแล้วจะเปิดทางสู่ Jungle Temple"),
    "Summon": ("Break a Plantera's Bulb in the underground Jungle. Bulbs only grow after all three mechanical bosses are defeated.",
               "ทุบ Plantera's Bulb ใน Jungle ใต้ดิน ดอกนี้จะงอกหลังชนะบอสจักรกลครบทั้งสามตัว"),
    "SummonCal": ("Break a Plantera's Bulb in the underground Jungle; bulbs only grow after all three mechanical bosses are defeated. Calamity's [item:CalamityMod/Portabulb] can summon it again in the Jungle.",
                  "ทุบ Plantera's Bulb ใน Jungle ใต้ดิน ดอกนี้จะงอกหลังชนะบอสจักรกลครบทั้งสามตัว ส่วน [item:CalamityMod/Portabulb] ของ Calamity ใช้เรียกซ้ำได้ใน Jungle"),
    "Unlocks": ("The Temple Key for the Jungle Temple (Golem), Dungeon upgrades, the Pumpkin and Frost Moons and much more of late Hardmode.",
                "Temple Key สำหรับเข้า Jungle Temple (Golem), Dungeon ที่อัปเกรดแล้ว, Pumpkin Moon และ Frost Moon รวมถึงเนื้อหา Hardmode ช่วงท้ายอีกมาก"),
    "UnlocksCal": ("The Temple Key for the Jungle Temple (Golem) and late Hardmode. Calamity adds Perennial ore underground and the Great Sand Shark.",
                   "Temple Key สำหรับเข้า Jungle Temple (Golem) และเนื้อหา Hardmode ช่วงท้าย ส่วน Calamity เพิ่มแร่ Perennial ใต้ดินและ Great Sand Shark"),
    "Requires": ("Defeat all three mechanical bosses first.", "ต้องชนะบอสจักรกลครบทั้งสามตัวก่อน"),
    "Tips": ("• Clear a large area of the underground Jungle and build platforms before breaking the bulb.\n• If you leave the Jungle during the fight, she becomes enraged.",
             "• ถางพื้นที่ใน Jungle ใต้ดินให้กว้างและวางแพลตฟอร์มก่อนทุบดอก\n• ถ้าออกจาก Jungle ระหว่างสู้ เธอจะคลั่ง"),
}

E["Golem"] = {
    "Short": ("Lihzahrd golem of the Jungle Temple.", "โกเลม Lihzahrd แห่ง Jungle Temple"),
    "Summary": ("A stone golem built by the Lihzahrds. Its head detaches in the second phase. Defeating it lets the Lunatic Cultist appear.",
                "โกเลมหินที่ Lihzahrd สร้างขึ้น หัวของมันจะแยกออกในเฟสที่สอง ชนะแล้ว Lunatic Cultist จะปรากฏได้"),
    "Summon": ("Use a [item:Terraria/LihzahrdPowerCell] on the Lihzahrd Altar deep inside the Jungle Temple.",
               "ใช้ [item:Terraria/LihzahrdPowerCell] กับ Lihzahrd Altar ในส่วนลึกของ Jungle Temple"),
    "Unlocks": ("The Lunatic Cultist and its cultists at the Dungeon entrance, Martian Probes, Tier 3 of the Old One's Army.",
                "Lunatic Cultist และพวกลัทธิที่หน้าทางเข้า Dungeon, Martian Probe และ Old One's Army ระดับ 3"),
    "UnlocksCal": ("The Lunatic Cultist at the Dungeon entrance, Martian Probes and Tier 3 of the Old One's Army. Calamity adds the plague in the Jungle and the Plaguebringer Goliath.",
                   "Lunatic Cultist ที่หน้าทางเข้า Dungeon, Martian Probe และ Old One's Army ระดับ 3 ส่วน Calamity เพิ่มโรคระบาดใน Jungle และ Plaguebringer Goliath"),
    "Requires": ("The Temple Key drops from Plantera.", "Temple Key ดรอปจาก Plantera"),
    "Tips": ("• Clear the temple traps around the altar room first.", "• เคลียร์กับดักรอบห้องแท่นบูชาก่อนสู้"),
}

E["PumpkinMoon"] = {
    "Name": ("Pumpkin Moon", "Pumpkin Moon"),
    "Short": ("Halloween wave event at night.", "อีเวนต์ฮาโลวีนแบบระลอกตอนกลางคืน"),
    "Summary": ("A wave-based night event. Score points by killing enemies to reach later waves with Mourning Wood and the Pumpking.",
                "อีเวนต์ตอนกลางคืนแบบระลอก สะสมแต้มจากการกำจัดศัตรูเพื่อไปถึงระลอกหลังที่มี Mourning Wood และ Pumpking"),
    "Summon": ("Use a [item:Terraria/PumpkinMoonMedallion] at night. It lasts until dawn.",
               "ใช้ [item:Terraria/PumpkinMoonMedallion] ตอนกลางคืน อีเวนต์จะอยู่จนถึงรุ่งเช้า"),
    "Requires": ("The medallion needs Ectoplasm, which drops after Plantera.", "การคราฟต์เหรียญต้องใช้ Ectoplasm ซึ่งดรอปหลังชนะ Plantera"),
    "Tips": ("• Start right at nightfall to get the most waves.\n• Build an arena with lots of platforms near your base.",
             "• เริ่มทันทีเมื่อค่ำเพื่อให้ได้ระลอกมากที่สุด\n• สร้างสนามที่มีแพลตฟอร์มหลายชั้นใกล้ฐาน"),
    "TipsCal": ("• Start right at nightfall to get the most waves.\n• After the Devourer of Gods its enemies also drop Nightmare Fuel.",
                "• เริ่มทันทีเมื่อค่ำเพื่อให้ได้ระลอกมากที่สุด\n• หลังชนะ Devourer of Gods ศัตรูในอีเวนต์นี้จะดรอป Nightmare Fuel ด้วย"),
}

E["MourningWood"] = {
    "Short": ("Pumpkin Moon miniboss (wave 4+).", "มินิบอสของ Pumpkin Moon (ระลอก 4 ขึ้นไป)"),
    "Summary": ("A haunted tree that appears from wave 4 of the Pumpkin Moon.", "ต้นไม้ผีสิงที่ปรากฏตั้งแต่ระลอกที่ 4 ของ Pumpkin Moon"),
}

E["Pumpking"] = {
    "Short": ("Pumpkin Moon miniboss (wave 7+).", "มินิบอสของ Pumpkin Moon (ระลอก 7 ขึ้นไป)"),
    "Summary": ("The pumpkin king that appears from wave 7 of the Pumpkin Moon.", "ราชาฟักทองที่ปรากฏตั้งแต่ระลอกที่ 7 ของ Pumpkin Moon"),
}

E["FrostMoon"] = {
    "Name": ("Frost Moon", "Frost Moon"),
    "Short": ("Christmas wave event at night.", "อีเวนต์คริสต์มาสแบบระลอกตอนกลางคืน"),
    "Summary": ("A wave-based night event with Everscream, Santa-NK1 and the Ice Queen in later waves.",
                "อีเวนต์ตอนกลางคืนแบบระลอก ระลอกหลังจะมี Everscream, Santa-NK1 และ Ice Queen"),
    "Summon": ("Use a [item:Terraria/NaughtyPresent] at night. It lasts until dawn.", "ใช้ [item:Terraria/NaughtyPresent] ตอนกลางคืน อีเวนต์จะอยู่จนถึงรุ่งเช้า"),
    "Requires": ("The present needs Ectoplasm, which drops after Plantera.", "การคราฟต์ Naughty Present ต้องใช้ Ectoplasm ซึ่งดรอปหลังชนะ Plantera"),
    "Tips": ("• Start right at nightfall to get the most waves.", "• เริ่มทันทีเมื่อค่ำเพื่อให้ได้ระลอกมากที่สุด"),
    "TipsCal": ("• Start right at nightfall to get the most waves.\n• After the Devourer of Gods its enemies also drop Endothermic Energy.",
                "• เริ่มทันทีเมื่อค่ำเพื่อให้ได้ระลอกมากที่สุด\n• หลังชนะ Devourer of Gods ศัตรูในอีเวนต์นี้จะดรอป Endothermic Energy ด้วย"),
}

E["Everscream"] = {
    "Short": ("Frost Moon miniboss (wave 4+).", "มินิบอสของ Frost Moon (ระลอก 4 ขึ้นไป)"),
    "Summary": ("An evil Christmas tree that appears from wave 4 of the Frost Moon.", "ต้นคริสต์มาสชั่วร้ายที่ปรากฏตั้งแต่ระลอกที่ 4 ของ Frost Moon"),
}

E["SantaNK1"] = {
    "Short": ("Frost Moon miniboss (wave 7+).", "มินิบอสของ Frost Moon (ระลอก 7 ขึ้นไป)"),
    "Summary": ("A robot Santa tank that appears from wave 7 of the Frost Moon.", "หุ่นยนต์ซานต้าแบบรถถังที่ปรากฏตั้งแต่ระลอกที่ 7 ของ Frost Moon"),
}

E["IceQueen"] = {
    "Short": ("Frost Moon miniboss (wave 11+).", "มินิบอสของ Frost Moon (ระลอก 11 ขึ้นไป)"),
    "Summary": ("A flying ice queen that appears from wave 11 of the Frost Moon.", "ราชินีน้ำแข็งบินได้ที่ปรากฏตั้งแต่ระลอกที่ 11 ของ Frost Moon"),
}

E["MartianMadness"] = {
    "Name": ("Martian Madness", "Martian Madness"),
    "Short": ("Alien invasion after Golem.", "เอเลี่ยนบุกหลัง Golem"),
    "Summary": ("An alien invasion led by the Martian Saucer. Its drops include strong weapons and mounts.",
                "การบุกของเอเลี่ยนที่นำโดย Martian Saucer ของที่ดรอปมีอาวุธแรงและพาหนะ"),
    "Summon": ("After Golem, a Martian Probe may appear above ground. Let it see you and fly away to start the invasion.",
               "หลังชนะ Golem อาจมี Martian Probe ปรากฏเหนือพื้นดิน ถ้ามันเห็นคุณแล้วบินหนีกลับไปได้ อีเวนต์จะเริ่ม"),
    "Requires": ("Defeat Golem first.", "ต้องชนะ Golem ก่อน"),
    "Tips": ("• Kill the probe before it escapes if you are not ready.", "• ถ้ายังไม่พร้อม ให้กำจัด Probe ก่อนที่มันจะหนีไป"),
}

E["MartianSaucer"] = {
    "Short": ("Martian Madness miniboss.", "มินิบอสของ Martian Madness"),
    "Summary": ("A flying saucer with turrets that appears during Martian Madness.", "จานบินพร้อมป้อมปืนที่ปรากฏระหว่าง Martian Madness"),
}

E["DukeFishron"] = {
    "Short": ("Ocean boss fished up with a Truffle Worm.", "บอสทะเลที่ตกได้ด้วย Truffle Worm"),
    "Summary": ("A mutant pig-shark that charges very fast and spawns sharknadoes. Optional but drops excellent gear.",
                "หมูผสมฉลามกลายพันธุ์ที่พุ่งเร็วมากและสร้างพายุฉลาม ไม่บังคับแต่ดรอปอุปกรณ์ดีมาก"),
    "Summon": ("Fish in the Ocean using a [item:Terraria/TruffleWorm] as bait. Truffle Worms are caught in the underground Glowing Mushroom biome in Hardmode.",
               "ตกปลาในทะเลโดยใช้ [item:Terraria/TruffleWorm] เป็นเหยื่อ ส่วน Truffle Worm จับได้ในไบโอม Glowing Mushroom ใต้ดินช่วง Hardmode"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Warning": ("If you leave the Ocean during the fight, it becomes enraged.", "ถ้าออกจากทะเลระหว่างสู้ มันจะคลั่ง"),
}

E["EmpressOfLight"] = {
    "Short": ("Hallow fairy empress; optional.", "จักรพรรดินีภูตแห่ง Hallow ไม่บังคับ"),
    "Summary": ("A radiant fairy empress with dense bullet patterns. Optional but drops top-tier gear. Fighting her during the day is far deadlier.",
                "จักรพรรดินีภูตผู้เจิดจ้าที่ยิงกระสุนหนาแน่น ไม่บังคับแต่ดรอปของระดับสูง สู้ตอนกลางวันจะอันตรายกว่ามาก"),
    "Summon": ("Kill a [item:Terraria/EmpressButterfly] (Prismatic Lacewing) in the surface Hallow at night after Plantera.",
               "กำจัด [item:Terraria/EmpressButterfly] (Prismatic Lacewing) ใน Hallow บนผิวดินตอนกลางคืนหลังชนะ Plantera"),
    "Requires": ("Defeat Plantera first.", "ต้องชนะ Plantera ก่อน"),
    "Warning": ("If the sun rises during the fight, her attacks kill almost instantly.", "ถ้าพระอาทิตย์ขึ้นระหว่างสู้ การโจมตีของเธอแทบจะทำให้ตายในทีเดียว"),
}

E["Betsy"] = {
    "Short": ("Old One's Army final boss (Tier 3).", "บอสสุดท้ายของ Old One's Army (ระดับ 3)"),
    "Summary": ("A fire-breathing dragon that appears in the final wave (7) of the Old One's Army's third tier.",
                "มังกรพ่นไฟที่ปรากฏในระลอกสุดท้าย (ที่ 7) ของ Old One's Army ระดับ 3"),
    "Summon": ("Start Tier 3 of the Old One's Army ([item:Terraria/DD2ElderCrystal] on its stand) and reach wave 7.",
               "เริ่ม Old One's Army ระดับ 3 (ใช้ [item:Terraria/DD2ElderCrystal] กับแท่น) แล้วไปให้ถึงระลอกที่ 7"),
    "Requires": ("Tier 3 of the Old One's Army opens after Golem.", "Old One's Army ระดับ 3 เปิดหลังชนะ Golem"),
}

E["LunaticCultist"] = {
    "Short": ("Cultist at the Dungeon; starts the pillars.", "ผู้นำลัทธิที่ Dungeon เริ่มอีเวนต์เสา"),
    "Summary": ("The leader of the cultists at the Dungeon entrance. Defeating him starts the Celestial Pillars.",
                "ผู้นำลัทธิที่หน้าทางเข้า Dungeon ชนะแล้วจะเริ่มอีเวนต์ Celestial Pillars"),
    "Summon": ("After Golem, four cultists worship in front of the Dungeon. Attack them to start the fight.",
               "หลังชนะ Golem จะมีสาวกลัทธิ 4 คนบูชาอยู่หน้า Dungeon โจมตีพวกเขาเพื่อเริ่มการต่อสู้"),
    "SummonCal": ("After Golem, four cultists worship in front of the Dungeon; attack them to start the fight. Calamity's [item:CalamityMod/EidolonTablet] can also summon it.",
                  "หลังชนะ Golem จะมีสาวกลัทธิ 4 คนบูชาอยู่หน้า Dungeon โจมตีพวกเขาเพื่อเริ่มการต่อสู้ หรือใช้ [item:CalamityMod/EidolonTablet] ของ Calamity เรียกได้เช่นกัน"),
    "Unlocks": ("The Celestial Pillars appear across the world.", "Celestial Pillar จะปรากฏทั่วโลก"),
    "Requires": ("Defeat Golem first.", "ต้องชนะ Golem ก่อน"),
    "Tips": ("• During his ritual he hides among copies of himself; find the real one quickly.", "• ระหว่างทำพิธีเขาจะซ่อนตัวปนกับร่างโคลน รีบหาตัวจริงให้เจอ"),
}

E["CelestialPillars"] = {
    "Name": ("Celestial Pillars", "Celestial Pillars"),
    "Short": ("Destroy the four lunar towers.", "ทำลายหอคอยดวงจันทร์ทั้ง 4"),
    "Summary": ("Four Celestial Towers (Solar, Vortex, Nebula, Stardust) are shielded until you kill enough enemies around each. Destroying all four summons the Moon Lord.",
                "หอคอยดวงจันทร์ 4 ต้น (Solar, Vortex, Nebula, Stardust) มีโล่ป้องกันจนกว่าจะกำจัดศัตรูรอบ ๆ ได้ครบ ทำลายครบทั้ง 4 ต้นจะเรียก Moon Lord"),
    "Summon": ("They appear when the Lunatic Cultist is defeated.", "เสาจะปรากฏเมื่อชนะ Lunatic Cultist"),
    "Unlocks": ("Lunar fragments for endgame gear, and the Moon Lord.", "เศษชิ้นส่วนดวงจันทร์สำหรับอุปกรณ์ช่วงท้าย และ Moon Lord"),
    "Requires": ("Defeat the Lunatic Cultist first.", "ต้องชนะ Lunatic Cultist ก่อน"),
}

E["MoonLord"] = {
    "Short": ("Final boss of Terraria.", "บอสสุดท้ายของ Terraria"),
    "Summary": ("The final boss of Terraria. Destroy both hands and the head before the core becomes vulnerable.",
                "บอสสุดท้ายของ Terraria ต้องทำลายมือทั้งสองข้างและหัวก่อน แกนกลางจึงจะโดนดาเมจได้"),
    "Summon": ("It arrives shortly after all four Celestial Pillars are destroyed. Later, a [item:Terraria/CelestialSigil] summons it again.",
               "จะมาหลังทำลาย Celestial Pillar ครบ 4 ต้นไม่นาน หลังจากนั้นใช้ [item:Terraria/CelestialSigil] เรียกซ้ำได้"),
    "Unlocks": ("Luminite ore and the best vanilla gear.", "แร่ Luminite และอุปกรณ์ที่ดีที่สุดของเกมหลัก"),
    "UnlocksCal": ("Luminite ore and the best vanilla gear. In Calamity this is only halfway: the Profaned Guardians, Providence and the rest of the post-Moon Lord route open.",
                   "แร่ Luminite และอุปกรณ์ที่ดีที่สุดของเกมหลัก แต่ใน Calamity นี่เพิ่งครึ่งทาง: Profaned Guardians, Providence และเส้นทางหลัง Moon Lord จะเปิดขึ้น"),
    "Requires": ("Destroy the four Celestial Pillars.", "ทำลาย Celestial Pillar ทั้ง 4 ต้น"),
    "Warning": ("With Catalyst loaded, the Astral Communicator stops working after the Moon Lord unless Astrageldon was already defeated.",
                "ถ้าเปิดม็อด Catalyst อยู่ Astral Communicator จะใช้ไม่ได้หลังชนะ Moon Lord หากยังไม่เคยชนะ Astrageldon"),
    "Tips": ("• Its gaze attacks and the Phantasmal Deathray hit hard: keep moving and bring healing.", "• ลำแสงจากดวงตาและ Phantasmal Deathray แรงมาก เคลื่อนที่ตลอดและเตรียมยาฟื้นพลังให้พอ"),
}

# ====================================================================== Calamity Mod 2.2.4

E["DesertScourge"] = {
    "Short": ("Desert worm; Calamity's first boss.", "หนอนทะเลทราย บอสแรกของ Calamity"),
    "Summary": ("A giant worm of the desert and Calamity's first boss. Defeating it lets the Giant Clam appear in the Sunken Sea.",
                "หนอนยักษ์แห่งทะเลทรายและบอสตัวแรกของ Calamity ชนะแล้ว Giant Clam จะเริ่มปรากฏใน Sunken Sea"),
    "Summon": ("Use a [item:CalamityMod/DesertMedallion] in the Desert. The medallion is not consumed.", "ใช้ [item:CalamityMod/DesertMedallion] ในทะเลทราย เหรียญไม่หายเมื่อใช้"),
    "Unlocks": ("The Giant Clam in the Sunken Sea, and a sandstorm starts. The Sea King can move in once both it and the Giant Clam are defeated.",
                "Giant Clam ใน Sunken Sea และพายุทราย ส่วน Sea King จะย้ายเข้ามาได้เมื่อชนะทั้งบอสนี้และ Giant Clam"),
    "Tips": ("• Fight in the open desert with a platform skybridge above the sand.", "• สู้ในทะเลทรายโล่ง ๆ และสร้างสะพานแพลตฟอร์มเหนือพื้นทราย"),
}

E["GiantClam"] = {
    "Short": ("Sunken Sea miniboss.", "มินิบอสของ Sunken Sea"),
    "Summary": ("A giant clam living in the waters of the Sunken Sea, below the Desert.", "หอยยักษ์ที่อาศัยในน้ำของ Sunken Sea ใต้ทะเลทราย"),
    "Summon": ("Spawns naturally in the water of the Sunken Sea once the Desert Scourge is defeated.", "เกิดเองในน้ำของ Sunken Sea หลังชนะ Desert Scourge"),
    "Requires": ("Defeat the Desert Scourge first.", "ต้องชนะ Desert Scourge ก่อน"),
}

E["AcidRain1"] = {
    "Name": ("Acid Rain", "Acid Rain"),
    "Short": ("Toxic storm at the Sulphurous Sea.", "พายุพิษที่ Sulphurous Sea"),
    "Summary": ("Calamity's Sulphurous Sea event. Acidic enemies and irradiated creatures attack; the first tier drops early materials.",
                "อีเวนต์ของ Sulphurous Sea ใน Calamity ศัตรูกรดและสัตว์กลายพันธุ์จากรังสีจะโจมตี ระดับแรกดรอปวัตถุดิบช่วงต้น"),
    "Summon": ("After the Eye of Cthulhu, the first Acid Rain starts when you enter the Sulphurous Sea. Later ones start by themselves at night, or use a [item:CalamityMod/CausticTear].",
               "หลังชนะ Eye of Cthulhu แล้ว Acid Rain ครั้งแรกจะเริ่มเมื่อคุณเข้าไปใน Sulphurous Sea ส่วนครั้งต่อไปจะเริ่มเองตอนกลางคืน หรือใช้ [item:CalamityMod/CausticTear]"),
    "Requires": ("Defeat the Eye of Cthulhu first.", "ต้องชนะ Eye of Cthulhu ก่อน"),
    "Warning": ("A Caustic Tear used before the Eye of Cthulhu (or Hardmode) is consumed but starts nothing.",
                "Caustic Tear ที่ใช้ก่อนชนะ Eye of Cthulhu (หรือก่อน Hardmode) จะหายไปโดยไม่เริ่มอีเวนต์"),
    "Tips": ("• The event fails if the rain stops or if no Acid Rain enemy is killed for 2.5 minutes.\n• Sulphurous water poisons you; avoid swimming in it early.",
             "• อีเวนต์จะล้มเหลวถ้าฝนหยุด หรือไม่ได้กำจัดศัตรูของ Acid Rain เลย 2.5 นาที\n• น้ำของ Sulphurous Sea มีพิษ ช่วงต้นอย่าว่ายน้ำนาน"),
}

E["Crabulon"] = {
    "Short": ("Mushroom crab boss.", "บอสปูเห็ด"),
    "Summary": ("A giant crab covered in glowing mushrooms, from the underground Glowing Mushroom biome.", "ปูยักษ์ที่ปกคลุมด้วยเห็ดเรืองแสง จากไบโอม Glowing Mushroom ใต้ดิน"),
    "Summon": ("Use a [item:CalamityMod/DecapoditaSprout] in an underground Glowing Mushroom biome. The sprout is not consumed.",
               "ใช้ [item:CalamityMod/DecapoditaSprout] ในไบโอม Glowing Mushroom ใต้ดิน ไอเทมไม่หายเมื่อใช้"),
    "Unlocks": ("In Revengeance its Treasure Bag holds the [item:CalamityMod/MushroomPlasmaRoot] (longer Rage).", "ในโหมด Revengeance ถุงของมันมี [item:CalamityMod/MushroomPlasmaRoot] (Rage อยู่นานขึ้น)"),
    "Tips": ("• Dig out a wide space in the mushroom biome before summoning; it jumps a lot.", "• ขุดพื้นที่ในไบโอมเห็ดให้กว้างก่อนเรียก เพราะมันกระโดดบ่อย"),
}

E["HiveMind"] = {
    "Short": ("Calamity's Corruption boss.", "บอส Corruption ของ Calamity"),
    "Summary": ("Calamity's boss of the Corruption. Its first defeat (or the Perforators') activates Aerialite ore in the sky.",
                "บอสแห่ง Corruption ของ Calamity การชนะครั้งแรก (หรือชนะ Perforators) ทำให้ขุดแร่ Aerialite บนฟ้าได้"),
    "Summon": ("Kill a Hive Tumor in the Corruption, or use a [item:CalamityMod/Teratoma] in the Corruption.", "ทำลาย Hive Tumor ใน Corruption หรือใช้ [item:CalamityMod/Teratoma] ใน Corruption"),
    "Unlocks": ("Aerialite ore on floating islands can now be mined (65% pickaxe power).", "ขุดแร่ Aerialite บนเกาะลอยฟ้าได้แล้ว (Pickaxe พลัง 65%)"),
    "Warning": ("Killing a Hive Tumor summons the boss: do not break them by accident.", "การทำลาย Hive Tumor จะเรียกบอสทันที ระวังอย่าทำลายโดยไม่ตั้งใจ"),
}

E["Perforators"] = {
    "Name": ("The Perforators", "The Perforators"),
    "Short": ("Calamity's Crimson boss.", "บอส Crimson ของ Calamity"),
    "Summary": ("A hive of the Crimson that releases three worms of different sizes. Its first defeat (or the Hive Mind's) activates Aerialite ore in the sky.",
                "รังของ Crimson ที่ปล่อยหนอนสามขนาดออกมา การชนะครั้งแรก (หรือชนะ Hive Mind) ทำให้ขุดแร่ Aerialite บนฟ้าได้"),
    "Summon": ("Kill a Perforator Cyst in the Crimson, or use [item:CalamityMod/BloodyWormFood] in the Crimson.", "ทำลาย Perforator Cyst ใน Crimson หรือใช้ [item:CalamityMod/BloodyWormFood] ใน Crimson"),
    "Unlocks": ("Aerialite ore on floating islands can now be mined (65% pickaxe power).", "ขุดแร่ Aerialite บนเกาะลอยฟ้าได้แล้ว (Pickaxe พลัง 65%)"),
    "Warning": ("Killing a Perforator Cyst summons the boss: do not break them by accident.", "การทำลาย Perforator Cyst จะเรียกบอสทันที ระวังอย่าทำลายโดยไม่ตั้งใจ"),
}

E["SlimeGod"] = {
    "Short": ("Two giant slimes and their core.", "สไลม์ยักษ์สองตัวกับแกนกลาง"),
    "Summary": ("A slime core with two huge Paladin slimes (one Corruption, one Crimson) that split when damaged.", "แกนสไลม์ที่มีสไลม์ Paladin ยักษ์สองตัว (Corruption และ Crimson) ซึ่งแยกร่างเมื่อโดนดาเมจ"),
    "Summon": ("Use an [item:CalamityMod/OverloadedSludge] anywhere.", "ใช้ [item:CalamityMod/OverloadedSludge] ที่ไหนก็ได้"),
    "Unlocks": ("In Revengeance its Treasure Bag holds the [item:CalamityMod/ElectrolyteGelPack] (stronger Adrenaline).", "ในโหมด Revengeance ถุงของมันมี [item:CalamityMod/ElectrolyteGelPack] (Adrenaline แรงขึ้น)"),
    "Tips": ("• The large slimes split into smaller ones when hurt; area damage helps.", "• สไลม์ใหญ่จะแยกเป็นตัวเล็กเมื่อโดนดาเมจ อาวุธโจมตีเป็นวงช่วยได้มาก"),
}

E["Cryogen"] = {
    "Short": ("Ice crystal boss of the Snow biome.", "บอสผลึกน้ำแข็งแห่งไบโอมหิมะ"),
    "Summary": ("A frozen crystal that fires ice in many patterns. Defeating it charges Cryonic ore and frees Permafrost the Archmage.",
                "ผลึกน้ำแข็งที่ยิงน้ำแข็งหลายรูปแบบ ชนะแล้วแร่ Cryonic จะขุดได้ และ Permafrost (Archmage) ย้ายเข้ามาได้"),
    "Summon": ("Use a [item:CalamityMod/CryoKey] in the Snow biome.", "ใช้ [item:CalamityMod/CryoKey] ในไบโอมหิมะ"),
    "Unlocks": ("Cryonic ore in the ice caves, and the Archmage (Permafrost) can move in.", "แร่ Cryonic ในถ้ำน้ำแข็ง และ Archmage (Permafrost) ย้ายเข้ามาได้"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
}

E["AquaticScourge"] = {
    "Short": ("Sulphurous Sea worm; stronger Acid Rain.", "หนอนแห่ง Sulphurous Sea ทำให้ Acid Rain แรงขึ้น"),
    "Summary": ("A giant sea worm of the Sulphurous Sea. Defeating it upgrades Acid Rain to its second tier.",
                "หนอนทะเลยักษ์แห่ง Sulphurous Sea ชนะแล้ว Acid Rain จะอัปเกรดเป็นระดับที่สอง"),
    "Summon": ("Use [item:CalamityMod/Seafood] in the Sulphurous Sea, or wait for it to appear there by itself.", "ใช้ [item:CalamityMod/Seafood] ใน Sulphurous Sea หรือรอให้มันปรากฏเองที่นั่น"),
    "Unlocks": ("Acid Rain tier 2, with Cragmaw Mire and new drops.", "Acid Rain ระดับ 2 ที่มี Cragmaw Mire และของดรอปใหม่"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
}

E["AcidRain2"] = {
    "Name": ("Acid Rain (tier 2)", "Acid Rain (ระดับ 2)"),
    "Short": ("Acid Rain tier 2 (after Aquatic Scourge).", "Acid Rain ระดับ 2 (หลัง Aquatic Scourge)"),
    "Summary": ("The second tier of Acid Rain, with stronger enemies and the Cragmaw Mire.", "Acid Rain ระดับที่สอง ศัตรูแรงขึ้นและมี Cragmaw Mire"),
    "Summon": ("Starts by itself at night or with a [item:CalamityMod/CausticTear] once the Aquatic Scourge is defeated.", "เริ่มเองตอนกลางคืน หรือใช้ [item:CalamityMod/CausticTear] หลังชนะ Aquatic Scourge"),
    "Requires": ("Defeat the Aquatic Scourge first.", "ต้องชนะ Aquatic Scourge ก่อน"),
}

E["CragmawMire"] = {
    "Short": ("Acid Rain miniboss (tier 2+).", "มินิบอสของ Acid Rain (ระดับ 2 ขึ้นไป)"),
    "Summary": ("An irradiated crab miniboss that appears during Acid Rain after the Aquatic Scourge.", "มินิบอสปูกลายพันธุ์ที่ปรากฏใน Acid Rain หลังชนะ Aquatic Scourge"),
}

E["BrimstoneElemental"] = {
    "Short": ("Fire elemental of the Brimstone Crag.", "ธาตุไฟแห่ง Brimstone Crag"),
    "Summary": ("An elemental of brimstone fire from the Brimstone Crag in the Underworld.", "วิญญาณธาตุไฟ Brimstone จาก Brimstone Crag ใน Underworld"),
    "Summon": ("Use a [item:CalamityMod/CharredIdol] in the Brimstone Crag.", "ใช้ [item:CalamityMod/CharredIdol] ใน Brimstone Crag"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
}

E["CalamitasClone"] = {
    "Short": ("Brimstone witch clone; tests your gear.", "ร่างโคลนแม่มด Brimstone ทดสอบอุปกรณ์"),
    "Summary": ("A clone of the witch Calamitas with two brothers. A good check before Plantera.", "ร่างโคลนของแม่มด Calamitas พร้อมพี่น้องอีกสองตัว เป็นบททดสอบที่ดีก่อนสู้ Plantera"),
    "Summon": ("Use an [item:CalamityMod/EyeofDesolation].", "ใช้ [item:CalamityMod/EyeofDesolation]"),
    "Unlocks": ("Ashes of Calamity, later used to summon Supreme Calamitas.", "Ashes of Calamity ซึ่งภายหลังใช้เรียก Supreme Calamitas"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
}

E["GreatSandShark"] = {
    "Short": ("Sand shark miniboss after Plantera.", "มินิบอสฉลามทรายหลัง Plantera"),
    "Summary": ("A giant shark that swims through sand, found in the Desert.", "ฉลามยักษ์ที่ว่ายผ่านทราย พบในทะเลทราย"),
    "Summon": ("Kill 10 Sand Sharks after Plantera, or use a [item:CalamityMod/SandstormsCore] in the Desert.", "กำจัด Sand Shark ครบ 10 ตัวหลังชนะ Plantera หรือใช้ [item:CalamityMod/SandstormsCore] ในทะเลทราย"),
    "Requires": ("Defeat Plantera first.", "ต้องชนะ Plantera ก่อน"),
}

E["Leviathan"] = {
    "Name": ("Leviathan and Anahita", "Leviathan และ Anahita"),
    "Short": ("Ocean duo: Anahita and the Leviathan.", "คู่บอสทะเล: Anahita และ Leviathan"),
    "Summary": ("Anahita, a siren, fights first; the Leviathan joins later. Defeating them unlocks new drops in the Abyss.",
                "Anahita นางไซเรนจะสู้ก่อน แล้ว Leviathan จะมาร่วมภายหลัง ชนะแล้วจะปลดล็อกของดรอปใหม่ใน Abyss"),
    "Summon": ("Kill the elusive singer that sometimes appears in the Ocean, or use a [item:CalamityMod/NaiadsWarhorn] in the Ocean. The horn needs Lumenyl, which only drops after this fight, so the first time is always through the singer.",
               "กำจัดนักร้องลึกลับที่บางครั้งปรากฏในทะเล หรือใช้ [item:CalamityMod/NaiadsWarhorn] ในทะเล แตรต้องใช้ Lumenyl ซึ่งดรอปหลังชนะบอสนี้เท่านั้น ครั้งแรกจึงต้องเรียกผ่านนักร้อง"),
    "Unlocks": ("Lumenyl and other Abyss drops.", "Lumenyl และของดรอปอื่นใน Abyss"),
    "Warning": ("The singer can appear in the Ocean even before Hardmode; killing it early summons Anahita.", "นักร้องอาจปรากฏในทะเลได้ตั้งแต่ก่อน Hardmode ถ้ากำจัดเร็วเกินไปจะเรียก Anahita ออกมา"),
    "Requires": ("The singer appears much more often after Plantera or Calamitas Clone.", "นักร้องจะปรากฏบ่อยขึ้นมากหลังชนะ Plantera หรือ Calamitas Clone"),
}

E["AstrumAureus"] = {
    "Short": ("Astral mech-walker; another meteor falls.", "หุ่นเดินแห่งดวงดาว ทำให้อุกกาบาต Astral ตกอีก"),
    "Summary": ("A giant astral walker from the Astral Infection. Defeating it drops another astral meteor and empowers astral enemies.",
                "หุ่นเดินยักษ์จาก Astral Infection ชนะแล้วจะมีอุกกาบาต Astral ตกอีกลูกและศัตรู Astral จะแรงขึ้น"),
    "Summon": ("Use an [item:CalamityMod/AstralChunk] in the Astral Infection.", "ใช้ [item:CalamityMod/AstralChunk] ใน Astral Infection"),
    "Unlocks": ("In Revengeance its Treasure Bag holds the [item:CalamityMod/StarlightFuelCell] (stronger Adrenaline).", "ในโหมด Revengeance ถุงของมันมี [item:CalamityMod/StarlightFuelCell] (Adrenaline แรงขึ้น)"),
    "Requires": ("The Astral Chunk needs a decrypted schematic from the lab on the large sky planetoid.", "Astral Chunk ต้องใช้แบบพิมพ์เขียวที่ถอดรหัสแล้วจากห้องแล็บบน Planetoid ใหญ่บนฟ้า"),
}

E["PlaguebringerGoliath"] = {
    "Short": ("Plague mecha-bee of the Jungle.", "ผึ้งจักรกลแห่งโรคระบาดใน Jungle"),
    "Summary": ("A giant mechanical bee spreading the plague, fought in the Jungle after Golem.", "ผึ้งจักรกลยักษ์ที่แพร่โรคระบาด สู้ใน Jungle หลังชนะ Golem"),
    "Summon": ("Use an [item:CalamityMod/Abombination] in the Jungle.", "ใช้ [item:CalamityMod/Abombination] ใน Jungle"),
    "Requires": ("The Abombination needs Plague Cell Canisters (plague enemies after Golem) and a decrypted schematic from the Jungle lab.", "Abombination ต้องใช้ Plague Cell Canister (จากศัตรูโรคระบาดหลัง Golem) และแบบพิมพ์เขียวที่ถอดรหัสจากห้องแล็บใน Jungle"),
}

E["Ravager"] = {
    "Short": ("Flesh golem of stone and bone.", "โกเลมเนื้อหนังจากหินและกระดูก"),
    "Summary": ("A huge flesh golem with detachable limbs.", "โกเลมเนื้อหนังยักษ์ที่แขนขาแยกออกได้"),
    "Summon": ("Use a [item:CalamityMod/DeathWhistle].", "ใช้ [item:CalamityMod/DeathWhistle]"),
    "Unlocks": ("In Revengeance its Treasure Bag holds [item:CalamityMod/InfernalBlood] (longer Rage).", "ในโหมด Revengeance ถุงของมันมี [item:CalamityMod/InfernalBlood] (Rage อยู่นานขึ้น)"),
    "Requires": ("The whistle needs Lunar Tablet Fragments from Jungle Temple enemies (after Plantera).", "นกหวีดต้องใช้ Lunar Tablet Fragment จากศัตรูใน Jungle Temple (หลัง Plantera)"),
}

E["AstrumDeus"] = {
    "Short": ("Astral star-worm; unlocks Astral Ore.", "หนอนดวงดาว ปลดล็อกแร่ Astral"),
    "Summary": ("A cosmic worm of the Astral Infection that splits into two worms. Defeating it lets you mine Astral Ore.",
                "หนอนจักรวาลแห่ง Astral Infection ที่แยกเป็นสองตัว ชนะแล้วจะขุดแร่ Astral ได้"),
    "Summon": ("Offer a [item:CalamityMod/TitanHeart] or [item:CalamityMod/Starcore] at an [item:CalamityMod/AstralBeaconItem] (a beacon also stands in the Astral Infection).",
               "ถวาย [item:CalamityMod/TitanHeart] หรือ [item:CalamityMod/Starcore] ที่ [item:CalamityMod/AstralBeaconItem] (มีแท่นตั้งอยู่ใน Astral Infection ด้วย)"),
    "Unlocks": ("Astral Ore can be mined.", "ขุดแร่ Astral ได้"),
    "Requires": ("Recommended after the Lunatic Cultist. Titan Hearts drop from Atlas, an enemy of the surface Astral Infection.", "แนะนำให้สู้หลัง Lunatic Cultist ส่วน Titan Heart ดรอปจาก Atlas ศัตรูของ Astral Infection บนผิวดิน"),
}

E["ProfanedGuardians"] = {
    "Name": ("Profaned Guardians", "Profaned Guardians"),
    "Short": ("Three guardians of the profaned flame.", "ผู้พิทักษ์เปลวไฟศักดิ์สิทธิ์สามตน"),
    "Summary": ("Three guardians of Providence: an attacker, a defender and a healer. Defeating them gives the Profaned Core.",
                "ผู้พิทักษ์ของ Providence สามตน: ฝ่ายโจมตี ฝ่ายป้องกัน และฝ่ายรักษา ชนะแล้วจะได้ Profaned Core"),
    "Summon": ("Use a [item:CalamityMod/ProfanedShard] in the Hallow or in the Underworld.", "ใช้ [item:CalamityMod/ProfanedShard] ใน Hallow หรือใน Underworld"),
    "Unlocks": ("The [item:CalamityMod/ProfanedCore] to summon Providence.", "[item:CalamityMod/ProfanedCore] สำหรับเรียก Providence"),
    "Requires": ("Defeat the Moon Lord first.", "ต้องชนะ Moon Lord ก่อน"),
}

E["Dragonfolly"] = {
    "Short": ("Jungle thunder-bird.", "นกสายฟ้าแห่ง Jungle"),
    "Summary": ("A giant electric bird of the Jungle. Its Effulgent Feathers are needed for Yharon's egg.", "นกไฟฟ้ายักษ์แห่ง Jungle ส่วน Effulgent Feather ของมันใช้ทำไข่สำหรับเรียก Yharon"),
    "Summon": ("Use [item:CalamityMod/ExoticPheromones] in the Jungle.", "ใช้ [item:CalamityMod/ExoticPheromones] ใน Jungle"),
    "Unlocks": ("Effulgent Feathers for the Yharon egg. In Revengeance its Treasure Bag holds the [item:CalamityMod/RedLightningContainer] (longer Rage).",
                "Effulgent Feather สำหรับไข่ Yharon และในโหมด Revengeance ถุงของมันมี [item:CalamityMod/RedLightningContainer] (Rage อยู่นานขึ้น)"),
    "Requires": ("Defeat the Moon Lord first.", "ต้องชนะ Moon Lord ก่อน"),
}

E["Providence"] = {
    "Short": ("The Profaned Goddess.", "เทพีแห่งเปลวไฟศักดิ์สิทธิ์"),
    "Summary": ("The Profaned Goddess. Defeating her releases Uelibloom ore and lets you fight the Sentinels.",
                "เทพีแห่งเปลวไฟศักดิ์สิทธิ์ ชนะแล้วแร่ Uelibloom จะเกิดและสู้ Sentinel ได้"),
    "Summon": ("Use the [item:CalamityMod/ProfanedCore] in the Hallow or in the Underworld.", "ใช้ [item:CalamityMod/ProfanedCore] ใน Hallow หรือใน Underworld"),
    "Unlocks": ("Uelibloom ore in the Jungle, and the Mark of Providence for the Sentinels.", "แร่ Uelibloom ใน Jungle และ Mark of Providence สำหรับเรียก Sentinel"),
    "Requires": ("The Profaned Core drops from the Profaned Guardians.", "Profaned Core ดรอปจาก Profaned Guardians"),
}

E["CeaselessVoid"] = {
    "Short": ("Sentinel of the Dungeon.", "Sentinel แห่ง Dungeon"),
    "Summary": ("One of the three Sentinels of the Devourer: a void that pulls you in, fought in the Dungeon.", "หนึ่งใน Sentinel สามตนของ Devourer: ความว่างเปล่าที่ดูดคุณเข้าไป สู้ใน Dungeon"),
    "Summon": ("Use a [item:CalamityMod/MarkofProvidence] in the Dungeon.", "ใช้ [item:CalamityMod/MarkofProvidence] ใน Dungeon"),
    "Requires": ("Defeat the Moon Lord first. The Mark of Providence drops from Providence or can be crafted.", "ต้องชนะ Moon Lord ก่อน ส่วน Mark of Providence ดรอปจาก Providence หรือคราฟต์ได้"),
}

E["StormWeaver"] = {
    "Short": ("Sentinel of space.", "Sentinel แห่งอวกาศ"),
    "Summary": ("One of the three Sentinels of the Devourer: an armored storm worm, fought high in the sky or in space.", "หนึ่งใน Sentinel สามตนของ Devourer: หนอนพายุหุ้มเกราะ สู้บนท้องฟ้าสูงหรือในอวกาศ"),
    "Summon": ("Use a [item:CalamityMod/MarkofProvidence] high in the sky or in space.", "ใช้ [item:CalamityMod/MarkofProvidence] บนท้องฟ้าสูงหรือในอวกาศ"),
    "Requires": ("Defeat the Moon Lord first. The Mark of Providence drops from Providence or can be crafted.", "ต้องชนะ Moon Lord ก่อน ส่วน Mark of Providence ดรอปจาก Providence หรือคราฟต์ได้"),
}

E["Signus"] = {
    "Short": ("Sentinel of the Underworld.", "Sentinel แห่ง Underworld"),
    "Summary": ("One of the three Sentinels of the Devourer: a teleporting assassin, fought in the Underworld.", "หนึ่งใน Sentinel สามตนของ Devourer: นักฆ่าที่วาร์ปได้ สู้ใน Underworld"),
    "Summon": ("Use a [item:CalamityMod/MarkofProvidence] in the Underworld.", "ใช้ [item:CalamityMod/MarkofProvidence] ใน Underworld"),
    "Requires": ("Defeat the Moon Lord first. The Mark of Providence drops from Providence or can be crafted.", "ต้องชนะ Moon Lord ก่อน ส่วน Mark of Providence ดรอปจาก Providence หรือคราฟต์ได้"),
}

E["Polterghast"] = {
    "Short": ("Dungeon ghost; last Acid Rain tier.", "ผีแห่ง Dungeon ปลดล็อก Acid Rain ระดับสุดท้าย"),
    "Summary": ("A mass of restless souls haunting the Dungeon. Defeating it stirs the last tier of Acid Rain.", "กลุ่มวิญญาณที่ไม่สงบซึ่งหลอกหลอน Dungeon ชนะแล้ว Acid Rain ระดับสุดท้ายจะเริ่มได้"),
    "Summon": ("Kill 30 Phantom Spirits (released by strong Dungeon enemies after the Moon Lord), or use a [item:CalamityMod/NecroplasmicBeacon] in the Dungeon.",
               "กำจัด Phantom Spirit ครบ 30 ตัว (ศัตรูตัวแกร่งใน Dungeon ปล่อยออกมาหลัง Moon Lord) หรือใช้ [item:CalamityMod/NecroplasmicBeacon] ใน Dungeon"),
    "Unlocks": ("Acid Rain tier 3 (Mauler, Nuclear Terror and the Old Duke). In Revengeance its Treasure Bag holds the [item:CalamityMod/Ectoheart].",
                "Acid Rain ระดับ 3 (Mauler, Nuclear Terror และ Old Duke) และในโหมด Revengeance ถุงของมันมี [item:CalamityMod/Ectoheart]"),
    "Requires": ("Defeat the Moon Lord first.", "ต้องชนะ Moon Lord ก่อน"),
    "Tips": ("• The Phantom Spirit count resets when you leave the world.", "• จำนวน Phantom Spirit ที่กำจัดจะรีเซ็ตเมื่อออกจากโลก"),
}

E["AcidRain3"] = {
    "Name": ("Acid Rain (tier 3)", "Acid Rain (ระดับ 3)"),
    "Short": ("Final Acid Rain; ends with the Old Duke.", "Acid Rain ระดับสุดท้าย จบด้วย Old Duke"),
    "Summary": ("The last tier of Acid Rain, with the Mauler and the Nuclear Terror. Completing it summons the Old Duke.",
                "Acid Rain ระดับสุดท้าย มี Mauler และ Nuclear Terror ทำอีเวนต์ให้ครบจะเรียก Old Duke"),
    "Summon": ("Starts by itself at night or with a [item:CalamityMod/CausticTear] once Polterghast is defeated.", "เริ่มเองตอนกลางคืน หรือใช้ [item:CalamityMod/CausticTear] หลังชนะ Polterghast"),
    "Requires": ("Defeat Polterghast first.", "ต้องชนะ Polterghast ก่อน"),
}

E["Mauler"] = {
    "Short": ("Acid Rain miniboss (tier 3).", "มินิบอสของ Acid Rain (ระดับ 3)"),
    "Summary": ("An irradiated shark miniboss of the last Acid Rain tier.", "มินิบอสฉลามกลายพันธุ์ของ Acid Rain ระดับสุดท้าย"),
}

E["NuclearTerror"] = {
    "Short": ("Acid Rain miniboss (tier 3).", "มินิบอสของ Acid Rain (ระดับ 3)"),
    "Summary": ("A radioactive miniboss of the last Acid Rain tier.", "มินิบอสกัมมันตรังสีของ Acid Rain ระดับสุดท้าย"),
}

E["OldDuke"] = {
    "Short": ("Irradiated Duke Fishron.", "Duke Fishron กลายพันธุ์"),
    "Summary": ("A rotting, irradiated Duke Fishron from the Sulphurous Sea.", "Duke Fishron ที่เน่าเปื่อยและปนเปื้อนรังสีจาก Sulphurous Sea"),
    "Summon": ("Finish the tier 3 Acid Rain, or fish in the Sulphurous Sea with a [item:CalamityMod/BloodwormItem] as bait (sold by the Sea King after the first win).",
               "ทำ Acid Rain ระดับ 3 ให้ครบ หรือตกปลาใน Sulphurous Sea โดยใช้ [item:CalamityMod/BloodwormItem] เป็นเหยื่อ (Sea King ขายหลังชนะครั้งแรก)"),
    "Requires": ("Defeat Polterghast first.", "ต้องชนะ Polterghast ก่อน"),
}

E["DevourerOfGods"] = {
    "Short": ("The cosmic serpent.", "งูยักษ์แห่งจักรวาล"),
    "Summary": ("A cosmic serpent that devours gods. Defeating it upgrades the Pumpkin Moon, Frost Moon and Solar Eclipse drops.",
                "งูจักรวาลผู้กลืนกินเทพเจ้า ชนะแล้ว Pumpkin Moon, Frost Moon และ Solar Eclipse จะดรอปวัตถุดิบใหม่"),
    "Summon": ("Use a [item:CalamityMod/CosmicWorm]. It can be crafted from the three Sentinels' drops, or from Luminite, lunar fragments, Meld Blobs and Necroplasm.",
               "ใช้ [item:CalamityMod/CosmicWorm] ซึ่งคราฟต์ได้จากของดรอปของ Sentinel ทั้งสาม หรือจาก Luminite, เศษชิ้นส่วนดวงจันทร์, Meld Blob และ Necroplasm"),
    "Unlocks": ("Nightmare Fuel, Endothermic Energy and Darksun Fragments from the moon events and the Eclipse.", "Nightmare Fuel, Endothermic Energy และ Darksun Fragment จากอีเวนต์ดวงจันทร์และ Eclipse"),
    "Requires": ("Defeat the Moon Lord first.", "ต้องชนะ Moon Lord ก่อน"),
    "Tips": ("• Its attacks are fast; a strong dash and good flight are essential.", "• ท่าโจมตีเร็วมาก ต้องมีแดชและการบินที่ดี"),
}

E["Yharon"] = {
    "Short": ("The Dragon of Rebirth; unlocks Auric ore.", "มังกรแห่งการเกิดใหม่ ปลดล็อกแร่ Auric"),
    "Summary": ("The Jungle Dragon, Yharon. Defeating him releases Auric ore, needed for the final bosses.", "มังกรแห่ง Jungle นามว่า Yharon ชนะแล้วแร่ Auric จะเกิด ซึ่งจำเป็นสำหรับบอสสุดท้าย"),
    "Summon": ("Use the [item:CalamityMod/YharonEgg].", "ใช้ [item:CalamityMod/YharonEgg]"),
    "Unlocks": ("Auric ore in the caverns, and the materials for the Exo Mechs and Supreme Calamitas.", "แร่ Auric ในถ้ำ และวัตถุดิบสำหรับ Exo Mechs และ Supreme Calamitas"),
    "Requires": ("The egg needs Effulgent Feathers from the Dragonfolly.", "ไข่ต้องใช้ Effulgent Feather จาก Dragonfolly"),
}

E["ExoMechs"] = {
    "Name": ("Exo Mechs", "Exo Mechs"),
    "Short": ("Draedon's three war machines.", "เครื่องจักรสงครามสามตัวของ Draedon"),
    "Summary": ("Draedon's final machines: Thanatos, Ares and the twins Artemis and Apollo. You choose which one to fight first.",
                "เครื่องจักรสุดท้ายของ Draedon: Thanatos, Ares และแฝด Artemis กับ Apollo คุณเลือกได้ว่าจะสู้ตัวไหนก่อน"),
    "Summon": ("Fully upgrade the Codebreaker, put in an [item:CalamityMod/AuricQuantumCoolingCell] and contact Draedon.",
               "อัปเกรด Codebreaker ให้ครบ ใส่ [item:CalamityMod/AuricQuantumCoolingCell] แล้วติดต่อ Draedon"),
    "Requires": ("Defeat Yharon first: the cooling cell needs Auric Bars.", "ต้องชนะ Yharon ก่อน เพราะ Cooling Cell ต้องใช้ Auric Bar"),
    "Tips": ("• See the Draedon's labs guide for the Codebreaker upgrades.", "• ดูวิธีอัปเกรด Codebreaker ได้ในคู่มือห้องแล็บของ Draedon"),
}

E["SupremeCalamitas"] = {
    "Short": ("The Brimstone Witch herself.", "แม่มด Brimstone ตัวจริง"),
    "Summary": ("Supreme Witch, Calamitas: one of the two final bosses of Calamity. Defeating her lets the Brimstone Witch move in.",
                "Supreme Witch, Calamitas: หนึ่งในบอสสุดท้ายสองตัวของ Calamity ชนะแล้วแม่มด Brimstone จะย้ายเข้ามาอยู่ได้"),
    "Summon": ("Offer [item:CalamityMod/AshesofCalamity] (or a [item:CalamityMod/CeremonialUrn] for later fights) at an [item:CalamityMod/AltarOfTheAccursedItem].",
               "ถวาย [item:CalamityMod/AshesofCalamity] (หรือ [item:CalamityMod/CeremonialUrn] สำหรับการสู้ครั้งต่อไป) ที่ [item:CalamityMod/AltarOfTheAccursedItem]"),
    "Unlocks": ("The Brimstone Witch (Calamitas) as a town NPC; she can enchant your items.", "Brimstone Witch (Calamitas) เป็น NPC ในเมือง เธอเสริมเวทมนตร์ให้ไอเทมได้"),
    "Requires": ("The Altar of the Accursed needs Auric Bars (after Yharon).", "Altar of the Accursed ต้องใช้ Auric Bar (หลัง Yharon)"),
}

E["PrimordialWyrm"] = {
    "Short": ("Ancient wyrm of the deepest Abyss.", "งูโบราณแห่งก้น Abyss"),
    "Summary": ("An ancient wyrm living in the deepest layer of the Abyss. It becomes enraged outside that layer.", "งูโบราณที่อาศัยในชั้นลึกที่สุดของ Abyss จะคลั่งเมื่ออยู่นอกชั้นนั้น"),
    "Summon": ("It appears when someone in the Abyss gets Chaos State, for example by teleporting with a Rod of Discord. With Infernum it can also be summoned with an Evoking Searune in the deepest layer.",
                  "มันจะปรากฏเมื่อมีผู้เล่นใน Abyss ติดสถานะ Chaos State เช่น วาร์ปด้วย Rod of Discord และถ้าเปิด Infernum จะเรียกด้วย Evoking Searune ในชั้นลึกสุดได้ด้วย"),
    "Unlocks": ("It drops a [item:CalamityMod/Terminus].", "ดรอป [item:CalamityMod/Terminus]"),
    "Warning": ("This works at any point of the game: teleporting in the Abyss early means a fight you cannot win.",
                "เรื่องนี้เกิดได้ทุกช่วงของเกม ถ้าวาร์ปใน Abyss เร็วเกินไปจะต้องเจอบอสที่ยังสู้ไม่ได้"),
}

E["BossRush"] = {
    "Name": ("Boss Rush", "Boss Rush"),
    "Short": ("Fight every boss in a row.", "สู้บอสทุกตัวต่อเนื่องกัน"),
    "Summary": ("An extreme challenge: every boss in order, massively strengthened, with no breaks.", "ความท้าทายสุดขีด: สู้บอสทุกตัวตามลำดับโดยเสริมพลังอย่างมาก และไม่มีช่วงพัก"),
    "Summon": ("Use the [item:CalamityMod/Terminus] (hold it to channel). It can be used at any time; use it again to stop.",
               "ใช้ [item:CalamityMod/Terminus] (กดค้างเพื่อร่าย) ใช้ได้ทุกเวลา และใช้อีกครั้งเพื่อหยุด"),
    "Warning": ("Boss Rush bosses are far stronger than normal. Start it only when you mean it.", "บอสใน Boss Rush แข็งแกร่งกว่าปกติมาก เริ่มเมื่อพร้อมจริง ๆ เท่านั้น"),
}

# ====================================================================== Catalyst 1.1.8

E["Astrageldon"] = {
    "Short": ("Astral slime; must be beaten before the Moon Lord.", "สไลม์ดวงดาว ต้องชนะก่อน Moon Lord"),
    "Summary": ("A giant astral slime from the Catalyst mod.", "สไลม์ดวงดาวยักษ์จากม็อด Catalyst"),
    "Summon": ("Use the [item:CatalystMod/AstralCommunicator] on the surface of the Astral Infection. The communicator is not consumed and drops from Ascended Astral Slimes.",
               "ใช้ [item:CatalystMod/AstralCommunicator] บนผิวดินของ Astral Infection อุปกรณ์นี้ไม่หายเมื่อใช้ และได้จากการกำจัด Ascended Astral Slime"),
    "Warning": ("Defeat Astrageldon before the Moon Lord: afterwards the Astral Communicator stops working unless Astrageldon was already killed.",
                "ต้องชนะ Astrageldon ก่อน Moon Lord เพราะหลังจากนั้น Astral Communicator จะใช้ไม่ได้ ถ้ายังไม่เคยชนะ Astrageldon"),
    "Requires": ("The Moon Lord was defeated before Astrageldon: in this world it can no longer be summoned.", "ถ้าชนะ Moon Lord ก่อน Astrageldon ไปแล้ว จะเรียกมันในโลกนี้ไม่ได้อีก"),
}

# ====================================================================== Infernum 2.0.1.35

E["Dreadnautilus"] = {
    "Short": ("Blood Moon nautilus boss (Infernum).", "บอสหอยงวงแห่ง Blood Moon (Infernum)"),
    "Summary": ("Infernum turns the Blood Moon's Dreadnautilus into a full boss.", "Infernum เปลี่ยน Dreadnautilus ของ Blood Moon ให้เป็นบอสเต็มตัว"),
    "Summon": ("Use [item:InfernumMode/RedBait] at night (not consumed), or fish it up during a Blood Moon.", "ใช้ [item:InfernumMode/RedBait] ตอนกลางคืน (ไม่หายเมื่อใช้) หรือตกขึ้นมาระหว่าง Blood Moon"),
    "Requires": ("Hardmode, with Infernum on.", "ต้องเป็น Hardmode และเปิด Infernum"),
}

E["BereftVassal"] = {
    "Short": ("Infernum's desert warrior boss.", "บอสนักรบแห่งทะเลทรายของ Infernum"),
    "Summary": ("A new boss added by Infernum: a lone warrior fought with the Great Sand Shark in the heart of the desert.", "บอสใหม่จาก Infernum: นักรบผู้โดดเดี่ยวที่สู้พร้อม Great Sand Shark ใจกลางทะเลทราย"),
    "Summon": ("Use a [item:CalamityMod/SandstormsCore] at the pedestal in the heart of the desert.", "ใช้ [item:CalamityMod/SandstormsCore] ที่แท่นใจกลางทะเลทราย"),
    "Requires": ("Defeat the Lunatic Cultist first, with Infernum on.", "ต้องชนะ Lunatic Cultist ก่อน และเปิด Infernum"),
}

# ====================================================================== Hunt of the Old God 1.2.3

E["Goozma"] = {
    "Short": ("The slime god of Hunt of the Old God.", "เทพสไลม์แห่ง Hunt of the Old God"),
    "Summary": ("Goozma, the ancient slime god. A very hard fight designed for the end of the game, though it can be summoned in Hardmode.",
                "Goozma เทพสไลม์โบราณ เป็นการต่อสู้ที่ยากมากซึ่งออกแบบไว้สำหรับช่วงท้ายเกม แม้จะเรียกได้ตั้งแต่ Hardmode"),
    "Summon": ("Throw an [item:CalamityMod/OverloadedSludge] onto King Slime during a Slime Rain, or use a [item:CalamityHunt/PluripotentSpawnEgg] on an empowered Slime Ninja Statue.",
               "โยน [item:CalamityMod/OverloadedSludge] ใส่ King Slime ระหว่าง Slime Rain หรือใช้ [item:CalamityHunt/PluripotentSpawnEgg] กับ Slime Ninja Statue ที่ถูกเสริมพลัง"),
    "Unlocks": ("The [item:CalamityHunt/GoozmaSoul]: touch it to absorb it for permanent stats (see the Upgrades tab). With Hunt's Shadowspec Curse option on (the default), items crafted from Shadowspec Bars can only be used after Goozma is defeated.", "[item:CalamityHunt/GoozmaSoul]: เดินไปแตะเพื่อดูดซับรับค่าสถานะถาวร (ดูในแท็บอัปเกรดถาวร) และถ้าเปิดตัวเลือก Shadowspec Curse ของ Hunt ไว้ (ค่าเริ่มต้น) ไอเทมที่คราฟต์จาก Shadowspec Bar จะใช้ได้หลังชนะ Goozma แล้วเท่านั้น"),
    "Requires": ("Hardmode.", "ต้องเป็น Hardmode"),
    "Warning": ("It is balanced for the end of the game; summoning it early is a trap.", "บอสนี้ปรับสมดุลไว้สำหรับช่วงท้ายเกม ถ้าเรียกมาสู้เร็วเกินไปจะเสียเปรียบมาก"),
}

# ====================================================================== Wrath of the Gods 1.2.31

E["SolynMeet"] = {
    "Name": ("Meet Solyn", "พบกับ Solyn"),
    "Short": ("Wrath of the Gods: meet the star traveler.", "Wrath of the Gods: พบนักเดินทางแห่งดวงดาว"),
    "Summary": ("Solyn, a traveler from the stars, starts the story of Wrath of the Gods.", "Solyn นักเดินทางจากดวงดาว คือจุดเริ่มต้นของเนื้อเรื่อง Wrath of the Gods"),
    "Summon": ("After Skeletron (or in Hardmode) Solyn can appear in your world. Find her and talk to her until the introduction ends.",
               "หลังชนะ Skeletron (หรือเมื่อเข้า Hardmode) Solyn จะปรากฏในโลกของคุณได้ ตามหาเธอแล้วคุยจนบทแนะนำตัวจบ"),
    "Requires": ("Defeat Skeletron first.", "ต้องชนะ Skeletron ก่อน"),
}

E["SolynCamp"] = {
    "Name": ("Solyn's camp", "แคมป์ของ Solyn"),
    "Short": ("Choose where Solyn sets up camp.", "เลือกที่ตั้งแคมป์ให้ Solyn"),
    "Summary": ("Solyn wants a place to set up camp in your world.", "Solyn อยากได้ที่ตั้งแคมป์ในโลกของคุณ"),
    "Summon": ("Talk to Solyn: she gives you a [item:NoxusBoss/StrangeFlagpole]. Place it on open, flat ground to choose the camp site.",
               "คุยกับ Solyn เธอจะมอบ [item:NoxusBoss/StrangeFlagpole] ให้ วางบนพื้นเรียบที่โล่งเพื่อเลือกที่ตั้งแคมป์"),
    "Requires": ("Meet Solyn first.", "ต้องพบ Solyn ก่อน"),
}

E["SolynStargaze"] = {
    "Name": ("Stargazing", "ดูดาว"),
    "Short": ("Repair the telescope and stargaze with Solyn.", "ซ่อมกล้องโทรทรรศน์แล้วดูดาวกับ Solyn"),
    "Summary": ("Solyn wants to go stargazing. The quest ends with a gift: the [item:NoxusBoss/StarCharm].", "Solyn อยากดูดาว เควสต์จบลงด้วยของขวัญ: [item:NoxusBoss/StarCharm]"),
    "Summon": ("Talk to Solyn about stargazing, then repair the telescope at her camp with 10 Gold or Platinum Bars (whichever your world has) and 15 Wood. Talk to her again to watch the sky.",
               "คุยกับ Solyn เรื่องดูดาว จากนั้นซ่อมกล้องโทรทรรศน์ที่แคมป์ด้วย Gold Bar หรือ Platinum Bar 10 แท่ง (แล้วแต่โลกของคุณมีแร่ชนิดไหน) และไม้ 15 ชิ้น แล้วคุยกับเธออีกครั้งเพื่อดูท้องฟ้า"),
    "Requires": ("Meet Solyn first.", "ต้องพบ Solyn ก่อน"),
}

E["PermafrostKeep"] = {
    "Name": ("Permafrost Keep", "ป้อมปราการ Permafrost"),
    "Short": ("Explore a frozen keep with Solyn.", "สำรวจป้อมปราการน้ำแข็งกับ Solyn"),
    "Summary": ("A frozen keep hidden in the snow. Solyn joins you to explore it.", "ป้อมปราการน้ำแข็งที่ซ่อนอยู่ในหิมะ Solyn จะไปสำรวจด้วยกัน"),
    "Summon": ("After Cryogen, Permafrost gives you the [item:NoxusBoss/DormantKey] as thanks for freeing him. Show it to Solyn, then open the keep with her.",
               "หลังชนะ Cryogen แล้ว Permafrost จะมอบ [item:NoxusBoss/DormantKey] ให้เป็นการขอบคุณที่ปลดปล่อยเขา นำไปคุยกับ Solyn แล้วเปิดป้อมปราการไปพร้อมกับเธอ"),
    "Requires": ("Finish the stargazing quest and defeat Cryogen.", "ทำเควสต์ดูดาวให้เสร็จและชนะ Cryogen"),
}

E["GenesisReveal"] = {
    "Name": ("The Genesis", "The Genesis"),
    "Short": ("Solyn tells you about the Genesis.", "Solyn เล่าเรื่อง Genesis"),
    "Summary": ("After the Moon Lord a rift appears in the sky; Solyn explains the Genesis.", "หลัง Moon Lord จะมีรอยแยกปรากฏบนท้องฟ้า Solyn จะอธิบายเรื่อง Genesis"),
    "Summon": ("Talk to Solyn once the rift is in the sky after the Moon Lord.", "คุยกับ Solyn เมื่อรอยแยกปรากฏบนฟ้าหลังชนะ Moon Lord"),
    "Requires": ("Finish Permafrost Keep and defeat the Moon Lord.", "ทำเควสต์ Permafrost Keep ให้เสร็จและชนะ Moon Lord"),
}

E["Antiseed"] = {
    "Name": ("The Antiseed", "The Antiseed"),
    "Short": ("Enter the rift with Solyn for the Antiseed.", "เข้าไปในรอยแยกกับ Solyn เพื่อรับ Antiseed"),
    "Summary": ("Solyn asks for help with the Ceaseless Void's rift. Inside waits [item:NoxusBoss/TheAntiseed].", "Solyn ขอความช่วยเหลือเรื่องรอยแยกของ Ceaseless Void ข้างในมี [item:NoxusBoss/TheAntiseed] รออยู่"),
    "Summon": ("After Providence, talk to Solyn about the Ceaseless Void, defeat it, then follow her into the rift.", "หลังชนะ Providence ให้คุยกับ Solyn เรื่อง Ceaseless Void จากนั้นชนะ Ceaseless Void แล้วตามเธอเข้าไปในรอยแยก"),
    "Requires": ("Learn about the Genesis and defeat Providence.", "ต้องฟังเรื่อง Genesis และชนะ Providence"),
}

E["Mars"] = {
    "Short": ("Draedon's machine, fought with Solyn.", "เครื่องจักรของ Draedon สู้ร่วมกับ Solyn"),
    "Summary": ("A war machine of Draedon, fought alongside Solyn in a special quest.", "เครื่องจักรสงครามของ Draedon ที่สู้ร่วมกับ Solyn ในเควสต์พิเศษ"),
    "Summon": ("Start the special quest with Solyn, then contact Draedon with the Codebreaker.", "เริ่มเควสต์พิเศษกับ Solyn แล้วติดต่อ Draedon ด้วย Codebreaker"),
    "Requires": ("Get the Antiseed and defeat the Exo Mechs.", "ต้องได้ Antiseed และชนะ Exo Mechs"),
}

E["AvatarOfEmptiness"] = {
    "Short": ("Wrath of the Gods boss: activate the Genesis.", "บอสของ Wrath of the Gods: เปิดใช้ Genesis"),
    "Summary": ("The Avatar of Emptiness appears when the Genesis is activated.", "Avatar of Emptiness จะปรากฏเมื่อเปิดใช้ Genesis"),
    "Summon": ("Construct and activate the Genesis.", "สร้างและเปิดใช้ Genesis"),
    "Requires": ("Defeat Mars and complete the Genesis.", "ต้องชนะ Mars และสร้าง Genesis ให้เสร็จ"),
}

E["NamelessDeity"] = {
    "Short": ("The final god of Wrath of the Gods.", "เทพองค์สุดท้ายของ Wrath of the Gods"),
    "Summary": ("The Nameless Deity, the final challenge of Wrath of the Gods.", "Nameless Deity ความท้าทายสุดท้ายของ Wrath of the Gods"),
    "Summon": ("Use the [item:CalamityMod/Terminus] after its gaze has opened.", "ใช้ [item:CalamityMod/Terminus] หลังจากดวงตาของมันเปิดขึ้น"),
    "Requires": ("Defeat the Avatar of Emptiness first.", "ต้องชนะ Avatar of Emptiness ก่อน"),
}

# ====================================================================== Calamity Fables 0.1.24

E["SirNautilus"] = {
    "Short": ("Fables: a lone soldier under the dunes.", "Fables: ทหารผู้โดดเดี่ยวใต้เนินทราย"),
    "Summary": ("A lone soldier from Calamity Fables, waiting in a hidden chamber under the desert.", "ทหารผู้โดดเดี่ยวจาก Calamity Fables ที่รออยู่ในห้องลับใต้ทะเลทราย"),
    "Summon": ("Find his hidden chamber beneath the dunes; an ancient sandstone graveyard marks the spot.", "ตามหาห้องลับของเขาใต้เนินทราย สุสานหินทรายโบราณคือจุดสังเกต"),
}

E["FablesDesertScourge"] = {
    "Short": ("Fables' reworked Desert Scourge.", "Desert Scourge เวอร์ชันปรับใหม่ของ Fables"),
    "Summary": ("Calamity Fables' version of the Desert Scourge. With Calamity loaded it replaces Calamity's Desert Scourge while Fables' \"replace Calamity\" option is on (the default).",
                "Desert Scourge เวอร์ชันของ Calamity Fables ถ้าเปิด Calamity อยู่ จะใช้แทน Desert Scourge ของ Calamity ขณะที่ตัวเลือก \"replace Calamity\" ของ Fables เปิดอยู่ (ค่าเริ่มต้น)"),
    "Summon": ("Lure it out with stormlion grubs: [item:CalamityFables/StormlionLarvaItem], [item:CalamityFables/DeadStormlionLarvaItem] or the [item:CalamityFables/BucketOfLarvae].",
               "ล่อมันออกมาด้วยตัวอ่อน Stormlion: [item:CalamityFables/StormlionLarvaItem], [item:CalamityFables/DeadStormlionLarvaItem] หรือ [item:CalamityFables/BucketOfLarvae]"),
}

E["ScourgeVsScarab"] = {
    "Short": ("Duel: Desert Scourge vs. Scarabeus.", "ศึกดวล: Desert Scourge ปะทะ Scarabeus"),
    "Summary": ("A special duel added by Calamity Fables when Spirit Reforged is loaded: the Desert Scourge and Scarabeus fight each other and you.",
                "ศึกดวลพิเศษจาก Calamity Fables เมื่อเปิด Spirit Reforged ด้วย: Desert Scourge และ Scarabeus จะสู้กันเองและสู้กับคุณ"),
    "Summon": ("Offer a stormlion grub ([item:CalamityFables/StormlionLarvaItem], [item:CalamityFables/DeadStormlionLarvaItem] or [item:CalamityFables/BucketOfLarvae]) on an [item:SpiritReforged/ScarabAltarItem].",
               "ถวายตัวอ่อน Stormlion ([item:CalamityFables/StormlionLarvaItem], [item:CalamityFables/DeadStormlionLarvaItem] หรือ [item:CalamityFables/BucketOfLarvae]) บน [item:SpiritReforged/ScarabAltarItem]"),
}

E["FablesCrabulon"] = {
    "Short": ("Fables' reworked Crabulon.", "Crabulon เวอร์ชันปรับใหม่ของ Fables"),
    "Summary": ("Calamity Fables' version of Crabulon. With Calamity loaded it replaces Calamity's Crabulon while Fables' \"replace Calamity\" option is on (the default).",
                "Crabulon เวอร์ชันของ Calamity Fables ถ้าเปิด Calamity อยู่ จะใช้แทน Crabulon ของ Calamity ขณะที่ตัวเลือก \"replace Calamity\" ของ Fables เปิดอยู่ (ค่าเริ่มต้น)"),
    "Summon": ("Before its first defeat it can peek out naturally in the mushroom caves. Draw its attention with a [item:CalamityFables/Hookstaff] used while standing on mushroom grass underground.",
               "ก่อนถูกชนะครั้งแรก มันอาจโผล่ออกมาเองในถ้ำเห็ด ดึงความสนใจของมันด้วย [item:CalamityFables/Hookstaff] โดยใช้ขณะยืนบนหญ้าเห็ดใต้ดิน"),
}

# ====================================================================== Spirit Reforged 0.2.2.8

E["Scarabeus"] = {
    "Short": ("Spirit Reforged: giant scarab of the Ziggurat.", "Spirit Reforged: แมลงปีกแข็งยักษ์แห่ง Ziggurat"),
    "Summary": ("A giant scarab beetle from Spirit Reforged, fought at the desert Ziggurat.", "แมลงปีกแข็งยักษ์จาก Spirit Reforged สู้ที่ Ziggurat ในทะเลทราย"),
    "Summon": ("Use any gem at the [item:SpiritReforged/ScarabAltarItem] on the Ziggurat during the day.", "ใช้อัญมณีชนิดใดก็ได้ที่ [item:SpiritReforged/ScarabAltarItem] บน Ziggurat ตอนกลางวัน"),
}

GUIDE = {"Entries": E}
