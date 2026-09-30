# Client QA fixtures

ตัวละคร โลก และ config ส่วนตัวไม่ได้รวมไว้ใน repo
สร้างตัวละครกับโลกสำหรับทดสอบ แล้วนำ **สำเนา** มาไว้ในโฟลเดอร์ fixture ในเครื่อง:

```text
config.json
Players/AYO.plr
Players/AYO.tplr
Worlds/FF.wld
Worlds/FF.twld
```

ใช้ชื่อไฟล์ตามนี้ เพราะ QA script เปิดตัวละคร AYO และโลก FF
`config.json` ที่แนบมาเป็นค่าพื้นฐานสำหรับทดสอบ ไม่ใช่ config ที่คัดลอกจากผู้เล่น

```powershell
./tools/RunClientQa.ps1 -TML $tml -Fixtures tools/qa-fixtures -Out .qa/client
```

ไฟล์ตัวละครและโลกถูกละเว้นใน `.gitignore` เก็บไว้ใช้ตรวจในเครื่องเท่านั้น
