<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p => p.FactPatient.AccountId == 67).GroupBy(p => p.AssistanceConditions).Select(g => new {Assistance = g.Key, AcceptPrice = g.Sum(p => p.AcceptPrice), Price = g.Sum(p => p.Price), Mec=g.Sum(p=>p.MEC), ExternalRefuse=g.Sum(p=>p.FactExternalRefuses.Sum(r=>r.Amount))}).ToDictionary(h => h.Assistance)
