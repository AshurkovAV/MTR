<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId != 18 && p.AssistanceConditionsV006.IDUMP == 2).Select(p=>
FactMedicalEvents.Where(r=>
r.FactPatient.AccountId == 18 && 
r.AssistanceConditionsV006.IDUMP == 1 && 
r.EventBegin <= p.EventEnd.Value.AddDays(-1) &&
p.EventBegin.Value.AddDays(1) <= r.EventEnd &&
r.FactPatient.PersonalIdFactPerson == p.FactPatient.PersonalIdFactPerson
).Select(r=>r.MedicalEventId) 
).SelectMany(p=>p)