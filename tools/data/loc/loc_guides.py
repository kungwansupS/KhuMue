# Guide articles (Data/guides.json). Every leaf is (English, Thai).
# Section bodies may use {0} for the key that opens the guide. A "...Cal" key replaces the base text while
# Calamity is loaded. Every claim was checked against the game code of the supported versions.

G = {}

G["using-guide"] = {
    "Title": ("How to use Khu Mue", "วิธีใช้ Khu Mue"),
    "Summary": ("What each tab does and how to find things fast.", "แต่ละแท็บทำอะไร และหาข้อมูลให้เร็วได้อย่างไร"),
    "Open": {"Title": ("Opening the guide", "การเปิดคู่มือ"),
             "Body": ("Press {0} to open or close the guide; Esc also closes it. The mouse back button (or the arrow at the top) returns to the previous page. You can change the key in Settings > Controls.",
                      "กด {0} เพื่อเปิดหรือปิดคู่มือ และกด Esc เพื่อปิดได้ ปุ่มย้อนกลับของเมาส์ (หรือลูกศรด้านบน) พากลับไปหน้าก่อน เปลี่ยนปุ่มได้ที่ Settings > Controls")},
    "Home": {"Title": ("Home", "หน้าแรก"),
             "Body": ("Home reads your world and shows where you are, the next main step with how to start it, other things you can do now, and permanent upgrades you have not used yet.",
                      "หน้าแรกอ่านสถานะโลกของคุณ แล้วแสดงว่าตอนนี้อยู่ช่วงไหน ขั้นหลักถัดไปพร้อมวิธีเริ่ม สิ่งอื่นที่ทำได้ตอนนี้ และอัปเกรดถาวรที่ยังไม่ได้ใช้")},
    "Progress": {"Title": ("Progression", "ความก้าวหน้า"),
                 "Body": ("Every boss, event and quest of your mods in order, marked as done, next, available or locked. Click one for how to start it, what it unlocks and its drops (read from the game).",
                          "บอส อีเวนต์ และเควสต์ทั้งหมดจากม็อดที่เปิดอยู่ เรียงตามลำดับ พร้อมสถานะ \"ผ่านแล้ว\" \"ถัดไป\" \"สามารถทำได้\" หรือ \"ยังล็อกอยู่\" คลิกเพื่อดูวิธีเริ่ม สิ่งที่ปลดล็อก และของที่ดรอป (อ่านจากเกม)")},
    "Gear": {"Title": ("Gear", "ชุดแนะนำ"),
             "Body": ("Recommended weapons, armor, accessories and buffs for your class and stage, adapted from the Calamity and Terraria wikis. It opens on your world's stage; the arrows browse other stages.",
                      "อาวุธ ชุดเกราะ เครื่องประดับ และบัฟที่แนะนำตามคลาสและช่วงของคุณ ดัดแปลงจากวิกิของ Calamity และ Terraria หน้านี้จะเปิดที่ช่วงของโลกคุณ ใช้ลูกศรเพื่อดูช่วงอื่น")},
    "Items": {"Title": ("Items", "ไอเทม"),
              "Body": ("Search any item in English or Thai. Its page shows every recipe with the base materials it needs in total, drops, shops, bags, Shimmer results, the chests and fishing spots it comes from and what it is used for. A green dot means you own it.",
                       "ค้นหาไอเทมได้ทั้งชื่ออังกฤษและไทย หน้าไอเทมแสดงสูตรคราฟต์พร้อมวัตถุดิบพื้นฐานที่ต้องใช้ทั้งหมด ดรอป ร้านค้า ถุง ผลลัพธ์ Shimmer หีบและจุดตกปลาที่พบไอเทมนั้น และนำไปคราฟต์อะไรได้บ้าง จุดสีเขียวหมายถึงคุณมีไอเทมนั้น")},
    "Upgrades": {"Title": ("Upgrades", "อัปเกรดถาวร"),
                 "Body": ("Every permanent upgrade (Life Crystals, Calamity fruits, Rage and Adrenaline boosters...) with what this character has already used.",
                          "อัปเกรดถาวรทั้งหมด (Life Crystal, ผลไม้ของ Calamity, บูสเตอร์ Rage และ Adrenaline...) พร้อมบอกว่าตัวละครนี้ใช้อะไรไปแล้วบ้าง")},
}

G["first-steps"] = {
    "Title": ("First steps", "ก้าวแรกของการเล่น"),
    "Summary": ("Survive the first nights and get ready for the first bosses.", "เอาตัวรอดคืนแรก ๆ และเตรียมพร้อมสำหรับบอสตัวแรก"),
    "Shelter": {"Title": ("A safe house", "บ้านที่ปลอดภัย"),
                "Body": ("Build a house before night. A room with background walls, a door, a light source, a table and a chair counts as housing, and new town NPCs move into such rooms.",
                         "สร้างบ้านก่อนค่ำ ห้องที่มีผนังพื้นหลัง ประตู แหล่งแสง โต๊ะ และเก้าอี้ จะนับเป็นที่อยู่อาศัย และ NPC ใหม่จะย้ายเข้ามาอยู่ในห้องแบบนี้")},
    "Explore": {"Title": ("Ore and gear", "แร่และอุปกรณ์"),
                "Body": ("Dig down for ore and upgrade your pickaxe and armor step by step. Chests underground often hold useful accessories such as Hermes Boots and Cloud in a Bottle.",
                         "ขุดลงไปหาแร่ แล้วอัปเกรด Pickaxe และชุดเกราะทีละขั้น หีบใต้ดินมักมีเครื่องประดับที่มีประโยชน์ เช่น Hermes Boots และ Cloud in a Bottle")},
    "Life": {"Title": ("More life and mana", "เพิ่มพลังชีวิตและมานา"),
             "Body": ("Each Life Crystal gives +20 max life (up to 400). Mana Crystals are crafted from 5 Fallen Stars, which fall at night. The Upgrades tab tracks them.",
                      "Life Crystal แต่ละชิ้นเพิ่มพลังชีวิตสูงสุด 20 (ได้ถึง 400) ส่วน Mana Crystal คราฟต์จาก Fallen Star 5 ชิ้น ซึ่งตกลงมาตอนกลางคืน ดูรายการทั้งหมดได้ที่แท็บอัปเกรดถาวร")},
    "Progress": {"Title": ("What next?", "ทำอะไรต่อ?"),
                 "Body": ("The Home tab always shows your next main step. Open it for how to start the fight and what gear to bring.",
                          "แท็บหน้าแรกจะแสดงขั้นหลักถัดไปเสมอ เปิดดูเพื่อรู้วิธีเริ่มการต่อสู้และอุปกรณ์ที่ควรเตรียม")},
}

G["difficulties"] = {
    "Title": ("World difficulty", "ระดับความยากของโลก"),
    "Summary": ("What Classic, Expert, Master and Journey change.", "Classic, Expert, Master และ Journey ต่างกันอย่างไร"),
    "SummaryCal": ("What Classic, Expert, Master and Journey change. Calamity's own modes are explained in the Calamity section.",
                   "Classic, Expert, Master และ Journey ต่างกันอย่างไร ส่วนโหมดของ Calamity อธิบายไว้ในหมวด Calamity"),
    "Classic": {"Title": ("Classic", "Classic"),
                "Body": ("The normal game. Bosses drop their items directly instead of in Treasure Bags.",
                         "เกมปกติ บอสดรอปไอเทมโดยตรงแทนที่จะอยู่ใน Treasure Bag")},
    "Expert": {"Title": ("Expert", "Expert"),
               "Body": ("Enemies have more health and damage, bosses gain new attacks, and bosses drop Treasure Bags with an exclusive Expert item. The Demon Heart's extra accessory slot only works here and in Master.",
                        "ศัตรูมีพลังชีวิตและดาเมจสูงขึ้น บอสมีท่าโจมตีใหม่ และบอสดรอป Treasure Bag ที่มีไอเทม Expert เฉพาะ ช่องเครื่องประดับเพิ่มจาก Demon Heart ใช้ได้เฉพาะโหมดนี้และ Master")},
    "Master": {"Title": ("Master", "Master"),
               "Body": ("Even stronger than Expert. Bosses also drop relics and pets, you get one extra accessory slot from the start, and you drop all your coins on death.",
                        "แข็งแกร่งกว่า Expert อีกขั้น บอสดรอป Relic และสัตว์เลี้ยงเพิ่ม คุณได้ช่องเครื่องประดับเพิ่ม 1 ช่องตั้งแต่เริ่ม และจะทำเหรียญหล่นทั้งหมดเมื่อตาย")},
    "Journey": {"Title": ("Journey", "Journey"),
                "Body": ("A creative mode: research items to duplicate them, control time and weather, and set enemy difficulty with a slider. Journey characters can only play Journey worlds.",
                         "โหมดสร้างสรรค์: วิจัยไอเทมเพื่อเสกซ้ำได้ ควบคุมเวลาและสภาพอากาศ และปรับความยากของศัตรูด้วยแถบเลื่อน ตัวละคร Journey เล่นได้เฉพาะโลก Journey")},
}

G["classes"] = {
    "Title": ("Classes", "คลาส"),
    "Summary": ("The damage classes and how they fit together.", "คลาสดาเมจแต่ละแบบและการใช้ร่วมกัน"),
    "Melee": {"Title": ("Melee", "Melee"),
              "Body": ("Swords, spears, yoyos and boomerangs. Melee armor tends to have the most defense.",
                       "ดาบ หอก โยโย่ และบูมเมอแรง ชุดเกราะสาย Melee มักมีพลังป้องกันสูงที่สุด")},
    "Ranged": {"Title": ("Ranged", "Ranged"),
               "Body": ("Bows and guns that use arrows and bullets, plus launchers. Pick the ammo for the job: piercing for crowds, strong single shots for bosses.",
                        "ธนูและปืนที่ใช้ลูกธนูและกระสุน รวมถึงเครื่องยิงต่าง ๆ เลือกกระสุนให้เหมาะกับงาน กระสุนเจาะทะลุสำหรับศัตรูเป็นกลุ่ม กระสุนแรงนัดเดียวสำหรับบอส")},
    "Magic": {"Title": ("Magic", "Magic"),
              "Body": ("Weapons that spend mana. Max mana, mana regeneration and Mana Potions keep you firing.",
                       "อาวุธที่ใช้มานา มานาสูงสุด การฟื้นมานา และ Mana Potion ช่วยให้ยิงได้ต่อเนื่อง")},
    "Summoner": {"Title": ("Summoner", "Summoner"),
                 "Body": ("Minions and sentries fight for you, and whips focus them on a target and boost them.",
                          "มินเนียนและ Sentry จะสู้แทนคุณ ส่วนแส้ใช้สั่งให้โฟกัสเป้าหมายและเสริมพลังให้มินเนียน")},
    "Mixing": {"Title": ("Mixing classes", "การผสมคลาส"),
               "Body": ("Minions keep attacking while you use any weapon, so summoning gear always adds damage.",
                        "มินเนียนยังโจมตีต่อไปขณะคุณใช้อาวุธใดก็ได้ อุปกรณ์สาย Summon จึงเพิ่มดาเมจได้เสมอ"),
               "BodyCal": ("In Calamity, holding a melee, ranged, magic or rogue weapon makes your minions deal 25% less damage. Calamity also adds the Rogue class (see the Rogue guide).",
                           "ใน Calamity ถ้าถืออาวุธ Melee, Ranged, Magic หรือ Rogue มินเนียนจะทำดาเมจลดลง 25% และ Calamity ยังเพิ่มคลาส Rogue (ดูคู่มือ Rogue)")},
}

G["boss-prep"] = {
    "Title": ("Preparing for a boss", "เตรียมตัวสู้บอส"),
    "Summary": ("Arenas, buffs and the things that save a fight.", "สนามสู้ บัฟ และสิ่งที่ช่วยให้รอด"),
    "Arena": {"Title": ("Build an arena", "สร้างสนามสู้"),
              "Body": ("Several long rows of platforms give room to dodge. A Campfire, a Heart Lantern and a Star in a Bottle nearby add regeneration; standing in honey heals too.",
                       "แพลตฟอร์มยาวหลายชั้นช่วยให้มีที่หลบ วาง Campfire, Heart Lantern และ Star in a Bottle ไว้ใกล้ ๆ เพื่อเพิ่มการฟื้นฟู และการยืนในน้ำผึ้งก็ช่วยฟื้นพลังชีวิต")},
    "Buffs": {"Title": ("Buffs", "บัฟ"),
              "Body": ("Ironskin, Regeneration, Swiftness and Endurance Potions and a food buff help every class. The Gear tab lists class buffs for each stage.",
                       "Ironskin, Regeneration, Swiftness, Endurance Potion และบัฟจากอาหาร ช่วยได้ทุกคลาส แท็บชุดแนะนำมีบัฟเฉพาะคลาสของแต่ละช่วง")},
    "Mobility": {"Title": ("Mobility first", "การเคลื่อนที่มาก่อน"),
                 "Body": ("Wings, dashes and extra jumps prevent more damage than armor. Upgrade them before anything else.",
                          "ปีก การแดช และการกระโดดหลายครั้ง ช่วยลดดาเมจได้มากกว่าชุดเกราะ ควรอัปเกรดก่อนอย่างอื่น")},
    "Healing": {"Title": ("Healing", "การฟื้นพลัง"),
                "Body": ("Healing potions share a cooldown (Potion Sickness), so drink at the right moment instead of too early.",
                         "ยาฟื้นพลังใช้คูลดาวน์ร่วมกัน (Potion Sickness) ดื่มในจังหวะที่จำเป็น ไม่ใช่เร็วเกินไป")},
}

G["hardmode"] = {
    "Title": ("Entering Hardmode", "การเข้าสู่ Hardmode"),
    "Summary": ("What changes after the Wall of Flesh.", "อะไรเปลี่ยนไปหลังชนะ Wall of Flesh"),
    "Changes": {"Title": ("What changes", "อะไรเปลี่ยนไป"),
                "Body": ("Stronger enemies appear everywhere, and a Hallow stripe and a second evil stripe cut through the world.",
                         "ศัตรูที่แรงขึ้นจะปรากฏทุกที่ และแถบ Hallow กับแถบสายชั่วร้ายแถบที่สองจะผ่ากลางโลก")},
    "Ores": {"Title": ("New ores", "แร่ใหม่"),
             "Body": ("Smash Demon or Crimson Altars with the Pwnhammer to spawn Hardmode ores: each smash adds ore of the next tier (Cobalt/Palladium, Mythril/Orichalcum, Adamantite/Titanium).",
                      "ทุบ Demon Altar หรือ Crimson Altar ด้วย Pwnhammer เพื่อให้แร่ Hardmode เกิด ทุบแต่ละครั้งจะได้แร่ระดับถัดไป (Cobalt/Palladium, Mythril/Orichalcum, Adamantite/Titanium)"),
             "BodyCal": ("Calamity's early Hardmode rework (on by default): Cobalt and Palladium appear as soon as Hardmode starts and altars give nothing. The first mechanical boss adds Mythril and Orichalcum, the second Adamantite and Titanium, the third Hallowed ore in the underground Hallow. The first two mechanical bosses you fight are also a little weaker.",
                         "ระบบปรับ Hardmode ช่วงต้นของ Calamity (เปิดเป็นค่าเริ่มต้น): แร่ Cobalt และ Palladium จะเกิดทันทีเมื่อเข้า Hardmode และการทุบแท่นบูชาจะไม่ให้อะไร บอสจักรกลตัวแรกเพิ่ม Mythril และ Orichalcum ตัวที่สองเพิ่ม Adamantite และ Titanium ตัวที่สามเพิ่มแร่ Hallowed ใน Hallow ใต้ดิน และบอสจักรกลสองตัวแรกที่คุณสู้จะอ่อนลงเล็กน้อยด้วย")},
    "Spread": {"Title": ("The spread", "การแพร่กระจาย"),
               "Body": ("The Hallow and the Corruption or Crimson slowly convert nearby grass, stone, sand and ice. Protect important areas with gaps or blocks that do not convert.",
                        "Hallow และ Corruption หรือ Crimson จะค่อย ๆ เปลี่ยนหญ้า หิน ทราย และน้ำแข็งรอบ ๆ ป้องกันพื้นที่สำคัญด้วยช่องว่างหรือบล็อกที่ไม่ถูกเปลี่ยน")},
    "Next": {"Title": ("First goals", "เป้าหมายแรก"),
             "Body": ("Get wings, then take on Queen Slime and the mechanical bosses. Souls of Light and Night drop from enemies in the underground Hallow and evil biomes.",
                      "หาปีกให้ได้ แล้วสู้ Queen Slime และบอสจักรกล Soul of Light และ Soul of Night ดรอปจากศัตรูใน Hallow และไบโอมสายชั่วร้ายใต้ดิน")},
}

G["shimmer"] = {
    "Title": ("Shimmer", "Shimmer"),
    "Summary": ("The shimmering lake that transforms items.", "ทะเลสาบระยิบระยับที่เปลี่ยนรูปไอเทม"),
    "Find": {"Title": ("Where it is", "อยู่ที่ไหน"),
             "Body": ("The Shimmer lake lies in the Aether, an underground biome near the edge of the world on the side opposite the Dungeon.",
                      "ทะเลสาบ Shimmer อยู่ใน Aether ไบโอมใต้ดินใกล้ขอบโลก ฝั่งตรงข้ามกับ Dungeon")},
    "Transform": {"Title": ("Transforming items", "การเปลี่ยนรูปไอเทม"),
                  "Body": ("Throw an item into Shimmer to transmute it, and many crafted items are taken apart into their ingredients. Item pages show Shimmer results under How to get.",
                           "โยนไอเทมลงใน Shimmer เพื่อแปลงเป็นอย่างอื่น และไอเทมที่คราฟต์หลายชนิดจะถูกแยกกลับเป็นวัตถุดิบ หน้าไอเทมแสดงผลลัพธ์ Shimmer ไว้ในส่วนวิธีหา")},
    "Boosters": {"Title": ("Permanent boosters", "บูสเตอร์ถาวร"),
                 "Body": ("Life Crystal becomes [item:Terraria/AegisCrystal], Life Fruit becomes [item:Terraria/AegisFruit], Mana Crystal becomes [item:Terraria/ArcaneCrystal], a Pink Pearl becomes [item:Terraria/GalaxyPearl], a Gold Worm becomes [item:Terraria/GummyWorm] and any fruit becomes [item:Terraria/Ambrosia].",
                          "Life Crystal กลายเป็น [item:Terraria/AegisCrystal], Life Fruit กลายเป็น [item:Terraria/AegisFruit], Mana Crystal กลายเป็น [item:Terraria/ArcaneCrystal], Pink Pearl กลายเป็น [item:Terraria/GalaxyPearl], Gold Worm กลายเป็น [item:Terraria/GummyWorm] และผลไม้ชนิดใดก็ได้กลายเป็น [item:Terraria/Ambrosia]")},
    "Careful": {"Title": ("Careful", "ข้อควรระวัง"),
                "Body": ("Touching Shimmer makes you phase through blocks and fall until you leave it. It does not hurt, but it can drop you far.",
                         "การสัมผัส Shimmer จะทำให้คุณทะลุบล็อกและตกลงไปจนกว่าจะหลุดออกมา ไม่ได้รับดาเมจ แต่อาจตกลงไปไกลมาก")},
}

# ------------------------------------------------------------------ Calamity

G["fishing"] = {
    "Title": ("Fishing", "การตกปลา"),
    "Summary": ("Fishing power, pond size, lava fishing, crates and the Angler's quests.",
                "พลังการตกปลา ขนาดบ่อ การตกปลาในลาวา กล่อง (Crate) และเควสของ Angler"),
    "Basics": {"Title": ("Getting started", "เริ่มต้น"),
               "Body": ("Hold a fishing pole, keep bait in your inventory and cast into a pond. What you catch depends on the liquid, the biome and the height where the bobber lands. Every item page in this guide lists where that item can be caught.",
                        "ถือเบ็ดตกปลา มีเหยื่ออยู่ในกระเป๋า แล้วเหวี่ยงเบ็ดลงบ่อ ของที่ได้ขึ้นกับชนิดของของเหลว ไบโอม และระดับความสูงที่ทุ่นตกลงไป หน้าไอเทมในคู่มือนี้บอกไว้ว่าไอเทมแต่ละชิ้นตกได้ที่ไหน"),
               "BodyCal": ("Hold a fishing pole, keep bait in your inventory and cast into a pond. What you catch depends on the liquid, the biome and the height where the bobber lands. Calamity adds catches in the Sunken Sea, the Sulphurous Sea, the Abyss, the Astral Infection and the lava of the Brimstone Crag. Every item page in this guide lists where that item can be caught.",
                           "ถือเบ็ดตกปลา มีเหยื่ออยู่ในกระเป๋า แล้วเหวี่ยงเบ็ดลงบ่อ ของที่ได้ขึ้นกับชนิดของของเหลว ไบโอม และระดับความสูงที่ทุ่นตกลงไป Calamity เพิ่มของที่ตกได้ใน Sunken Sea, Sulphurous Sea, Abyss, Astral Infection และในลาวาของ Brimstone Crag หน้าไอเทมในคู่มือนี้บอกไว้ว่าไอเทมแต่ละชิ้นตกได้ที่ไหน")},
    "Power": {"Title": ("Fishing power", "พลังการตกปลา"),
              "Body": ("Fishing power is the pole, the bait and your bonuses added up: a Fishing Potion gives 15, an Angler Tackle Bag 10, and up to three Chum in the pond add 11, 6 and 3. The higher it is, the better your chance of rare catches. Luck changes it as well: bad luck can lower it, good luck can raise it.",
                       "พลังการตกปลาคือค่าของเบ็ด เหยื่อ และโบนัสรวมกัน Fishing Potion ให้ 15, Angler Tackle Bag ให้ 10 และ Chum ในบ่อสูงสุดสามชิ้นเพิ่มให้อีก 11, 6 และ 3 ยิ่งพลังสูง ยิ่งมีโอกาสได้ของหายาก โชค (Luck) ก็มีผลด้วย โชคไม่ดีอาจทำให้พลังลดลง ส่วนโชคดีอาจทำให้พลังเพิ่มขึ้น")},
    "Water": {"Title": ("Pond size", "ขนาดบ่อ"),
              "Body": ("A pond needs about 300 tiles of liquid for full fishing power; high up in the sky layer fewer are enough. A smaller pond lowers your fishing power.",
                       "บ่อต้องมีของเหลวประมาณ 300 ช่องจึงจะได้พลังการตกปลาเต็มที่ ถ้าตกปลาในชั้นท้องฟ้าซึ่งอยู่สูงมาก ใช้น้อยกว่านั้นได้ บ่อที่เล็กกว่านี้จะทำให้พลังการตกปลาลดลง")},
    "Lava": {"Title": ("Lava and honey", "ลาวาและน้ำผึ้ง"),
             "Body": ("Fishing in lava needs one of these: the Hotline Fishing Hook, lava bait (Magma Snail, Lavafly or Hell Butterfly), or a Lavaproof Fishing Hook or Lavaproof Tackle Bag. Honey needs nothing special.",
                      "การตกปลาในลาวาต้องมีอย่างใดอย่างหนึ่งต่อไปนี้: Hotline Fishing Hook, เหยื่อลาวา (Magma Snail, Lavafly หรือ Hell Butterfly) หรือ Lavaproof Fishing Hook / Lavaproof Tackle Bag ส่วนน้ำผึ้งตกปลาได้โดยไม่ต้องใช้อะไรเพิ่ม")},
    "Crates": {"Title": ("Crates", "กล่อง (Crate)"),
               "Body": ("About one catch in ten is a crate, one in four with a Crate Potion. Crates hold ores, bars, potions, coins and items of the biome where they were caught; in Hardmode you get the Hardmode versions.",
                        "ของที่ตกได้ประมาณ 1 ใน 10 เป็นกล่อง (Crate) และเพิ่มเป็น 1 ใน 4 เมื่อดื่ม Crate Potion ในกล่องมีแร่ แท่งโลหะ ยา เหรียญ และไอเทมของไบโอมที่ตกได้ เมื่อเข้าสู่ Hardmode จะได้กล่องรุ่น Hardmode")},
    "Angler": {"Title": ("The Angler's quests", "เควสของ Angler"),
               "Body": ("Wake the Angler sleeping at the beach. Every morning at 4:30 AM he asks for one quest fish, which can only be caught while it is his quest. Hand it in for a reward: your 5th, 10th, 15th, 20th, 25th and 30th finished quests give the Fuzzy Carrot, the Angler Hat, Vest and Pants, the Bottomless Water Bucket and the Golden Fishing Rod, and rare rewards come more often the more quests you finish.",
                        "ปลุก Angler ที่นอนหลับอยู่ริมทะเล ทุกเช้าเวลา 04:30 น. เขาจะขอปลาเควสหนึ่งตัว ซึ่งตกได้เฉพาะตอนที่เป็นเควสของเขาเท่านั้น นำไปส่งเพื่อรับรางวัล เมื่อทำเควสสำเร็จครั้งที่ 5, 10, 15, 20, 25 และ 30 จะได้ Fuzzy Carrot, Angler Hat, Angler Vest, Angler Pants, Bottomless Water Bucket และ Golden Fishing Rod ตามลำดับ และยิ่งทำเควสมาก รางวัลหายากก็ยิ่งออกบ่อยขึ้น")},
}

G["cal-difficulty"] = {
    "Title": ("Calamity difficulty modes", "โหมดความยากของ Calamity"),
    "Summary": ("Revengeance, Death and Malice, and how to switch.", "Revengeance, Death, Malice และวิธีเปลี่ยนโหมด"),
    "Select": {"Title": ("Choosing a mode", "การเลือกโหมด"),
               "Body": ("Open your inventory: Calamity's difficulty icon sits at the top right, next to the minimap. Click it to choose a mode. It cannot be changed while a boss is alive or during Boss Rush.",
                        "เปิดช่องเก็บของ ไอคอนความยากของ Calamity อยู่มุมขวาบนข้างแผนที่ย่อ คลิกเพื่อเลือกโหมด เปลี่ยนไม่ได้ระหว่างที่มีบอสอยู่หรือระหว่าง Boss Rush")},
    "Revengeance": {"Title": ("Revengeance", "Revengeance"),
                    "Body": ("Needs Expert Mode (in a Master world Calamity turns it into Death by itself). The intended Calamity experience and what the Gear tab assumes. Enables Rage and Adrenaline, makes enemies more aggressive, gives every boss new attacks, and bosses drop both Expert and Master rewards.",
                             "ต้องเป็นโหมด Expert (ในโลก Master Calamity จะเปลี่ยนเป็น Death ให้เอง) เป็นประสบการณ์ที่ Calamity ตั้งใจให้เล่น และเป็นโหมดที่แท็บชุดแนะนำใช้เป็นฐาน เปิดใช้ Rage และ Adrenaline ศัตรูดุขึ้น บอสทุกตัวมีท่าใหม่ และบอสดรอปทั้งรางวัล Expert และ Master")},
    "Death": {"Title": ("Death", "Death"),
              "Body": ("Needs Master Mode: in a Master world Calamity switches Revengeance to Death on its own. Bosses are much faster and harder than in Revengeance, basic enemies are far more aggressive, Blood Moons send endless waves, debuffs are more lethal and the Abyss is more dangerous.",
                       "ต้องเป็นโหมด Master ในโลก Master ถ้าเปิด Revengeance ระบบจะเปลี่ยนเป็น Death ให้เอง บอสเร็วและยากกว่า Revengeance มาก ศัตรูทั่วไปดุกว่ามาก Blood Moon มีศัตรูมาเป็นระลอกไม่หยุด ดีบัฟอันตรายขึ้น และ Abyss อันตรายยิ่งขึ้น")},
    "Malice": {"Title": ("Malice", "Malice"),
               "Body": ("Needs Legendary Mode (a Master world with the For the Worthy seed), where Calamity switches Revengeance to Malice on its own: Death Mode and For the Worthy behavior with Legendary stats.",
                        "ต้องเป็นโหมด Legendary (โลก Master ที่ใช้ seed For the Worthy) ซึ่งระบบจะเปลี่ยน Revengeance เป็น Malice ให้เอง พฤติกรรมแบบ Death และ For the Worthy พร้อมค่าสถานะแบบ Legendary")},
}

G["cal-rage-adrenaline"] = {
    "Title": ("Rage and Adrenaline", "Rage และ Adrenaline"),
    "Summary": ("Revengeance's two damage meters.", "เกจดาเมจสองแบบของ Revengeance"),
    "Adrenaline": {"Title": ("Adrenaline", "Adrenaline"),
                   "Body": ("Fills while you fight bosses; getting hit empties it. When full, press the Adrenaline key for 5 seconds of 2.5x damage. While it is full and unused, hits against you are also halved.",
                            "เกจจะเพิ่มขณะสู้บอส และหมดเมื่อโดนโจมตี เมื่อเต็มให้กดปุ่ม Adrenaline เพื่อรับดาเมจ 2.5 เท่าเป็นเวลา 5 วินาที ระหว่างที่เกจเต็มและยังไม่ได้ใช้ ดาเมจที่คุณได้รับจะลดลงครึ่งหนึ่งด้วย")},
    "Rage": {"Title": ("Rage", "Rage"),
             "Body": ("Fills when you are near enemies or use certain items. When full, press the Rage key for +35% damage for 9 seconds.",
                      "เกจจะเพิ่มเมื่ออยู่ใกล้ศัตรูหรือใช้ไอเทมบางชนิด เมื่อเต็มให้กดปุ่ม Rage เพื่อรับดาเมจ +35% เป็นเวลา 9 วินาที")},
    "Boosters": {"Title": ("Boosters", "บูสเตอร์"),
                 "Body": ("Rage lasts 1 second longer per booster: [item:CalamityMod/MushroomPlasmaRoot], [item:CalamityMod/InfernalBlood], [item:CalamityMod/RedLightningContainer]. Adrenaline gains +20% damage and +5% damage reduction per booster: [item:CalamityMod/ElectrolyteGelPack], [item:CalamityMod/StarlightFuelCell], [item:CalamityMod/Ectoheart]. All come from Treasure Bags in Revengeance.",
                          "Rage อยู่นานขึ้น 1 วินาทีต่อบูสเตอร์: [item:CalamityMod/MushroomPlasmaRoot], [item:CalamityMod/InfernalBlood], [item:CalamityMod/RedLightningContainer] ส่วน Adrenaline ได้ดาเมจ +20% และลดดาเมจที่ได้รับ +5% ต่อบูสเตอร์: [item:CalamityMod/ElectrolyteGelPack], [item:CalamityMod/StarlightFuelCell], [item:CalamityMod/Ectoheart] ทั้งหมดได้จาก Treasure Bag ในโหมด Revengeance")},
    "TrueMelee": {"Title": ("Keys and true melee", "ปุ่มและ True Melee"),
                  "Body": ("Bind the Rage and Adrenaline keys in Settings > Controls. True melee hits (the weapon itself touching the enemy) get only half of the Rage and Adrenaline damage bonus.",
                           "ตั้งปุ่ม Rage และ Adrenaline ได้ที่ Settings > Controls การโจมตีแบบ True Melee (ตัวอาวุธกระทบศัตรูโดยตรง) จะได้โบนัสดาเมจจาก Rage และ Adrenaline แค่ครึ่งเดียว")},
}

G["cal-rogue"] = {
    "Title": ("Rogue and stealth", "Rogue และ Stealth"),
    "Summary": ("Calamity's throwing class.", "คลาสอาวุธขว้างของ Calamity"),
    "Stealth": {"Title": ("Stealth", "Stealth"),
                "Body": ("Wearing rogue armor lets you build stealth. It builds while you are not attacking (more slowly while moving) and is only used up by attacking. The more stealth you have, the more rogue damage you deal.",
                         "การสวมเกราะสาย Rogue ทำให้สะสม Stealth ได้ Stealth จะเพิ่มขณะไม่ได้โจมตี (เพิ่มช้าลงขณะเคลื่อนที่) และจะลดลงเฉพาะตอนโจมตี ยิ่งมี Stealth มาก ดาเมจ Rogue ยิ่งสูง")},
    "Strikes": {"Title": ("Stealth Strikes", "Stealth Strike"),
                "Body": ("At full stealth your next rogue attack becomes a Stealth Strike: a special, much stronger version of the weapon's attack. More stealth regeneration slightly lowers the stealth damage bonus but lets you strike more often.",
                         "เมื่อ Stealth เต็ม การโจมตี Rogue ครั้งถัดไปจะเป็น Stealth Strike ซึ่งเป็นท่าพิเศษที่แรงกว่ามาก การฟื้น Stealth ที่เร็วขึ้นจะลดโบนัสดาเมจลงเล็กน้อย แต่ทำให้ใช้ Stealth Strike ได้บ่อยขึ้น")},
    "Gear": {"Title": ("Two play styles", "สองสไตล์การเล่น"),
             "Body": ("The Gear tab splits rogue weapons and accessories into Spam (attack as fast as possible) and Stealth strike builds.",
                      "แท็บชุดแนะนำแยกอาวุธและเครื่องประดับ Rogue เป็นสาย Spam (โจมตีให้เร็วที่สุด) และสาย Stealth Strike")},
}

G["cal-summon"] = {
    "Title": ("Summoner rules", "กติกาของ Summoner"),
    "Summary": ("Calamity's cross-class penalty for minions.", "บทลงโทษของมินเนียนเมื่อถืออาวุธต่างคลาสใน Calamity"),
    "Penalty": {"Title": ("The penalty", "บทลงโทษ"),
                "Body": ("While you hold a melee, ranged, magic or rogue weapon, your minions and sentries deal 25% less damage.",
                         "ขณะถืออาวุธ Melee, Ranged, Magic หรือ Rogue มินเนียนและ Sentry จะทำดาเมจลดลง 25%")},
    "Exceptions": {"Title": ("Exceptions", "ข้อยกเว้น"),
                   "Body": ("Whips and other summon weapons, tools, accessories and ammo do not trigger it. It is also lifted by Forbidden armor with a magic weapon, a Forbidden Circlet set with a rogue weapon, Fearmonger armor, GemTech armor's blue gem, the Profaned Soul Crystal, and during the Old One's Army.",
                            "แส้และอาวุธ Summon อื่น เครื่องมือ เครื่องประดับ และกระสุน ไม่ทำให้เกิดบทลงโทษ และบทลงโทษจะหายไปเมื่อใช้เกราะ Forbidden กับอาวุธ Magic, ชุด Forbidden Circlet กับอาวุธ Rogue, เกราะ Fearmonger, อัญมณีสีน้ำเงินของเกราะ GemTech, Profaned Soul Crystal และระหว่างอีเวนต์ Old One's Army")},
}

G["cal-defense-damage"] = {
    "Title": ("Defense damage", "ดาเมจต่อพลังป้องกัน"),
    "Summary": ("Why your defense drops after hits.", "ทำไมพลังป้องกันลดลงหลังโดนโจมตี"),
    "How": {"Title": ("How it works", "ทำงานอย่างไร"),
            "Body": ("Certain attacks also chip away at your defense for a short time. It regenerates quickly once you stop getting hit, so avoiding hits in a row matters more than stacking defense.",
                     "การโจมตีบางอย่างจะลดพลังป้องกันของคุณชั่วคราว และจะฟื้นกลับอย่างรวดเร็วเมื่อไม่โดนโจมตีต่อ การหลบไม่ให้โดนติด ๆ กันจึงสำคัญกว่าการเพิ่มพลังป้องกันอย่างเดียว")},
}

G["cal-biomes"] = {
    "Title": ("Calamity's biomes", "ไบโอมของ Calamity"),
    "Summary": ("The Sunken Sea, the Sulphurous Sea, the Abyss and more.", "Sunken Sea, Sulphurous Sea, Abyss และอื่น ๆ"),
    "SunkenSea": {"Title": ("Sunken Sea", "Sunken Sea"),
                  "Body": ("Deep below the Desert lies the Sunken Sea, the ruins of a fallen kingdom. The Giant Clam lives there after the Desert Scourge, and the Sea King (Amidias) moves in once both are beaten; he can give you a bubble for breathing underwater.",
                           "ลึกลงไปใต้ทะเลทรายคือ Sunken Sea ซากอาณาจักรที่ล่มสลาย Giant Clam จะอาศัยอยู่ที่นั่นหลังชนะ Desert Scourge และ Sea King (Amidias) จะย้ายเข้ามาเมื่อชนะทั้งสอง เขาให้ฟองอากาศช่วยหายใจใต้น้ำได้")},
    "Sulphurous": {"Title": ("Sulphurous Sea", "Sulphurous Sea"),
                   "Body": ("One ocean is toxic: its water poisons you over time. Acid Rain and the Aquatic Scourge belong here.",
                            "ทะเลฝั่งหนึ่งเป็นพิษ น้ำของมันจะทำให้คุณติดพิษเมื่ออยู่นาน Acid Rain และ Aquatic Scourge อยู่ที่นี่")},
    "Abyss": {"Title": ("The Abyss", "Abyss"),
              "Body": ("Deep below the Sulphurous Sea: dark, deep and dangerous. Bring proper diving gear and light. Its locked chests open after Skeletron. Never teleport there with a Rod of Discord.",
                       "ลึกลงไปใต้ Sulphurous Sea: มืด ลึก และอันตราย เตรียมอุปกรณ์ดำน้ำและแสงให้พร้อม หีบที่ถูกล็อกจะเปิดได้หลังชนะ Skeletron และห้ามวาร์ปด้วย Rod of Discord ที่นั่นเด็ดขาด")},
    "Astral": {"Title": ("Astral Infection", "Astral Infection"),
               "Body": ("An alien biome from a fallen star. Astrum Aureus and Astrum Deus are fought here, and Astral Ore can be mined after Astrum Deus.",
                        "ไบโอมต่างดาวจากดาวที่ตกลงมา สู้ Astrum Aureus และ Astrum Deus ที่นี่ และขุดแร่ Astral ได้หลังชนะ Astrum Deus")},
    "Crag": {"Title": ("Brimstone Crag", "Brimstone Crag"),
             "Body": ("A scorched region of the Underworld, home of the Brimstone Elemental.", "พื้นที่ไหม้เกรียมใน Underworld ถิ่นของ Brimstone Elemental")},
    "Labs": {"Title": ("Labs and planetoids", "ห้องแล็บและ Planetoid"),
             "Body": ("Draedon's abandoned labs are hidden near the Sunken Sea, on a large planetoid in the sky, in the Jungle, in the Underworld and deep in the ice caves. See the Draedon's labs guide.",
                      "ห้องแล็บร้างของ Draedon ซ่อนอยู่ใกล้ Sunken Sea, บน Planetoid ใหญ่บนฟ้า, ใน Jungle, ใน Underworld และลึกในถ้ำน้ำแข็ง ดูคู่มือห้องแล็บของ Draedon")},
}

G["cal-acid-rain"] = {
    "Title": ("Acid Rain", "Acid Rain"),
    "Summary": ("The Sulphurous Sea's event in three tiers.", "อีเวนต์ของ Sulphurous Sea ที่มีสามระดับ"),
    "Start": {"Title": ("Starting it", "การเริ่ม"),
              "Body": ("After the Eye of Cthulhu, the first Acid Rain starts when you enter the Sulphurous Sea. Later ones start by themselves at night or with a [item:CalamityMod/CausticTear]. A Caustic Tear used before the Eye of Cthulhu (or Hardmode) is wasted.",
                       "หลังชนะ Eye of Cthulhu แล้ว Acid Rain ครั้งแรกจะเริ่มเมื่อคุณเข้า Sulphurous Sea ส่วนครั้งต่อไปจะเริ่มเองตอนกลางคืน หรือใช้ [item:CalamityMod/CausticTear] ก็ได้ แต่ถ้าใช้ Caustic Tear ก่อนชนะ Eye of Cthulhu (หรือก่อน Hardmode) ไอเทมจะหายไปเปล่า ๆ")},
    "Tiers": {"Title": ("Tiers", "ระดับ"),
              "Body": ("Tier 1 after the Eye of Cthulhu, tier 2 after the Aquatic Scourge (with the Cragmaw Mire), tier 3 after Polterghast (with the Mauler and the Nuclear Terror). Completing tier 3 summons the Old Duke.",
                       "ระดับ 1 หลัง Eye of Cthulhu, ระดับ 2 หลัง Aquatic Scourge (มี Cragmaw Mire), ระดับ 3 หลัง Polterghast (มี Mauler และ Nuclear Terror) และเมื่อทำระดับ 3 ให้ครบจะเรียก Old Duke")},
    "Fail": {"Title": ("Failing", "การล้มเหลว"),
             "Body": ("The event fails if the rain stops, or if no Acid Rain enemy is killed for 2.5 minutes.",
                      "อีเวนต์จะล้มเหลวถ้าฝนหยุด หรือไม่ได้กำจัดศัตรูของ Acid Rain เลย 2.5 นาที")},
}

G["cal-draedon"] = {
    "Title": ("Draedon's labs and the Codebreaker", "ห้องแล็บของ Draedon และ Codebreaker"),
    "Summary": ("Schematics, power cells and the road to the Exo Mechs.", "แบบพิมพ์เขียว Power Cell และเส้นทางสู่ Exo Mechs"),
    "Labs": {"Title": ("The labs", "ห้องแล็บ"),
             "Body": ("Draedon's labs hold encrypted schematics, Power Cell Factories and Charging Stations that you can take. Power Cell Factories make Draedon Power Cells over time.",
                      "ห้องแล็บของ Draedon มีแบบพิมพ์เขียวที่เข้ารหัส Power Cell Factory และ Charging Station ที่เก็บไปใช้ได้ Power Cell Factory จะผลิต Draedon Power Cell ไปเรื่อย ๆ")},
    "Codebreaker": {"Title": ("The Codebreaker", "Codebreaker"),
                    "Body": ("Build the [item:CalamityMod/CodebreakerBase] and install parts: the Decryption Computer decrypts the planetoid schematic; the Jungle one also needs the Sensor Array; the Underworld one also the Advanced Display; the ice one also the Voltage Regulation System.",
                             "สร้าง [item:CalamityMod/CodebreakerBase] แล้วติดตั้งชิ้นส่วน: Decryption Computer ถอดรหัสแบบพิมพ์เขียวจาก Planetoid ได้ แบบของ Jungle ต้องมี Sensor Array เพิ่ม แบบของ Underworld ต้องมี Advanced Display เพิ่ม และแบบของถ้ำน้ำแข็งต้องมี Voltage Regulation System เพิ่ม")},
    "Power": {"Title": ("Power cells", "Power Cell"),
              "Body": ("Decrypting costs Draedon Power Cells: 500 for the planetoid schematic, 950 for the Jungle, 1750 for the Underworld and 5000 for the ice caves. Draedon's Arsenal weapons use charge, refilled at a Charging Station.",
                       "การถอดรหัสใช้ Draedon Power Cell: 500 สำหรับแบบของ Planetoid, 950 สำหรับ Jungle, 1750 สำหรับ Underworld และ 5000 สำหรับถ้ำน้ำแข็ง อาวุธ Draedon's Arsenal ใช้พลังงานชาร์จ ซึ่งเติมได้ที่ Charging Station")},
    "ExoMechs": {"Title": ("Calling Draedon", "การเรียก Draedon"),
                 "Body": ("With an [item:CalamityMod/AuricQuantumCoolingCell] installed (after Yharon), the Codebreaker can contact Draedon and start the Exo Mechs fight.",
                          "เมื่อติดตั้ง [item:CalamityMod/AuricQuantumCoolingCell] (หลังชนะ Yharon) Codebreaker จะติดต่อ Draedon และเริ่มการต่อสู้กับ Exo Mechs ได้")},
}

G["cal-npcs"] = {
    "Title": ("Calamity's town NPCs", "NPC ในเมืองของ Calamity"),
    "Summary": ("Who moves in, and when.", "ใครย้ายเข้ามา และเมื่อไร"),
    "SeaKing": {"Title": ("Sea King (Amidias)", "Sea King (Amidias)"),
                "Body": ("Moves in after the Desert Scourge and the Giant Clam are defeated. He can give you a bubble for breathing underwater and later sells the Old Duke's bait.",
                         "ย้ายเข้ามาหลังชนะ Desert Scourge และ Giant Clam เขาให้ฟองอากาศช่วยหายใจใต้น้ำได้ และภายหลังจะขายเหยื่อสำหรับเรียก Old Duke")},
    "Bandit": {"Title": ("Bandit", "Bandit"),
               "Body": ("Moves in after Skeletron while someone carries a Platinum Coin (inventory or portable storage). If you reforge a lot, visit him: it can pay off.",
                        "ย้ายเข้ามาหลังชนะ Skeletron ขณะที่มีผู้เล่นพก Platinum Coin (ในช่องเก็บของหรือที่เก็บของพกพา) ถ้าคุณรีฟอร์จบ่อย ลองไปหาเขา อาจคุ้มค่า")},
    "Archmage": {"Title": ("Archmage (Permafrost)", "Archmage (Permafrost)"),
                 "Body": ("Moves in after Cryogen is defeated.", "ย้ายเข้ามาหลังชนะ Cryogen")},
    "Witch": {"Title": ("Brimstone Witch (Calamitas)", "Brimstone Witch (Calamitas)"),
              "Body": ("Moves in after Supreme Calamitas is defeated. She can enchant your items with new powers.",
                       "ย้ายเข้ามาหลังชนะ Supreme Calamitas เธอเสริมพลังใหม่ให้ไอเทมของคุณได้")},
}

G["cal-boss-rush"] = {
    "Title": ("Boss Rush", "Boss Rush"),
    "Summary": ("Every boss in a row, greatly strengthened.", "สู้บอสทุกตัวต่อเนื่องแบบเสริมพลังอย่างมาก"),
    "Start": {"Title": ("Starting it", "การเริ่ม"),
              "Body": ("Hold the [item:CalamityMod/Terminus] to start Boss Rush at any time; use it again to stop. The Terminus is in the shrine chest at the very bottom of the Abyss, and the Primordial Wyrm drops one.",
                       "กดใช้ [item:CalamityMod/Terminus] ค้างไว้เพื่อเริ่ม Boss Rush ได้ทุกเวลา และใช้อีกครั้งเพื่อหยุด Terminus อยู่ในหีบของศาลเจ้าที่ก้นสุดของ Abyss และ Primordial Wyrm ก็ดรอปด้วย")},
    "Rules": {"Title": ("Rules", "กติกา"),
              "Body": ("Bosses come one after another and are far stronger than normal. Normal boss summons do not work during it, and the difficulty cannot be changed.",
                       "บอสจะมาทีละตัวต่อเนื่องและแข็งแกร่งกว่าปกติมาก ระหว่างนั้นไอเทมเรียกบอสปกติจะใช้ไม่ได้ และเปลี่ยนระดับความยากไม่ได้")},
}

# ------------------------------------------------------------------ add-ons

G["infernum"] = {
    "Title": ("Infernum Mode", "Infernum Mode"),
    "Summary": ("Harder, rewritten boss fights for Calamity.", "การต่อสู้กับบอสแบบเขียนใหม่ที่ยากขึ้นสำหรับ Calamity"),
    "What": {"Title": ("What it is", "คืออะไร"),
             "Body": ("Infernum rewrites the fights of most Calamity and vanilla bosses with new, much harder attacks, and adds bosses such as the Bereft Vassal and a boss version of the Dreadnautilus.",
                      "Infernum เขียนการต่อสู้ของบอสส่วนใหญ่ทั้งของ Calamity และเกมหลักใหม่ ด้วยท่าที่ยากขึ้นมาก และเพิ่มบอสใหม่ เช่น Bereft Vassal และ Dreadnautilus แบบบอสเต็มตัว")},
    "Enable": {"Title": ("Turning it on", "การเปิดใช้"),
               "Body": ("Open your inventory, click Calamity's difficulty icon (top right) and choose Infernum. It turns on Revengeance and sets the world to Expert. Its extra bosses appear in the Progression tab only while Infernum is on.",
                        "เปิดช่องเก็บของ คลิกไอคอนความยากของ Calamity (มุมขวาบน) แล้วเลือก Infernum ระบบจะเปิด Revengeance และตั้งโลกเป็น Expert ให้อัตโนมัติ บอสเพิ่มของ Infernum จะแสดงในแท็บความก้าวหน้าเฉพาะตอนที่เปิด Infernum อยู่")},
    "Limits": {"Title": ("Limits", "ข้อจำกัด"),
               "Body": ("Infernum turns itself off in Master Mode, For the Worthy and Zenith (Get fixed boi) worlds, and it warns you if Eternity Mode is also on.",
                        "Infernum จะปิดตัวเองในโลก Master, For the Worthy และ Zenith (Get fixed boi) และจะเตือนถ้าเปิด Eternity Mode ไว้ด้วย")},
}

G["wotg"] = {
    "Title": ("Wrath of the Gods", "Wrath of the Gods"),
    "Summary": ("Solyn's story and the gods at the end of it.", "เรื่องราวของ Solyn และเหล่าเทพในตอนจบ"),
    "Story": {"Title": ("Solyn's quests", "เควสต์ของ Solyn"),
              "Body": ("The mod follows Solyn, a traveler from the stars. Her quests appear in the Progression tab in order: meet her, set up camp, go stargazing, explore Permafrost Keep, learn about the Genesis, recover the Antiseed and fight Mars.",
                       "ม็อดนี้เล่าเรื่องของ Solyn นักเดินทางจากดวงดาว เควสต์ของเธอจะแสดงในแท็บความก้าวหน้าตามลำดับ: พบเธอ ตั้งแคมป์ ดูดาว สำรวจ Permafrost Keep ฟังเรื่อง Genesis นำ Antiseed กลับมา และสู้ Mars")},
    "Genesis": {"Title": ("The Genesis", "The Genesis"),
                "Body": ("Activating the finished Genesis summons the Avatar of Emptiness. After it, the Nameless Deity is fought with the Terminus.",
                         "การเปิดใช้ Genesis ที่สร้างเสร็จแล้วจะเรียก Avatar of Emptiness หลังจากนั้นจะสู้ Nameless Deity โดยใช้ Terminus")},
}

G["fables"] = {
    "Title": ("Calamity Fables", "Calamity Fables"),
    "Summary": ("Reworked early bosses and a desert duel.", "บอสช่วงต้นแบบปรับใหม่และศึกดวลในทะเลทราย"),
    "Replace": {"Title": ("Replacing Calamity's bosses", "การแทนที่บอสของ Calamity"),
                "Body": ("Fables reworks the Desert Scourge and Crabulon. While its \"replace Calamity\" option is on (the default), the guide shows the Fables versions instead of Calamity's.",
                         "Fables ปรับ Desert Scourge และ Crabulon ใหม่ ขณะที่ตัวเลือก \"replace Calamity\" เปิดอยู่ (ค่าเริ่มต้น) คู่มือจะแสดงเวอร์ชันของ Fables แทนของ Calamity")},
    "Bosses": {"Title": ("Its bosses", "บอสของม็อดนี้"),
               "Body": ("Sir Nautilus waits in a hidden chamber under the dunes. The Desert Scourge is lured out with stormlion grubs. Crabulon is provoked with the Hookstaff on underground mushroom grass. With Spirit Reforged, a grub offered at the Ancient Altar starts the Scourge vs. Scarab duel.",
                        "Sir Nautilus รออยู่ในห้องลับใต้เนินทราย ส่วน Desert Scourge ล่อออกมาได้ด้วยตัวอ่อน Stormlion และ Crabulon ถูกยั่วให้ออกมาด้วย Hookstaff บนหญ้าเห็ดใต้ดิน ถ้าเปิด Spirit Reforged ด้วย การถวายตัวอ่อนที่ Ancient Altar จะเริ่มศึกดวล Scourge vs. Scarab")},
}

G["hunt"] = {
    "Title": ("Hunt of the Old God", "Hunt of the Old God"),
    "Summary": ("Goozma, the Auric Souls and the Shadowspec Curse.", "Goozma, Auric Soul และคำสาป Shadowspec"),
    "Goozma": {"Title": ("Goozma", "Goozma"),
               "Body": ("Throw an [item:CalamityMod/OverloadedSludge] onto King Slime during a Slime Rain, or use a [item:CalamityHunt/PluripotentSpawnEgg] on an empowered Slime Ninja Statue. It can be summoned from Hardmode on, but it is balanced for the end of the game.",
                        "โยน [item:CalamityMod/OverloadedSludge] ใส่ King Slime ระหว่าง Slime Rain หรือใช้ [item:CalamityHunt/PluripotentSpawnEgg] กับ Slime Ninja Statue ที่ถูกเสริมพลัง เรียกได้ตั้งแต่ Hardmode แต่ปรับสมดุลไว้สำหรับช่วงท้ายเกม")},
    "Souls": {"Title": ("Auric Souls", "Auric Soul"),
              "Body": ("Goozma, Yharon and the Old Duke each drop an Auric Soul every time they are defeated while anyone in the world has not absorbed that soul. Touch it to absorb it: +10 max life, +20 max mana, +3 defense and +3% to damage, crit chance, melee speed and movement speed.",
                       "Goozma, Yharon และ Old Duke จะดรอป Auric Soul ทุกครั้งที่ถูกชนะ ตราบใดที่ยังมีผู้เล่นในโลกที่ยังไม่ได้ดูดซับ Soul นั้น เดินไปแตะเพื่อดูดซับ: พลังชีวิตสูงสุด +10 มานาสูงสุด +20 พลังป้องกัน +3 และ +3% ให้ดาเมจ อัตราคริติคอล ความเร็วโจมตีระยะประชิด และความเร็วเคลื่อนที่")},
    "Curse": {"Title": ("The Shadowspec Curse", "คำสาป Shadowspec"),
              "Body": ("Hunt's client option Shadowspec Curse is on by default: items crafted from Shadowspec Bars cannot be used or equipped until Goozma is defeated in the world, and they show a question mark in the inventory (and in this guide). Turn the option off in Hunt of the Old God's mod settings to lift it.",
                       "ตัวเลือก Shadowspec Curse ของ Hunt เปิดอยู่เป็นค่าเริ่มต้น ไอเทมที่คราฟต์จาก Shadowspec Bar จะใช้หรือสวมใส่ไม่ได้จนกว่าจะชนะ Goozma ในโลกนั้น และจะมีเครื่องหมายคำถามในช่องเก็บของ (รวมถึงในคู่มือนี้) ถ้าต้องการยกเลิก ให้ปิดตัวเลือกนี้ในการตั้งค่าม็อด Hunt of the Old God"),},
}

G["catalyst"] = {
    "Title": ("Catalyst", "Catalyst"),
    "Summary": ("Astrageldon, and why it must come before the Moon Lord.", "Astrageldon และทำไมต้องสู้ก่อน Moon Lord"),
    "Astrageldon": {"Title": ("Astrageldon", "Astrageldon"),
                    "Body": ("Use the [item:CatalystMod/AstralCommunicator] on the surface of the Astral Infection. Defeat Astrageldon before the Moon Lord: afterwards the Communicator stops working in that world unless Astrageldon was already killed.",
                             "ใช้ [item:CatalystMod/AstralCommunicator] บนผิวดินของ Astral Infection ต้องชนะ Astrageldon ก่อน Moon Lord เพราะหลังจากนั้น Communicator จะใช้ไม่ได้ในโลกนั้น หากยังไม่เคยชนะ Astrageldon")},
}

G["spirit"] = {
    "Title": ("Spirit Reforged", "Spirit Reforged"),
    "Summary": ("Scarabeus of the Ziggurat.", "Scarabeus แห่ง Ziggurat"),
    "Scarabeus": {"Title": ("Scarabeus", "Scarabeus"),
                  "Body": ("Use any gem at the Ancient Altar on the desert Ziggurat during the day. The altar can also be crafted.",
                           "ใช้อัญมณีชนิดใดก็ได้ที่ Ancient Altar บน Ziggurat ในทะเลทรายตอนกลางวัน แท่นบูชานี้คราฟต์ได้ด้วย")},
}

# ------------------------------------------------------------------ warnings

G["mistakes"] = {
    "Title": ("Common mistakes", "ข้อผิดพลาดที่พบบ่อย"),
    "Summary": ("Things that are easy to regret.", "สิ่งที่ทำแล้วอาจเสียใจภายหลัง"),
    "Hardmode": {"Title": ("Starting Hardmode", "การเริ่ม Hardmode"),
                 "Body": ("Defeating the Wall of Flesh cannot be undone: the Hallow and the evil biomes start spreading. Protect key areas first.",
                          "การชนะ Wall of Flesh ย้อนกลับไม่ได้ Hallow และไบโอมสายชั่วร้ายจะเริ่มแพร่กระจาย ป้องกันพื้นที่สำคัญก่อน")},
    "Dungeon": {"Title": ("The Dungeon too early", "เข้า Dungeon เร็วเกินไป"),
                "Body": ("Entering the Dungeon before Skeletron is defeated summons the Dungeon Guardian, which kills almost instantly.",
                         "การเข้า Dungeon ก่อนชนะ Skeletron จะเรียก Dungeon Guardian ซึ่งทำให้คุณตายแทบทันที")},
    "Enrage": {"Title": ("Leaving the arena", "ออกจากสนามสู้"),
               "Body": ("Plantera enrages outside the Jungle and Duke Fishron outside the Ocean. Fight them where they belong.",
                        "Plantera จะคลั่งเมื่อออกนอก Jungle และ Duke Fishron จะคลั่งเมื่อออกนอกทะเล สู้ในถิ่นของพวกมัน")},
    "Empress": {"Title": ("The Empress at dawn", "Empress ตอนรุ่งเช้า"),
                "Body": ("If the sun rises while you fight the Empress of Light, her attacks kill almost instantly. Start early in the night.",
                         "ถ้าพระอาทิตย์ขึ้นระหว่างสู้ Empress of Light การโจมตีของเธอแทบจะทำให้ตายในทีเดียว ควรเริ่มตั้งแต่หัวค่ำ")},
}

G["cal-mistakes"] = {
    "Title": ("Calamity traps", "กับดักใน Calamity"),
    "Summary": ("Hidden triggers that summon bosses or waste items.", "ตัวกระตุ้นที่ซ่อนอยู่ซึ่งเรียกบอสหรือทำให้เสียไอเทม"),
    "Abyss": {"Title": ("Teleporting in the Abyss", "การวาร์ปใน Abyss"),
              "Body": ("Never use a Rod of Discord (or anything that gives Chaos State) while in the Abyss: it immediately summons the Primordial Wyrm, at any point of the game.",
                       "ห้ามใช้ Rod of Discord (หรืออะไรก็ตามที่ทำให้ติด Chaos State) ขณะอยู่ใน Abyss เพราะจะเรียก Primordial Wyrm ออกมาทันที ไม่ว่าจะอยู่ช่วงไหนของเกม")},
    "Triggers": {"Title": ("Enemies that summon bosses", "ศัตรูที่เรียกบอส"),
                 "Body": ("Killing these summons a boss: a Hive Tumor (The Hive Mind), a Perforator Cyst (The Perforators), the elusive singer in the Ocean (Anahita and the Leviathan, even before Hardmode), 10 Sand Sharks after Plantera (Great Sand Shark) and 30 Phantom Spirits (Polterghast).",
                          "การทำลายสิ่งเหล่านี้จะเรียกบอส: Hive Tumor (The Hive Mind), Perforator Cyst (The Perforators), นักร้องลึกลับในทะเล (Anahita และ Leviathan แม้ก่อน Hardmode), Sand Shark 10 ตัวหลัง Plantera (Great Sand Shark) และ Phantom Spirit 30 ตัว (Polterghast)")},
    "Tear": {"Title": ("Caustic Tear too early", "ใช้ Caustic Tear เร็วเกินไป"),
             "Body": ("A Caustic Tear used before the Eye of Cthulhu (or Hardmode) is consumed but does not start Acid Rain.",
                      "Caustic Tear ที่ใช้ก่อนชนะ Eye of Cthulhu (หรือก่อน Hardmode) จะหายไปโดยไม่เริ่ม Acid Rain")},
    "SmallWorld": {"Title": ("Small worlds", "โลกขนาดเล็ก"),
                   "Body": ("Calamity does not support small worlds; use a medium or large world.",
                            "Calamity ไม่รองรับโลกขนาดเล็ก ควรใช้โลกขนาดกลางหรือใหญ่")},
}

GUIDE = {"Guides": G}
