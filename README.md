# Khu Mue [Calamity]

ไกด์ในเกมสำหรับ Terraria / tModLoader กด **B** เพื่อเปิด
มีเส้นทางบอส วิธีเรียกบอส อุปกรณ์ที่แนะนำตามคลาส แหล่งหาไอเทม และรายการอัปเกรดถาวร
รองรับข้อความภาษาอังกฤษและภาษาไทย

สูตร ดรอป ร้านค้า Shimmer และค่าสถานะอ่านจากเกมที่กำลังเล่น
ส่วนคำแนะนำกับเส้นทางการเล่นอยู่ใน `Data/`
เวอร์ชันม็อดที่ใช้ตรวจข้อมูลระบุไว้ใน `Core/SupportedMods.cs`
และ [บันทึกการตรวจข้อมูล](docs/DATA-VERIFICATION.md)

ชื่อภายในคือ `KhuMue` เปลี่ยนมาจาก `BetterGuide` ตั้งแต่รุ่น 2.1

## Build

ใช้ .NET SDK 8 และ tModLoader สาย 2026.7 สำหรับ Terraria 1.4.4.9
ตั้ง `$tml` ให้ตรงกับที่ติดตั้งในเครื่อง:

```powershell
$tml = 'X:/SteamLibrary/steamapps/common/tModLoader'
dotnet build KhuMue.csproj -c Release -p:TMLInstallPath="$tml" -p:BuildMod=false
```

สร้างไฟล์ม็อดลงโฟลเดอร์ที่เลือก:

```powershell
./tools/Package.ps1 -TML $tml -Out .qa
```

หรือวางโฟลเดอร์ repo ชื่อ `KhuMue` ใน ModSources แล้วใช้ Build + Reload ในเกม

## แก้ข้อมูลและข้อความ

- `Core/` อ่านข้อมูลเกมและสถานะโลก
- `UI/` หน้าต่างและหน้าของไกด์
- `Data/` เส้นทางการเล่น อุปกรณ์ อัปเกรด และบทความ
- `Localization/` ข้อความที่ใช้ในม็อด
- `tools/data/loc/` ต้นฉบับข้อความภาษาอังกฤษและภาษาไทย

หลังแก้ข้อความ รัน `python tools/data/localization.py`
เส้นทางบอสสร้างจาก `python tools/data/progression.py`
เครื่องมือใน `tools/` ไม่ได้รวมไปกับม็อดที่แจก

ตรวจข้อมูลกับม็อดที่ติดตั้งอยู่:

```powershell
./tools/RunDataDump.ps1 -TML $tml -Profile full -Out tools/data/dump
python tools/data/validate.py tools/data/dump
```

ต้องมีม็อดชุดที่เลือกติดตั้งจาก Workshop เครื่องมือใช้โปรไฟล์ชั่วคราวแยกจากเซฟผู้เล่น
ข้อมูล dump ไม่ได้แนบไว้ใน repo ให้สร้างจากม็อดเวอร์ชันที่จะตรวจ
`tools/data/obtain_sources.py` ใช้อัปเดตข้อมูลหีบ การตกปลา และรางวัล Angler
โดยอ่านซอร์สเกมและ Calamity ในเครื่อง ไม่มีการนำซอร์สเหล่านั้นมาแจกใน repo นี้

## ตรวจหน้าไกด์ในเกม

ใช้ `tools/RunClientQa.ps1` พร้อม `-TML`, `-Fixtures` และ `-Out`
ตัวละครกับโลกสำหรับทดสอบไม่ได้แนบมา ให้เตรียมในเครื่องตาม
[วิธีเตรียม fixture](tools/qa-fixtures/README.md)
เครื่องมือรองรับทั้ง client ปกติและการต่อ dedicated server ด้วย `-Multiplayer`

## API

`Mod.Call("GetApiVersion")` คืนค่า 1, `Mod.Call("OpenGuide")` เปิดไกด์
และ `Mod.Call("OpenItem", itemType)` เปิดหน้าไอเทม

## สิทธิ์

ไอคอนและกรอบของม็อดใช้ [CC0 1.0](LICENSE-ART)
นำไปใช้ แก้ไข หรือแจกต่อได้ รวมถึงใช้เชิงพาณิชย์ ไม่ต้องขออนุญาตหรือให้เครดิต
กรอบแบบ PNG พื้นหลังโปร่งใสแยกไว้ใน [art/icon-frame](art/icon-frame/)

โค้ดใช้ [MIT](LICENSE)
ข้อมูลอุปกรณ์และข้อความที่ดัดแปลงจาก Calamity Mod Wiki กับ Terraria Wiki
รวมถึง snapshot ที่ใช้สร้างข้อมูล ยังใช้ CC BY-NC-SA 3.0 และ 4.0 ตามต้นทาง
ดูขอบเขตไฟล์และเครดิตใน [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)
จึงไม่ควรใช้สิทธิ์ MIT ของโค้ดไปอ้างครอบข้อมูลจากวิกิ
