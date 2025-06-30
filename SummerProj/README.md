# Summer Project API

Це REST API для управління компаніями, вакансіями, працівниками та іншими сутностями.

## Технології

- .NET 8.0
- Entity Framework Core
- AutoMapper 12.0.1
- FluentValidation 11.3.0
- Swagger/OpenAPI
- Bogus (для генерації тестових даних)
- PostgreSQL

## Структура проекту

- **BLL** - Business Logic Layer (бізнес-логіка, сервіси, DTO, валідація)
- **DAL.EF** - Data Access Layer (Entity Framework, репозиторії, Unit of Work)
- **SummerProj** - Web API проект (контролери, конфігурація)

## Вирішення проблем з залежностями

### Проблема з AutoMapper
Якщо виникає помилка конфлікту версій AutoMapper, переконайтеся що:

1. В `SummerProj.Api.csproj`:
```xml
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

2. В `BLL.csproj`:
```xml
<PackageReference Include="AutoMapper" Version="12.0.1" />
```

3. В `Program.cs` використовуйте:
```csharp
builder.Services.AddAutoMapper(typeof(EntityMappingProfile).Assembly);
```

### Проблема з FluentValidation
Переконайтеся що версії FluentValidation співпадають:

1. В `SummerProj.Api.csproj`:
```xml
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
```

2. В `BLL.csproj`:
```xml
<PackageReference Include="FluentValidation" Version="11.3.0" />
```

## Налаштування бази даних

### PostgreSQL
1. Встановіть PostgreSQL на ваш комп'ютер
2. Створіть базу даних `SummerBase`
3. Налаштуйте користувача `postgres` з паролем `zxc123`
4. Або змініть рядок підключення в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SummerBase;Username=your_username;Password=your_password"
  }
}
```

### SQL Server (альтернатива)
Якщо хочете використовувати SQL Server, змініть в `Program.cs`:

```csharp
builder.Services.AddDbContext<SummerDbContext>(options =>
    options.UseSqlServer(connectionString));
```

І в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SummerBase;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Ініціалізація бази даних

### Автоматична ініціалізація
При першому запуску додатку база даних автоматично ініціалізується тестовими даними, якщо вона порожня.

### Ручна ініціалізація через API

#### POST /api/DataInitialization/seed
Ініціалізувати базу даних тестовими даними

#### DELETE /api/DataInitialization/clear
Очистити всі дані з бази даних

#### GET /api/DataInitialization/stats
Отримати статистику кількості записів у базі даних

### Генеровані тестові дані
- **10 типів активності** (IT, фінанси, маркетинг, тощо)
- **15 компаній** з повною інформацією
- **20 працівників** з резюме та освітою
- **25 вакансій** від різних компаній
- **30 відгуків** від працівників на вакансії
- **20 угод** між працівниками та компаніями

## API Endpoints

### DataInitialization (Ініціалізація бази даних)

#### POST /api/DataInitialization/seed
Ініціалізувати базу даних тестовими даними

#### DELETE /api/DataInitialization/clear
Очистити всі дані з бази даних

#### GET /api/DataInitialization/stats
Отримати статистику бази даних

### ActivityType (Типи активності)

#### GET /api/ActivityType
Отримати всі типи активності

#### GET /api/ActivityType/{id}
Отримати тип активності за ID

#### POST /api/ActivityType
Створити новий тип активності

#### PUT /api/ActivityType/{id}
Оновити тип активності

#### DELETE /api/ActivityType/{id}
Видалити тип активності

### Companie (Компанії)

#### GET /api/Companie
Отримати всі компанії

#### GET /api/Companie/{id}
Отримати компанію за ID

#### POST /api/Companie
Створити нову компанію

#### PUT /api/Companie/{id}
Оновити компанію

#### DELETE /api/Companie/{id}
Видалити компанію

### Worker (Працівники)

#### GET /api/Worker
Отримати всіх працівників

#### GET /api/Worker/{id}
Отримати працівника за ID

#### POST /api/Worker
Створити нового працівника

#### PUT /api/Worker/{id}
Оновити працівника

#### DELETE /api/Worker/{id}
Видалити працівника

### Vacancy (Вакансії)

#### GET /api/Vacancy
Отримати всі вакансії

#### GET /api/Vacancy/{id}
Отримати вакансію за ID

#### POST /api/Vacancy
Створити нову вакансію

#### PUT /api/Vacancy/{id}
Оновити вакансію

#### DELETE /api/Vacancy/{id}
Видалити вакансію

### Response (Відгуки)

#### GET /api/Response
Отримати всі відгуки

#### GET /api/Response/{id}
Отримати відгук за ID

#### POST /api/Response
Створити новий відгук

#### PUT /api/Response/{id}
Оновити відгук

#### DELETE /api/Response/{id}
Видалити відгук

### Agreement (Угоди)

#### GET /api/Agreement
Отримати всі угоди

#### GET /api/Agreement/{id}
Отримати угоду за ID

#### POST /api/Agreement
Створити нову угоду

#### PUT /api/Agreement/{id}
Оновити угоду

#### DELETE /api/Agreement/{id}
Видалити угоду

## Коди відповідей

- **200 OK** - Успішний запит
- **201 Created** - Ресурс створено
- **204 No Content** - Ресурс видалено
- **400 Bad Request** - Помилка валідації
- **404 Not Found** - Ресурс не знайдено
- **500 Internal Server Error** - Внутрішня помилка сервера

## Запуск проекту

### Крок 1: Підготовка
1. Переконайтеся, що у вас встановлено .NET 8.0
2. Встановіть PostgreSQL або SQL Server
3. Налаштуйте базу даних та рядок підключення

### Крок 2: Відновлення пакетів
```bash
dotnet restore
```

### Крок 3: Запуск проекту
```bash
cd SummerProj
dotnet run
```

### Крок 4: Перевірка
1. База даних автоматично ініціалізується тестовими даними при першому запуску
2. Відкрийте Swagger UI: `https://localhost:7001/swagger`
3. Або використайте файл `TestApi.http` для тестування

## Тестування API

Використовуйте файли `.http` для тестування API в Visual Studio Code або інших IDE:

- `TestApi.http` - простий тест для перевірки роботи API
- `DataInitialization.http` - ініціалізація та управління базою даних
- `AllApis.http` - комплексні приклади запитів для всіх API
- `CompanieApi.http` - приклади запитів для компаній
- `ActivityTypeApi.http` - приклади запитів для типів активності
- `WorkerApi.http` - приклади запитів для працівників
- `VacancyApi.http` - приклади запитів для вакансій

## Послідовність тестування

1. **Перевірте статистику бази даних:**
   ```
   GET /api/DataInitialization/stats
   ```

2. **Якщо база порожня, ініціалізуйте її:**
   ```
   POST /api/DataInitialization/seed
   ```

3. **Тестуйте CRUD операції для всіх сутностей**

4. **При необхідності очистіть базу:**
   ```
   DELETE /api/DataInitialization/clear
   ```

## Валідація

Всі запити проходять валідацію через FluentValidation. Приклади правил валідації:

- Назва компанії не може бути порожньою
- Email повинен мати правильний формат
- Телефон повинен мати правильний формат
- Зовнішні ключі повинні існувати в базі даних
- Дата народження не може бути в майбутньому
- Зарплата повинна бути позитивною

## Обробка помилок

API повертає структуровані повідомлення про помилки:

```json
{
  "message": "Помилка валідації",
  "error": "CompanyName не може бути порожнім"
}
```

## Залежності між сутностями

1. **ActivityType** - незалежна сутність
2. **Companie** - залежить від ActivityType
3. **Worker** - залежить від ActivityType
4. **Vacancy** - залежить від Companie
5. **Response** - залежить від Worker та Vacancy
6. **Agreement** - залежить від Worker та Companie

## Послідовність створення даних

Для правильного тестування API рекомендується наступна послідовність:

1. Створіть ActivityType
2. Створіть Companie (з посиланням на ActivityType)
3. Створіть Worker (з посиланням на ActivityType)
4. Створіть Vacancy (з посиланням на Companie)
5. Створіть Response (з посиланнями на Worker та Vacancy)
6. Створіть Agreement (з посиланнями на Worker та Companie)

## Вирішення поширених проблем

### Помилка підключення до бази даних
- Перевірте, чи запущений PostgreSQL/SQL Server
- Перевірте правильність рядка підключення
- Перевірте права доступу користувача

### Помилка міграцій
```bash
dotnet ef database update --project DAL.EF --startup-project SummerProj
```

### Помилка генерації даних
- Перевірте, чи всі залежності правильно налаштовані
- Перевірте, чи існують всі необхідні enum'и (ResponseStatus)
- Перевірте, чи правильно налаштовані зовнішні ключі 