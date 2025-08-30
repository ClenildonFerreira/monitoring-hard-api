# monitoring-hard-api

## Visão Geral

O **monitoring-hard-api** é uma API RESTful desenvolvida em ASP.NET 8 Com banco de dados MySQL Utilizando Pomelo e Entity Framework para pensitencia devido a utilização da
Vertical Slice Arquitetura(uma arquitetura modular) onde eu vou separa as responsabilidades por camadas eu add Carter para endpoints minimalistas e SignalR para comunicação em tempo real

---

## Arquitetura

- Vertical Slice Arquitetura - facilita manutenção, testes, escalabilidade e da uma Palhinha na SOLID

- **Domain:** Entidades centrais (`Device`, `Event`)
- \*\* Todo o projeto está dentro de uma (`src`)

---

## Tecnologias Utilizadas

- **Backend:** ASP.NET 8, Carter, FluentValidation, SignalR, Swashbuckle (Swagger)
- **Persistência:** Entity Framework
- **Banco de Dados:** MySQL

---

## Banco de Dados

- **Padrão:** MySQL
- **String de conexão padrão:**

  ```
  Server=localhost;Port=3307;Database=monitoramento;Uid=root;Pwd=root;
  ```

  (Definida em `appsettings.json`)

- **Entidades principais:**
  - **Device:** Id, Name, Location, IntegrationId, CreatedAt, UpdatedAt
  - **Event:** Id, DeviceId, Temperature, Humidity, IsAlarm, Timestamp

---

## Observações

- Pontos a melhorar: muita coisa kkkk poderia comerça pelo JWT, logs detalhados aumentando a observilidade melhorar poquinho endpoints melhorar consitencia entre eles acho que só isso.
- Os teste é só tristeza

> "Funciona na minha máquina! ( ͡◉ ͜ʖ ͡◉)"
