<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId != 6280 && p.AssistanceConditionsV006.IDUMP == 1)
.Select(p=>
	FactMedicalEvents.Where(r=>
		r.FactPatient.AccountId == 6280 && 
		
		r.AssistanceConditionsV006.IDUMP == 1 && 
		r.EventBegin <= p.EventEnd.Value.AddDays(-1) &&
		p.EventBegin.Value.AddDays(1) <= r.EventEnd &&
		r.FactPatient.PersonalIdFactPerson.PName == p.FactPatient.PersonalIdFactPerson.PName &&
		r.FactPatient.PersonalIdFactPerson.Surname == p.FactPatient.PersonalIdFactPerson.Surname &&
		r.FactPatient.PersonalIdFactPerson.Patronymic == p.FactPatient.PersonalIdFactPerson.Patronymic &&
		r.FactPatient.PersonalIdFactPerson.Birthday == p.FactPatient.PersonalIdFactPerson.Birthday)                                  
.Select(r=>r.MedicalEventId) 
).SelectMany(p=>p)