# ProductCatalog

Middleware REST API koji izlaže proizvode s naprednim filtriranjem/pretragom, razvijen za Abysalto Backend Akademiju. Prvi izvor podataka je [DummyJSON](https://dummyjson.com/), a arhitektura je namjerno napravljena tako da se u budućnosti mogu dodati i drugi izvori (baza, file system, RSS...).

## Tehnološki stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 + SQL Server
- JWT autentifikacija/autorizacija
- Serilog (log u konzolu + SQL Server tablicu)
- `IMemoryCache` za caching filter/search upita
- xUnit + Moq (unit testovi)

## Arhitektura

Clean Architecture, četiri projekta:

- **ProductCatalog.Domain** — entiteti (`Product`, `Category`), sučelja repozitorija, `Result<T>` pattern. Bez ovisnosti o ičemu drugom.
- **ProductCatalog.Application** — use-case handleri (`RequestHandler<TRequest,TResponse>`), DTO-i, sučelja za cross-cutting servise (`ICacheService`, `IJwtService`, `IAuthService`).
- **ProductCatalog.Infrastructure** — EF Core `DbContext` i repozitoriji (SQL Server), `DummyJsonService` (komunikacija s DummyJSON-om), sinkronizacijski servisi, JWT/cache implementacije.
- **ProductCatalog.Api** — kontroleri, middleware, DI wiring, Swagger.

Proizvodi i kategorije se **sinkroniziraju iz DummyJSON-a u lokalnu SQL Server bazu** (pri startu aplikacije, i svaku ponoć u pozadini) — endpointi čitaju iz baze, ne uživo s vanjskog servisa. Ako se izvor promijeni (proizvod/kategorija nestane, doda se novi), sync to prepoznaje i radi insert/update/delete po razlici.

## Preduvjeti

- .NET 8 SDK
- SQL Server instanca dostupna lokalno (Express/Developer edition, ili LocalDB).

## Konfiguracija

`appsettings.Development.json` nije u repozitoriju (gitignored, sadrži tajne). Kreiraj ga ručno u `ProductCatalog.Api/` prije prvog pokretanja:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductCatalogDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Secret": "promijeni-ovo-u-svoj-dovoljno-dugacak-secret"
  }
}
```

Prilagodi `DefaultConnection` svom SQL Server setupu.

**Bazu treba ručno kreirati (praznu) prije prvog pokretanja** — npr. preko SQL Server Management Studija

## Pokretanje

Pri prvom pokretanju aplikacija:

1. Primjeni EF Core migracije na (praznu, ručno kreiranu) bazu — kreira tablice ako ne postoje.
2. Odmah sinkronizira kategorije i proizvode s DummyJSON-a u bazu.
3. U pozadini pokreće job koji istu sinkronizaciju ponavlja svaku ponoć (insert/update/delete po razlici naspram vanjskog izvora).

Swagger UI dostupan na `/swagger` u Development okruženju.

## Autentifikacija

Login ide uživo prema DummyJSON-u (`POST auth/login` + `GET auth/me`), a lokalni JWT se generira iz tog odgovora — nema lokalne baze korisnika (namjerno, da obrisan/promijenjen korisnik na DummyJSON-u ne zadrži pristup do sljedećeg sync-a).

```
POST /api/auth/login
{
  "username": "emilys",
  "password": "emilyspass"
}
```

Vraća `{ "token": "<jwt>" }`. Testni korisnici i njihove role (`admin`/ostalo) se mogu provjeriti na [dummyjson.com/users](https://dummyjson.com/users) — `GET /api/products/filter` zahtijeva korisnika s `role: admin`, ostali product endpointi rade s bilo kojim važećim tokenom.

Token se šalje kao `Authorization: Bearer <token>` header.

## Endpointi

| Metoda | Ruta                                                 | Opis                                                        | Auth               |
| ------ | ---------------------------------------------------- | ----------------------------------------------------------- | ------------------ |
| POST   | `/api/auth/login`                                    | Prijava, vraća JWT                                          | -                  |
| GET    | `/api/products`                                      | Lista proizvoda (slika, naziv, cijena, opis do 100 znakova) | JWT                |
| GET    | `/api/products/{id}`                                 | Detalji proizvoda                                           | JWT                |
| GET    | `/api/products/filter?category=&minPrice=&maxPrice=` | Filtriranje po kategoriji/cijeni                            | JWT + rola `admin` |
| GET    | `/api/products/search?searchTerm=`                   | Pretraga po nazivu/opisu/brendu                             | JWT                |

Filter i Search rezultati su keširani u memoriji (5 min) po kombinaciji parametara — ponovljeni identični pozivi ne idu ponovno na bazu.

## Testiranje

Unit testovi (mockane ovisnosti, bez baze/mreže):

```
dotnet test tests/ProductCatalog.UnitTests
```

Za ručno testiranje endpointa koristi Swagger UI (`/swagger`).

## Logiranje

Serilog piše u konzolu i u `Logs` tablicu u istoj SQL Server bazi (tablica se automatski kreira pri prvom zapisu).

## Korištenje AI alata

Korišten Claude. U komentarima naznačeno u kojim dijelovima je točno korišten.
