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
(p.FactPatient.INP == null || p.FactPatient.INP == string.Empty)) 
|| p.FactPatient.InsuranceDocType == null)
.Select(p=>p.PatientId).Distinct()