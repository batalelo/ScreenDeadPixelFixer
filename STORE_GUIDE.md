# دليل رفع Black Circle Fixer على Microsoft Store (Partner Center)

تم إعداد المشروع بالكامل ليكون حزمة **MSIX (Desktop Bridge)** فائقة الخفة وبدون أي تبعيات خارجية، مع الحفاظ على الحجم الصغير جداً (~60 إلى 80 كيلوبايت فقط!).

---

## 📋 الخطوة 1: الحصول على بيانات الهوية من Partner Center

عند حجز اسم التطبيق داخل حسابك في **[Microsoft Partner Center](https://partner.microsoft.com/dashboard)**:

1. افتح تطبيقك في Partner Center.
2. اذهب إلى: **Product management** > **Product Identity**.
3. ستجد 3 بيانات أساسية:
   * **Package/Identity/Name** (مثال: `12345TakeYourSite.BlackCircleFixer`)
   * **Package/Identity/Publisher** (مثال: `CN=A1B2C3D4-E5F6-7890-ABCD-1234567890AB`)
   * **Package/Properties/PublisherDisplayName** (اسم الناشر الخاص بك)

---

## ✏️ الخطوة 2: تحديث ملف `Package/AppxManifest.xml`

افتح الملف `Package/AppxManifest.xml` وضع القيم الثلاث في السطور التالية:

```xml
  <Identity 
    Name="ضع_Package_Identity_Name_هنا" 
    Publisher="ضع_Package_Identity_Publisher_هنا" 
    Version="1.0.0.0" 
    ProcessorArchitecture="neutral" />

  <Properties>
    <DisplayName>Black Circle Fixer</DisplayName>
    <PublisherDisplayName>ضع_PublisherDisplayName_هنا</PublisherDisplayName>
    ...
```

---

## 📦 الخطوة 3: إنشاء حزمة `.msix` الجاهزة للرفع

لديك طريقتان للحصول على ملف `BlackCircleFixer.msix`:

### الطريقة الأولى (الأسهل والأسرع عبر GitHub Actions سحابياً):
1. بمجرد عمل `git push` إلى GitHub، سيعمل Workflow تلقائياً على خوادم مايكروسوفت/GitHub:
   - يجمع الكود.
   - ينشئ جميع مقاسات الأيقونات المطلوبة للمتجر بدقة عالية.
   - يستدعي أداة `MakeAppx.exe` الرسمية.
2. ادخل إلى تبويب **Actions** في مستودع GitHub، وافتح أحدث تشغيل.
3. حمّل الـ Artifact المسمى **`BlackCircleFixer-MSIX`** وستجد بداخله ملف `BlackCircleFixer.msix` جاهزاً ومضغوطاً بحجم لا يتجاوز **~80 KB**!

### الطريقة الثانية (محلياً على جهازك):
إذا كان لديك Windows SDK مثبتاً على جهازك، فقط افتح PowerShell وشغّل:
```powershell
.\package_msix.ps1
```
وسيقوم بإنشاء `BlackCircleFixer.msix` في المجلد الرئيسي فوراً.

---

## 🚀 الخطوة 4: التقديم داخل Partner Center

1. داخل Partner Center، أنشئ تقديم جديد (**Start your submission**).
2. في قسم **Packages**: اسحب وأفلت ملف `BlackCircleFixer.msix`.
3. في قسم **Store listings**:
   * **Title**: `Black Circle Fixer`
   * **Description**: يمكنك استخدام نفس النص الترويجي المكتوب في `README.md`.
   * **Keywords**:
     `black circle on laptop screen`, `black screen circle`, `black spot on screen`, `screen dead zone`, `damaged screen workaround`.
4. في قسم **Submission options / Notes for certification**:
   * سيسألك مراجع مايكروسوفت عن سبب استخدام صلاحية `runFullTrust`:
   * **اكتب لهم هذا التبرير الجاهز:**
     > *"Black Circle Fixer is a desktop utility that provides a visual workaround for users with damaged laptop displays (black circles/bruises). It requires 'runFullTrust' to capture desktop screen pixels behind the user's defined dead zone and display a click-through transparent magnifying overlay with live cursor tracking."*
5. اضغط **Submit to the Store**!

ستقوم مايكروسوفت بتوقيع الحزمة تلقائياً ونشرها للمستخدمين مع ميزة التحديثات التلقائية والتثبيت بضغطة زر واحدة.
