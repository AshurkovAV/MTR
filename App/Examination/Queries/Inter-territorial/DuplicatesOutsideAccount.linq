<Query Kind="Expression">
  <Connection>
    <ID>955e9d89-9d1f-4c2b-a8d7-8c16138f0fec</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId != 2)
.Select(p=>
FactMedicalEvents.Where(r=>
r.FactPatient.AccountId == 2 && 
r.SpecialityCode == p.SpecialityCode && 
r.EventBegin == p.EventBegin &&
r.EventEnd == p.EventEnd &&
r.ProfileCodeId == p.ProfileCodeId &&
(r.FactPatient.INP == p.FactPatient.INP || (p.FactPatient.INP == null && r.FactPatient.INP == null)) &&
(r.FactPatient.InsuranceDocNumber == p.FactPatient.InsuranceDocNumber || (r.FactPatient.InsuranceDocNumber == null && p.FactPatient.InsuranceDocNumber == null)) &&
r.FactPatient.Newborn == p.FactPatient.Newborn && r.FactPatient.Newborn == "0" &&
r.FactPatient.AccountIdFactTerritoryAccount.Type == 1 &&
r.FactPatient.PersonalIdFactPerson.PName == p.FactPatient.PersonalIdFactPerson.PName &&
r.FactPatient.PersonalIdFactPerson.Surname == p.FactPatient.PersonalIdFactPerson.Surname &&
r.FactPatient.PersonalIdFactPerson.Patronymic == p.FactPatient.PersonalIdFactPerson.Patronymic &&
r.FactPatient.PersonalIdFactPerson.Birthday == p.FactPatient.PersonalIdFactPerson.Birthday 
).Select(r=>r.MedicalEventId) 
).SelectMany(p=>p)