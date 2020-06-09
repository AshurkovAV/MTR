<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 1).Where(p=>
Math.Floor((p.EventBegin - p.FactPatient.PersonalIdFactPerson.Birthday).Value.Days / 365.25) < 18.0 && (p.IsChildren == null || p.IsChildren == false)).Select(p=>p.MedicalEventId)