<Query Kind="Expression">
  <Connection>
    <ID>955e9d89-9d1f-4c2b-a8d7-8c16138f0fec</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactTerritoryAccounts.Where(p=>p.TerritoryAccountId == 8).Where(p=>
p.Date == null ||
p.Source == null ||
p.Destination == null ||
(p.AccountNumber == null || p.AccountNumber == string.Empty) ||
p.Price == null ||
p.AcceptPrice == null
)
.Select(p=>new {ID = p.TerritoryAccountId, Description = "Тест"})