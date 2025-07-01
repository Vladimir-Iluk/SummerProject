# Документація по пагінації та пошуку

## Огляд

Додано можливість пагінації та пошуку по словам до всіх основних контролерів API. Кожен контролер тепер має новий endpoint `/search` для отримання сторінкованих результатів з можливістю пошуку.

## Параметри пошуку

Всі нові endpoint'и використовують `SearchParametersDto` з наступними параметрами:

- `PageNumber` (int) - номер сторінки (за замовчуванням: 1)
- `PageSize` (int) - розмір сторінки (за замовчуванням: 10, максимум: 100)
- `SearchTerm` (string) - термін для пошуку (опціонально)
- `SortBy` (string) - поле для сортування (опціонально)
- `SortDirection` (string) - напрямок сортування: "asc" або "desc" (за замовчуванням: "asc")

## Нові Endpoint'и

### 1. Компанії (`/api/Companie/search`)

**GET** `/api/Companie/search`

**Параметри сортування:**
- `companyname` - назва компанії
- `emailcompany` - email компанії
- `address` - адреса
- `phone` - телефон
- `activitytypename` - назва типу активності

**Поля для пошуку:**
- Назва компанії
- Email компанії
- Адреса
- Телефон
- Назва типу активності

**Приклад запиту:**
```
GET /api/Companie/search?PageNumber=1&PageSize=10&SearchTerm=tech&SortBy=companyname&SortDirection=asc
```

### 2. Вакансії (`/api/Vacancy/search`)

**GET** `/api/Vacancy/search`

**Параметри сортування:**
- `title` - назва вакансії
- `salary` - зарплата
- `createdat` - дата створення
- `isopen` - статус відкриття
- `companyname` - назва компанії

**Поля для пошуку:**
- Назва вакансії
- Опис
- Вимоги
- Зарплата
- Назва компанії

**Приклад запиту:**
```
GET /api/Vacancy/search?PageNumber=1&PageSize=5&SearchTerm=developer&SortBy=salary&SortDirection=desc
```

### 3. Працівники (`/api/Worker/search`)

**GET** `/api/Worker/search`

**Параметри сортування:**
- `lastname` - прізвище
- `firstname` - ім'я
- `qualification` - кваліфікація
- `email` - email
- `expectedsalary` - очікувана зарплата
- `activitytypename` - назва типу активності

**Поля для пошуку:**
- Прізвище
- Ім'я
- Кваліфікація
- Email
- Очікувана зарплата
- Назва типу активності

**Приклад запиту:**
```
GET /api/Worker/search?PageNumber=1&PageSize=15&SearchTerm=senior&SortBy=expectedsalary&SortDirection=desc
```

### 4. Типи активності (`/api/ActivityType/search`)

**GET** `/api/ActivityType/search`

**Параметри сортування:**
- `activityname` - назва активності

**Поля для пошуку:**
- Назва активності

**Приклад запиту:**
```
GET /api/ActivityType/search?PageNumber=1&PageSize=20&SearchTerm=IT&SortBy=activityname&SortDirection=asc
```

## Структура відповіді

Всі endpoint'и повертають об'єкт `PagedList<T>` з наступною структурою:

```json
{
  "currentPage": 1,
  "totalPages": 5,
  "pageSize": 10,
  "totalCount": 47,
  "hasPrevious": false,
  "hasNext": true,
  "items": [
    // Масив об'єктів відповідного типу
  ]
}
```

## Валідація

- `PageNumber` повинен бути більше 0
- `PageSize` повинен бути від 1 до 100
- `SortDirection` приймає значення "asc" або "desc" (нечутливо до регістру)

## Помилки

При неправильних параметрах повертається HTTP 400 Bad Request з детальним описом помилок валідації.

## Авторизація

Всі нові endpoint'и доступні анонімно (`[AllowAnonymous]`), як і існуючі GET методи.

## Примітки

1. Пошук виконується без урахування регістру
2. Пошук здійснюється по частковому співпадінню (Contains)
3. Якщо не вказано параметр сортування, використовується сортування за замовчуванням для кожного типу сутності
4. Всі endpoint'и підтримують CancellationToken для скасування операцій 