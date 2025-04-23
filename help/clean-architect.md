## 🧱 لایه‌ها در معماری تمیز (Clean Architecture)


``` 
[Domain]         <== هسته‌ی خالص، بدون وابستگی به هیچ‌چیز
[Application]    <== قوانین کسب‌و‌کار + واسط‌ها
[Infrastructure] <== پیاده‌سازی جزئیات (پایگاه داده، فایل، http و ...)
[Host]           <== نقطه‌ی ورودی برنامه، مثل API یا UI
```

---

### ✅ ارتباط درست بین لایه‌ها:

| لایه | می‌تونه به | نمی‌تونه به |
|------|------------|--------------|
| **Domain** | — | ❌ Application, Infrastructure, Host |
| **Application** | Domain | ❌ Infrastructure, Host |
| **Infrastructure** | Application, Domain | ❌ Host |
| **Host (API)** | Application, Infrastructure | ❌ نباید به Domain یا Application وابستگی مستقیم بزنه (بدون واسطه) |

---

### 💡 پس به طور ساده:
- `Domain`: مغز سیستم ـ قوانین مدل‌محور ـ کاملاً مستقل
- `Application`: هماهنگ‌کننده قوانین کسب‌و‌کار. فقط با interface کار می‌کنه
- `Infrastructure`: پیاده‌سازی interfaceهایی که توی Application تعریف شدن
- `Host`: API هست که پروژه رو اجرا می‌کنه، با Application کار می‌کنه، از DI همه چیزو می‌گیره

---

---

## 🎯 مزیت این معماری:
- وابستگی فقط به داخل هست (وابستگی‌ها معکوس‌ان)
- راحت تست‌نویسی می‌کنی (چون همه‌چی با اینترفیسه)
- هر لایه قابل تعویض بدون تغییر لایه‌های دیگه‌ست
- پروژه‌ت منظم، قابل فهم و مقیاس‌پذیره

---
حتماً، اینم ساختار معماری تمیز (Clean Architecture) با نمودار گرافیکی به سبک Markdown using `graphviz` / mermaid syntax:

---

### 🧱 Clean Architecture - Markdown Mermaid Graph

```mermaid
graph TD
    A[Host API Layer] --> B[Infrastructure Layer]
    A --> C[Application Layer]
    B --> C
    C --> D[Domain Layer]

    subgraph Responsibilities
        A1[API Controllers, Startup, DI setup]
        B1[EF Core, Identity, File Storage, Email]
        C1[UseCases, Services, Interfaces]
        D1[Entities, ValueObjects, Domain Events]
    end

    A --> A1
    B --> B1
    C --> C1
    D --> D1

    %% Disallowed dependency paths
    D -.X.-> C
    C -.X.-> B
    B -.X.-> A

    style A fill:red,stroke:#f3a13c,stroke-width:2px
    style B fill:blue,stroke:#dcb99a,stroke-width:2px
    style C fill:green,stroke:#5cb85c,stroke-width:2px
    style D fill:gray,stroke:#3c78d8,stroke-width:2px
```

---

### 📌 تفسیر سریع:

- ✅ مسیرهای مجاز: فقط از بیرون به درون (⬇️)
- ❌ مسیرهای ممنوع: از درون به بیرون (🔒)
- هر لایه فقط لایه پایین‌ترش رو می‌بینه
- `Application` به `Infrastructure` وابسته نیست، بلکه `Interface` می‌ده و `Infrastructure` اون رو پیاده می‌کنه.

---
