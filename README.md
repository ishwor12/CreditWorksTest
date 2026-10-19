1.CreditWorksTest
CreditWorks Software Engineer – Technical Programming Assignment

2.Vehicle Management

- Add vehicles and view them in a sorted list.
- A vehicle's category (with icon) is worked out automatically from its weight.
- Categories can be created, changed and deleted. The weight ranges are always validated so there are no gaps and no overlaps.
- Changing a category range immediately changes the category shown for every existing vehicle.

3. Technology
   Backend C#, ASP.NET Core Web API (.NET 10), Entity Framework Core
   Database Microsoft SQL Server (2019 )
   Frontend React (JavaScript), Vite, plain CSS
   Tests xUnit (unit tests and integration tests)

4. Setup and Required SDKs

- .NET 10 SDK
- SQL Server 2019
- Node.js
- Git

5. Database

The default connection string: `src/VehicleManagement.Api/appsettings.json`. SQL Server LocalDB used
-- Server=(localdb)\MSSQLLocalDB;Database=CreditWorks;Trusted_Connection=True;TrustServerCertificate=True

-- use migration for initial run.

** data Seed ** The initial migration inserts the five manufacturers (Mazda, Mercedes, Honda, Ferrari, Toyota) and the three categories (Light 0 to under 500 kg, Medium 500 to under 2500 kg, Heavy 2500 kg and above).

6. Build and running application

Start the API (terminal 1, from the repository root):

dotnet run --project src/VehicleManagement.Api

working on: `http://localhost:5077`In Development, Swagger-Link: `http://localhost:5077/swagger`.

Start the frontend (terminal 2):

cd client
cd client --Mistakely made double folder
npm install
npm run dev

Open `http://localhost:5173`. The Vite dev server forwards requests that start with `/api` to `http://localhost:5077` defaut.

### Run the tests

```
dotnet test
```

- **Unit tests** need no database.
- **Integration tests** start the real API against a separate database called `CreditWorks_Tests`. It is created automatically and reset before every test, so the development data is never touched.

  7.Navbar as per assignment.

- Vehicles tab.
- Add vehicle tab.
- Categories tab.

8. API

`GET /api/manufacturers` manufacturers, sorted by name |
`GET /api/categories` categories, ordered by minimum weight |
`PUT /api/categories` Replaces the whole category list (updates, new rows and deleted rows) after validating it as one set.
`GET /api/vehicles?sortBy=&descending=` vehicles with their calculated category. `sortBy` is `owner` (default), `manufacturer`, `year` or `weight`
`POST /api/vehicles` To Creates a vehicle. 9. Validation and Errors

Validation and not-found errors return JSON with a `title`, a `status` and an `errors`
Status codes are 400 for invalid input, 404 for missing records and 500 for unexpected errors.
The details go to the server log and no stack traces reach the client.

10. Design notes and Architecture

The solution has 4 projects in `src` and 2 test projects and 1 Client Prohect as Frontend:

src/VehicleManagement.Api Controllers, exception-handling middleware, startup
src/VehicleManagement.Application Services, DTOs, request validators, IAppDbContext
src/VehicleManagement.Domain Entities and the category range rules
src/VehicleManagement.Infrastructure EF Core DbContext, entity configuration, migrations

tests/VehicleManagement.UnitTests
tests/VehicleManagement.IntegrationTests

client React frontend

Dependencies point inwards: Api references Application and Infrastructure, Infrastructure references Application and Domain, and Application references Domain.

Domain references nothing.

Controllers only handle HTTP. Services hold the business logic and read and write data through a small `IAppDbContext` interface that the EF Core context implements.

I did not add a repository layer, because EF Core already provides one and the application is small.

The Application project references EFCore.

11. Database

Table
`Manufacturers` `Id`, `Name`. Name is unique. A table, so the list can change without a code change.

`VehicleCategories` `Id`, `Name` (unique), `IconKey`, `MinWeightKg` (inclusive), `MaxWeightKg` (exclusive, null means no upper limit). Weights are `decimal(18,2)`.

`Vehicles` `Id`, `OwnerName`, `ManufacturerId` (foreign key, delete restricted), `YearOfManufacture`, `WeightKg` (`decimal(18,2)`).

Considerations:
The `Vehicles` table has no category column. This is what lets a category change update every existing vehicle at once.

Icons are stored as a short key `scooter`. The frontend maps the key to a picture (emoji).

- Database Vlaidations:
  weight is always greater than 0
  year is not before 1886
  category's minimum is non negative,
  and category maximum greater than its minimum.
  gaps and overlaps cannot be expressed as a row constraint, so they are enforced by `CategoryRangeValidator`.

12. Category Assumptions and rules

Categories are loaded (the table is tiny) and the first one whose range contains the vehicle's weight is used. The rule lives in one method, `VehicleCategory.Contains`.
Sorting of the vehicle list runs in SQL. The sort field is checked against a fixed list.

Ranges are half-open: the minimum is included and the maximum is excluded, written `[Min, Max)`.

- A vehicle weighing exactly 500.00 kg is Medium, not Light.
- A vehicle weighing exactly 2500.00 kg is Heavy, not Medium.
- Every weight therefore belongs to exactly one category.
- The lowest category must start at 0 kg, and the highest category has no upper limit, so every positive weight is covered.

`CategoryRangeValidator` sorts the proposed categories by minimum weight and checks that:

1. the lowest category starts at 0,
2. each category's maximum is greater than its minimum,
3. each category ends exactly where the next one starts
4. only the highest category has no upper limit, and
5. boundaries have at most two decimal places.

The category list is saved as a whole (`PUT /api/categories`) and validated as a whole.
The changes are applied in a single database transaction, so a request either succeeds completely or changes nothing.

13. Validation

All validation runs on the server.

- Vehicles: owner name required (up to 200 characters), manufacturer required and must exist, year of manufacture between 1886 and next year, weight greater than 0 and at most 1,000,000 kg with at most two decimal places.

- Categories: name required (up to 100 characters) and unique ignoring upper and lower case, icon required (up to 50 characters), at least one category, plus the range rules above.

14. Keys

- All data access goes through Entity Framework Core.
- All input is validated on the server, wtih error responses details.
- No secret keys. The default connection string uses Windows authentication

15. Assumptions

- Manufacturers are fixed reference data managed through migrations.
- The maximum vehicle weight of 1,000,000 kg and the allowed year range (1886 to next year) are my own assumption.
- Category names are unique, ignoring upper and lower case. At least one category must always exist.
- Vehicles can be added and listed. Editing and deleting vehicles is not included.
- HTTPS redirection is switched off in the Development environment only, so the Vite dev proxy can call the API over http.

16. What I would have improved

- Add paging, and indexes on the sortable columns, for large numbers of vehicles.
- Cache the small category table and refresh it when categories change.
- Add authentication, authorisation .
- Validate icon keys on the server and serve real SVG icons.
- Move the frontend's `fetch` calls into a small API layers.
- Add structured logging .

17. -------------------------- Review and Decleration of work ------------------------

I, Ishwor Khatiwada have completed this Test with my knowlwdge and abilities and have used AI (Claude Code) For

- review
- test cases
- to check i have meet all assignment tasks.
- Refactoring codes and cleaning bugs and issues in between.
- In Meantime Some lines have been Directly Copied Directly from Claude(AI).
