<Query Kind="Expression">
  <Connection>
    <ID>a935a395-cebe-4d7a-84d9-914a51b43e82</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 2 && p.AssistanceConditionsV006.IDUMP == 1).Select(p=>
FactMedicalEvents.Where(r=>
r.FactPatient.AccountId == 2 && 
r.AssistanceConditionsV006.IDUMP == 3 && 
r.EventBegin <= p.EventEnd.Value.AddDays(-1) &&
p.EventBegin.Value.AddDays(1) <= r.EventEnd &&
r.FactPatient.PersonalIdFactPerson == p.FactPatient.PersonalIdFactPerson
).Select(r=>r.MedicalEventId) 
).SelectMany(p=>p)