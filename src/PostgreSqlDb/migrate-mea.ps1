Push-Location -Path "./Kmd.Momentum.Mea.DbAdmin"
  &dotnet run -- create -t "Server={Server};Database={Database};Port=5432;User Id={User};Password={Password};" -s localhost 
  &dotnet run -- migrate -t "Server={Server};Database={Database};Port=5432;User Id={User};Password={Password};" -s localhost -f ../../logic/MigrationScripts
Pop-Location