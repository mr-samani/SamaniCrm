# ✅ انواع Relationship در EF Core

EF Core سه نوع اصلی ارتباط داره:

| نوع              | توضیح ساده                                 |
| ---------------- | ------------------------------------------ |
| **One-to-One**   | هر شیء A فقط با یک شیء B مرتبطه            |
| **One-to-Many**  | یک شیء A می‌تونه چندتا شیء B داشته باشه    |
| **Many-to-Many** | A و B هر دو می‌تونن چندتا از هم داشته باشن |

---

# ✅ قانون کلی: ارتباط همیشه باید در **مدل‌ها (Entities)** مشخص بشه

ارتباطات هم می‌تونن به صورت **Convention-based** تعریف بشن (خود EF بفهمه) یا با **Fluent API** (دستی در `OnModelCreating` یا `EntityTypeConfiguration` کلاس).

---

## 🔷 1. One-to-Many (رایج‌ترین)

### مثال: یک `Category` چندتا `Product` داره.

### ✅ مدل‌ها:

```csharp
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Guid CategoryId { get; set; }               // FK
    public Category Category { get; set; } = default!; // Navigation
}
```

### ✅ Fluent API:

```csharp
builder.HasOne(p => p.Category)
       .WithMany(c => c.Products)
       .HasForeignKey(p => p.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);
```

---

## 🔷 2. One-to-One

### مثال: یک `User` فقط یک `UserProfile` داره.

### ✅ مدل‌ها:

```csharp
public class User
{
    public Guid Id { get; set; }
    public UserProfile Profile { get; set; } = default!;
}

public class UserProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
```

### ✅ Fluent API:

```csharp
builder.HasOne(u => u.Profile)
       .WithOne(p => p.User)
       .HasForeignKey<UserProfile>(p => p.UserId);
```

---

## 🔷 3. Many-to-Many

### مثال: `User` می‌تونه چندتا `Role` داشته باشه، و `Role` می‌تونه چندتا `User` داشته باشه.

از EF Core 5 به بعد، می‌شه بدون کلاس join هم انجام داد.

### ✅ مدل‌ها:

```csharp
public class User
{
    public Guid Id { get; set; }
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}

public class Role
{
    public Guid Id { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}
```

### ✅ Fluent API (حتی لازم نیست بنویسی، EF خودش می‌فهمه)

اما اگه بخوای جدول واسط (join table) خودت بسازی:

```csharp
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}
```

---

# ✅ قانون ساده برای یادگیری سریع

| ارتباط       | توی کدوم Entity‌ها تعریف بشه؟                                                             |
| ------------ | ----------------------------------------------------------------------------------------- |
| One-to-Many  | `Many` کلاس: `FK + Navigation to One`<br>`One` کلاس: `Collection Navigation to Many`      |
| One-to-One   | هر دو طرف `Navigation` + فقط یکی `FK` داره                                                |
| Many-to-Many | هر دو طرف `Collection` دارن. اگه کلاس واسط داری، باید خودش شامل دو `FK + Navigation` باشه |

---

# ✅ مثال کامل برای فهم بهتر (Product ↔ Category)

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Guid CategoryId { get; set; }      // FK
    public Category Category { get; set; } = default!;
}

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

### Fluent:

```csharp
builder.HasOne(p => p.Category)
       .WithMany(c => c.Products)
       .HasForeignKey(p => p.CategoryId);
```

---

# 💡 اگر شک داشتی، این سه نکته رو یادت باشه:

1. اگر `یک چیز به چند چیز` هست → توی "چندتا" کلاس `FK` بذار
2. `Navigation Property` همیشه باید دو طرفه باشه، مگر در شرایط خاص
3. `Fluent API` کمک می‌کنه همه‌چی شفاف، تست‌پذیر و قابل کنترل باشه

