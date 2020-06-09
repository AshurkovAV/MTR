<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactEconomicAccounts.Select(p=> new { 
	account=p, 
	Date = p.AccountIdFactTerritoryAccount.Date, 
	AccountDate=p.AccountIdFactTerritoryAccount.AccountDate,
	AccountNumber=p.AccountIdFactTerritoryAccount.AccountNumber,
	Price=p.AccountIdFactTerritoryAccount.Price,
	AcceptPrice=p.AccountIdFactTerritoryAccount.AcceptPrice,
	Source=p.AccountIdFactTerritoryAccount.Source,
	Destination=p.AccountIdFactTerritoryAccount.Destination,
	Status=p.AccountIdFactTerritoryAccount.Status,
	Direction=p.AccountIdFactTerritoryAccount.Direction,
	HospitalFact=p.FactEconomicPayments.Where(r=>r.AssistanceConditionsId == 1).Sum(r=>(decimal?)r.Amount)??0M,
	DayHospitalFact=p.FactEconomicPayments.Where(r=>r.AssistanceConditionsId == 2).Sum(r=>(decimal?)r.Amount)??0M,
	PolyclinicFact=p.FactEconomicPayments.Where(r=>r.AssistanceConditionsId == 3).Sum(r=>(decimal?)r.Amount)??0M,
	AmbulanceFact=p.FactEconomicPayments.Where(r=>r.AssistanceConditionsId == 4).Sum(r=>(decimal?)r.Amount)??0M,
	HospitalAcceptPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 1).Sum(s=>s.AcceptPrice),
	DayHospitalAcceptPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 2).Sum(s=>s.AcceptPrice),
	PolyclinicAcceptPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 3).Sum(s=>s.AcceptPrice),
	AmbulanceAcceptPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 4).Sum(s=>s.AcceptPrice),
	HospitalPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 1).Sum(s=>s.Price),
	DayHospitalPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 2).Sum(s=>s.Price),
	PolyclinicPrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 3).Sum(s=>s.Price),
	AmbulancePrice=FactMedicalEvents.Where(s=>s.FactPatient.AccountId == p.AccountIdFactTerritoryAccount.TerritoryAccountId && s.AssistanceConditions == 4).Sum(s=>s.Price),
	} 
)