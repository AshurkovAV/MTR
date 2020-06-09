<Query Kind="Expression">
  <Connection>
    <ID>955e9d89-9d1f-4c2b-a8d7-8c16138f0fec</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId != 11 && p.AssistanceConditionsV006.IDUMP == 1)
.Select(p=>
	FactMedicalEvents.Where(r=>
		r.FactPatient.AccountId == 11 && 
		r.AssistanceConditionsV006.IDUMP == 1 && 
		r.EventBegin <= p.EventEnd.Value.AddDays(-1) &&
		p.EventBegin.Value.AddDays(1) <= r.EventEnd &&
		r.FactPatient.PersonalIdFactPerson.PName == p.FactPatient.PersonalIdFactPerson.PName &&
		r.FactPatient.PersonalIdFactPerson.Surname == p.FactPatient.PersonalIdFactPerson.Surname &&
		r.FactPatient.PersonalIdFactPerson.Patronymic == p.FactPatient.PersonalIdFactPerson.Patronymic &&
		r.FactPatient.PersonalIdFactPerson.Birthday == p.FactPatient.PersonalIdFactPerson.Birthday &&
		r.FactPatient.Newborn == "0")                                  
.Select(r=>new {r.MedicalEventId, l = p.MedicalEventId}) 
).SelectMany(p=>p)