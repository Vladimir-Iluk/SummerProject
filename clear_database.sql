-- Скрипт для очищення бази даних PostgreSQL
-- Виконайте цей скрипт в pgAdmin або через psql

-- Видаляємо всі таблиці з публічної схеми
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

-- Відновлюємо права доступу
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public; 