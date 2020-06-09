<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactTerritoryAccounts.Where(p=>p.TerritoryAccountId == 18).Where(p=>p.Price!=FactMedicalEvents.Where(r=>r.FactPatient.AccountId == p.TerritoryAccountId).Sum(r=>r.Price)).Select(p=>p.TerritoryAccountId)