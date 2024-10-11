# 📰 News Parser

News Parser — это парсер новостей с сайта zakon.kz, созданный на базе ASP.NET Core, в качестве тестового задания.

## 📚 Стек технологий

- **ASP.NET Core** — для построения высокопроизводительных веб-приложений.
- **HttpClient** — для работы с HTTP-запросами и получения данных с веб-ресурсов.
- **Dapper** — легковесный ORM для быстрого взаимодействия с базой данных.
- **Npgsql** — провайдер для работы с базами данных PostgreSQL через .NET.
- **Docker** — для контейнеризации приложения.

## 🚀 Как запустить

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/nizamike229/testParser.git
2. Перейдите в директорию проекта:
   ```bash
   cd testParser
   ```
3. Убедитесь, что у вас установлен Docker
<br>[Установка Docker](https://www.docker.com/products/docker-desktop/)

4. Запустите приложение с помощью Docker Compose:
   ```bash
   docker-compose up --build
   ```

5. Доступ к приложению:
   <br>После успешного запуска приложение будет доступно по адресу:
   ```bash
   http://localhost:5000
   ```
   
 
