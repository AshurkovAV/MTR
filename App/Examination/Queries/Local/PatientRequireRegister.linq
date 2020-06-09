<Query Kind="Expression">
  <Connection>
    <ID>a935a395-cebe-4d7a-84d9-914a51b43e82</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.RegisterId == 10).Where(p=>
((p.FactPatient.InsuranceDocNumber == null || p.FactPatient.InsuranceDocNumber == string.Empty)&&
(p.FactPatient.INP == null || p.FactPatient.INP == string.Empty)) || 
p.FactPatient.InsuranceDocType == null ||
p.FactPatient.InsuranceId == null ||
(p.FactPatient.TerritoryOkato == null || p.FactPatient.TerritoryOkato == string.Empty)||
(p.FactPatient.Newborn == null || p.FactPatient.Newborn == string.Empty)||
p.FactPatient.PersonalIdFactPerson.Birthday == null ||
p.FactPatient.PersonalIdFactPerson.Sex == null ||
(p.FactPatient.PersonalIdFactPerson.PName == null || p.FactPatient.PersonalIdFactPerson.PName == string.Empty) || 
(p.FactPatient.PersonalIdFactPerson.Surname == null || p.FactPatient.PersonalIdFactPerson.Surname == string.Empty) ||
(p.FactPatient.PersonalIdFactPerson.Patronymic == null || p.FactPatient.PersonalIdFactPerson.Patronymic == string.Empty)
)
.Select(p=>p.PatientId).Distinct()