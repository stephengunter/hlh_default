# hlh_default
dotnet run --project ./IdentityWeb 

dotnet run --launch-profile https --project ./IdentityWeb
dotnet run --launch-profile https --project ./IT_Api
dotnet run --launch-profile https --project ./Doc3Api

dotnet build ./hlh_default.sln && dotnet run --project ./IdentityWeb 

dotnet ef migrations add Event-test -s ../IdentityWeb -o PostgreSqlMigrations/

dotnet ef migrations add init -s ../IdentityWeb --context IdentityContext --output-dir Migrations/Identity
dotnet ef migrations add init -s ../ITApi --context ITContext --output-dir Migrations/IT_Api
dotnet ef migrations add init -s ../Doc3Api --context Doc3Context --output-dir Migrations/Doc

dotnet ef migrations add init -s ../IdentityWeb
dotnet ef migrations remove -s ../IdentityWeb
dotnet ef database update -s ../IdentityWeb --context IdentityContext
dotnet ef database update -s ../ITApi --context ITContext
dotnet ef database update -s ../Doc3Api --context Doc3Context

