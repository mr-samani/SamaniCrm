ساخت یک **سیستم مدیریت محصولات بسیار حرفه‌ای، مقیاس‌پذیر و داینامیک** برای یک **پلتفرم اینترپرایز** که سازمان‌ها و افراد مختلف بتوانند انواع محصولات متنوع خود را تعریف و بفروشند، نیازمند طراحی جدولی منعطف، آینده‌نگر و دقیق است.

من در ادامه **ساختار پیشنهادی جداول دیتابیس** را در قالب **پایگاه داده رابطه‌ای (مانند SQL Server)** و با در نظر گرفتن اصول **DDD، Clean Architecture و Multi-Tenancy** شرح می‌دهم:

---

## ✅ اصول کلی طراحی

1. **Multi-Tenancy**: هر Tenant (شرکت، شخص، سازمان) باید بتواند دسته‌بندی و ویژگی‌های مخصوص خود را داشته باشد.
2. **Dynamic Attributes**: پشتیبانی از ویژگی‌های پویا برای محصولات.
3. **Category Trees**: پشتیبانی از دسته‌بندی‌های تو در تو (hierarchical).
4. **Localization**: پشتیبانی از چند زبان در نام‌ها و توضیحات.
5. **Separation of Concerns**: جداسازی موجودی، قیمت‌گذاری، مشخصات فنی، فایل‌ها، تصویرها و ... از موجودیت اصلی محصول.

---

## 📦 ساختار پیشنهادی جداول

### 1. Tenants

```sql
Tenants
- Id (GUID)
- Name
- Slug
- CreatedAt
- IsActive
```

---

### 2. ProductCategories

```sql
ProductCategories
- Id (GUID)
- TenantId (FK to Tenants)
- ParentId (nullable, FK to ProductCategories)
- Name
- Slug
- Description
- SortOrder
- IsActive
- CreatedAt
```

> دسته‌بندی‌های تو در تو با ParentId

---

### 3. Products

```sql
Products
- Id (GUID)
- TenantId (FK to Tenants)
- SKU (string, unique per tenant)
- Title
- Slug
- Description
- CategoryId (FK to ProductCategories)
- ProductTypeId (FK to ProductTypes)
- IsActive
- CreatedAt
```

---

### 4. ProductTypes

```sql
ProductTypes
- Id (GUID)
- TenantId
- Name
- Description
```

> هر ProductType یک مجموعه‌ای از ویژگی‌ها (attributes) را دارد. مثل "موبایل"، "کتاب"، "لباس"

---

### 5. ProductAttributes (ویژگی‌های تعریف‌شده برای هر نوع محصول)

```sql
ProductAttributes
- Id (GUID)
- ProductTypeId (FK to ProductTypes)
- Name
- DataType (string) -- مثل string, int, decimal, bool, datetime, enum
- IsRequired
- IsVariant (bool) -- آیا این ویژگی باعث تنوع محصول می‌شود؟
- SortOrder
```

---

### 6. ProductAttributeValues (مقداردهی ویژگی‌ها به هر محصول)

```sql
ProductAttributeValues
- Id (GUID)
- ProductId (FK to Products)
- AttributeId (FK to ProductAttributes)
- Value (string)
```

> مقدار به صورت string ذخیره می‌شود، اما با توجه به DataType تفسیر می‌گردد.

---

### 7. ProductVariants

```sql
ProductVariants
- Id (GUID)
- ProductId (FK to Products)
- SKU
- Price
- Stock
- IsActive
- CreatedAt
```

> اگر محصول دارای ویژگی‌های تنوع‌پذیر باشد (مثل رنگ، سایز)، هر ترکیب یک Variant می‌شود.

---

### 8. ProductVariantAttributeValues

```sql
ProductVariantAttributeValues
- Id (GUID)
- ProductVariantId (FK to ProductVariants)
- AttributeId (FK to ProductAttributes)
- Value (string)
```

---

### 9. ProductPrices (برای تاریخچه قیمت‌ها و قیمت‌های منطقه‌ای)

```sql
ProductPrices
- Id
- ProductVariantId
- Currency
- Price
- StartDate
- EndDate (nullable)
```

---

### 10. ProductInventory

```sql
ProductInventory
- Id
- ProductVariantId
- Quantity
- WarehouseId (nullable)
- LastUpdated
```

---

### 11. ProductImages

```sql
ProductImages
- Id
- ProductId
- Url
- IsMain
- SortOrder
```

---

### 12. ProductFiles (برای محصولاتی مثل نرم‌افزار یا کتاب الکترونیکی)

```sql
ProductFiles
- Id
- ProductId
- FileUrl
- FileType
```

---

## 🌍 پشتیبانی از چند زبان (اختیاری)

```sql
ProductTranslations
- Id
- ProductId
- LanguageCode
- Title
- Description
```

---

## 📊 مزایا و انعطاف‌پذیری این مدل

| ویژگی                   | پشتیبانی                                  |
| ----------------------- | ----------------------------------------- |
| تنوع محصولات            | ✔️ با استفاده از ProductType + Attributes |
| دسته‌بندی داینامیک      | ✔️ با Category Tree                       |
| ویژگی‌های قابل تغییر    | ✔️ Dynamic Attributes                     |
| چند فروشنده / شرکت      | ✔️ با TenantId                            |
| تنوع (Variant) در محصول | ✔️ با ProductVariant                      |
| تاریخچه قیمت            | ✔️ با ProductPrices                       |
| انبار و موجودی          | ✔️ با ProductInventory                    |
| قابلیت توسعه            | ✔️ کاملا ماژولار و منعطف                  |

---

## ❗ نکات مهم در پیاده‌سازی

* از **Entity Framework Core** استفاده کن با **Fluent API** برای تعریف دقیق روابط.
* از **CQRS** و **MediatR** برای مدیریت Command/Query ها استفاده کن.
* حتما **soft delete** و **IsActive** را لحاظ کن.
* در سطح **Domain Layer** از **Value Object** برای Attribute و AttributeValue استفاده کن تا تایپ و ولیدیشن در لایه دامین حفظ شود.
* از **JSON Schema** برای ذخیره ساختار ویژگی‌های محصول هم می‌توان استفاده کرد (در معماری NoSQL).

---

اگر خواستی، می‌تونم کل **کلاس‌های Entity و Mapping** این ساختار را هم برات بنویسم در قالب یک پروژه `.NET 9` با Clean Architecture و Multi-Tenancy.

آیا نیاز به این مرحله هم داری؟
