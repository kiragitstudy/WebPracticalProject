-- Расширение для UUID (генерация ключей)
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Отдельная схема приложения
CREATE SCHEMA IF NOT EXISTS app;
SET search_path TO app, public;

CREATE TABLE IF NOT EXISTS app.users (
  id               UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  email            TEXT        NOT NULL UNIQUE,
  password_hash    TEXT,
  display_name     TEXT,
  role             TEXT        NOT NULL DEFAULT 'customer',
  google_id        TEXT UNIQUE,          -- для OAuth
  email_confirmed  BOOLEAN     NOT NULL DEFAULT false,
  created_at       TIMESTAMPTZ NOT NULL DEFAULT now(),
  last_login_at    TIMESTAMPTZ
);

-- ============= CONTACT MESSAGES (форма "Написать сообщение") =============
CREATE TABLE IF NOT EXISTS app.contact_messages (
  id          UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  name        TEXT,
  email       TEXT        NOT NULL,
  subject     TEXT,
  message     TEXT        NOT NULL,
  created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============= INSTRUMENTS (каталог) =============
CREATE TABLE IF NOT EXISTS app.instruments (
  id             UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  title          TEXT        NOT NULL,
  brand          TEXT,
  category       TEXT,
  description    TEXT,
  image_url      TEXT,                            -- одна главная картинка
  price_per_day  NUMERIC(10,2) NOT NULL,
  is_featured    BOOLEAN     NOT NULL DEFAULT false,  -- для /Catalog/Popular
  is_active      BOOLEAN     NOT NULL DEFAULT true,
  created_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_instruments_category ON app.instruments(category);
CREATE INDEX IF NOT EXISTS ix_instruments_brand ON app.instruments(brand);
CREATE INDEX IF NOT EXISTS ix_instruments_price ON app.instruments(price_per_day);
CREATE INDEX IF NOT EXISTS ix_instruments_featured ON app.instruments(is_featured);

-- ============= RENTALS (демо-заказы аренды, максимально просто) =============
CREATE TABLE IF NOT EXISTS app.rentals (
  id            UUID        PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id       UUID        REFERENCES app.users(id) ON DELETE SET NULL,
  instrument_id UUID        NOT NULL REFERENCES app.instruments(id) ON DELETE CASCADE,
  start_at      TIMESTAMPTZ NOT NULL,
  end_at        TIMESTAMPTZ NOT NULL,
  status        TEXT        NOT NULL DEFAULT 'draft',  -- draft|active|closed|cancelled
  total_amount  NUMERIC(12,2),
  created_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
  CHECK (end_at > start_at)
);

CREATE INDEX IF NOT EXISTS ix_rentals_user ON app.rentals(user_id);
CREATE INDEX IF NOT EXISTS ix_rentals_instr ON app.rentals(instrument_id);
CREATE INDEX IF NOT EXISTS ix_rentals_status ON app.rentals(status);

ALTER TABLE app.contact_messages
    ADD COLUMN IF NOT EXISTS user_id UUID REFERENCES app.users(id) ON DELETE SET NULL;
CREATE INDEX IF NOT EXISTS ix_contact_messages_user ON app.contact_messages(user_id);
